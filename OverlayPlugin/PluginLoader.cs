using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using RainbowMage.HtmlRenderer;
using RainbowMage.OverlayPlugin.Updater;

namespace RainbowMage.OverlayPlugin
{
    public class PluginLoader : IActPluginV1
    {
        PluginMain pluginMain;
        Logger logger;
        static AssemblyResolver asmResolver;
        string pluginDirectory;

        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            if (asmResolver == null)
            {
                pluginDirectory = GetPluginDirectory();

                var directories = new List<string>();
                directories.Add(Path.Combine(pluginDirectory, "libs"));
                directories.Add(Path.Combine(pluginDirectory, "addons"));
                directories.Add(GetCefPath());
                asmResolver = new AssemblyResolver(directories);
            }

            /*
             * We explicitly load OverlayPlugin.Common here for two reasons:
             *  * To prevent a stack overflow in the assembly loaded handler when we use the logger interface.
             *  * To check that the loaded version matches.
             */
            if (!SanityChecker.LoadSaneAssembly("OverlayPlugin.Common"))
            {
                pluginStatusText.Text = Resources.FailedToLoadCommon;
                return;
            }

            Initialize(pluginScreenSpace, pluginStatusText);
        }

        // AssemblyResolver でカスタムリゾルバを追加する前に PluginMain が解決されることを防ぐために、
        // インライン展開を禁止したメソッドに処理を分離
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Initialize(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            pluginStatusText.Text = Resources.InitRuntime;

            Registry.Init();
            logger = new Logger();
            asmResolver.ExceptionOccured += (o, e) => logger.Log(LogLevel.Error, Resources.AssemblyResolverError, e.Exception);
            asmResolver.AssemblyLoaded += (o, e) => logger.Log(LogLevel.Debug, Resources.AssemblyResolverLoaded, e.LoadedAssembly.FullName);
            pluginMain = new PluginMain(pluginDirectory, logger);

            // Load the assembly for CefInstaller and make sure the version matches.
            if (!SanityChecker.LoadSaneAssembly("OverlayPlugin.Updater"))
            {
                pluginStatusText.Text = Resources.CefInstallerFailed;
                return;
            }

            if (ActGlobals.oFormActMain.Visible)
            {
                // ACT is running and this plugin was added. Immediately initialize!
                InitPluginCore(pluginScreenSpace, pluginStatusText);
            }
            else
            {
                // ACT is starting up and loading plugins. Wait until it's done and the main window becomes visible before we start initializing.
                EventHandler initHandler = null;
                initHandler = (o, e) =>
                {
                    ActGlobals.oFormActMain.VisibleChanged -= initHandler;
                    InitPluginCore(pluginScreenSpace, pluginStatusText);
                };

                ActGlobals.oFormActMain.VisibleChanged += initHandler;
                pluginStatusText.Text = Resources.WaitingForAct;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private async void InitPluginCore(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            pluginStatusText.Text = Resources.InitCef;

            if (await CefInstaller.EnsureCef(GetCefPath()))
            {
                // Finally, load the core and html renderer. We load these here since Core depends on HtmlRenderer and that depends on CEF.
                // Thus we can't load these before the CefInstaller is done.
                if (SanityChecker.LoadSaneAssembly("OverlayPlugin.Core") && SanityChecker.LoadSaneAssembly("HtmlRenderer"))
                {
                    ActGlobals.oFormActMain.Invoke((Action)(() =>
                    {
                        pluginMain.InitPlugin(pluginScreenSpace, pluginStatusText);
                    }));
                } else
                {
                    pluginStatusText.Text = Resources.CoreOrHtmlRendererInsane;
                }
            }
        }

        public void DeInitPlugin()
        {
            if (pluginMain != null)
            {
                pluginMain.DeInitPlugin();
                Registry.Clear();
            }

            // We can't re-init CEF after shutting it down. So let's only do that when ACT closes to avoid unexpected behaviour (crash when re-enabling the plugin).
            // TODO: Figure out how to detect disabling plugin vs ACT shutting down.
            // ShutdownRenderer(null, null);
        }

        /*
        public void ShutdownRenderer(object sender, EventArgs e)
        {
            Renderer.Shutdown();

            // We can only dispose the resolver once the HtmlRenderer is shut down.
            asmResolver.Dispose();
        }
        */

        private string GetPluginDirectory()
        {
            // ACT のプラグインリストからパスを取得する
            // Assembly.CodeBase からはパスを取得できない
            var plugin = ActGlobals.oFormActMain.ActPlugins.Where(x => x.pluginObj == this).FirstOrDefault();
            if (plugin != null)
            {
                return Path.GetDirectoryName(plugin.pluginFile.FullName);
            }
            else
            {
                throw new Exception("Could not find ourselves in the plugin list!");
            }
        }

        private string GetCefPath()
        {
            return Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, "OverlayPluginCef", Environment.Is64BitProcess ? "x64" : "x86");
        }
    }
}
