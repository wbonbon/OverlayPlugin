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

            // Prevent a stack overflow in the assembly loaded handler by loading the logger interface early.
            var commonAsm = Assembly.Load("OverlayPlugin.Common");

            // Check if the above line loaded an old version of OverlayPlugin.Common.dll
            var commonVersion = commonAsm.GetName().Version;
            var loaderVersion = Assembly.GetExecutingAssembly().GetName().Version;
            if (commonVersion != loaderVersion)
            {
                MessageBox.Show(
                    $"ACT loaded {commonAsm.Location} {commonVersion} which doesn't match your OverlayPlugin version " +
                    $"({loaderVersion}). Aborting plugin load.\n\n" +
                    "Please make sure the old OverlayPlugin is disabled and restart ACT." +
                    "If that doesn't fix the issue, remove the above mentioned file and any OverlayPlugin*.dll, CEF or " +
                    "HtmlRenderer.dll files in the same directory.",
                    "OverlayPlugin Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            Initialize(pluginScreenSpace, pluginStatusText);
        }

        // AssemblyResolver でカスタムリゾルバを追加する前に PluginMain が解決されることを防ぐために、
        // インライン展開を禁止したメソッドに処理を分離
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Initialize(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            pluginStatusText.Text = "Initializing Runtime...";

            Registry.Init();
            logger = new Logger();
            asmResolver.ExceptionOccured += (o, e) => logger.Log(LogLevel.Error, "AssemblyResolver: Error: {0}", e.Exception);
            asmResolver.AssemblyLoaded += (o, e) => logger.Log(LogLevel.Debug, "AssemblyResolver: Loaded: {0}", e.LoadedAssembly.FullName);
            pluginMain = new PluginMain(pluginDirectory, logger);

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
                pluginStatusText.Text = "Waiting for ACT to finish startup...";
            }
        }

        private async void InitPluginCore(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            pluginStatusText.Text = "Initializing CEF...";

            if (await CefInstaller.EnsureCef(GetCefPath()))
            {
                ActGlobals.oFormActMain.Invoke((Action) (() =>
                {
                    pluginMain.InitPlugin(pluginScreenSpace, pluginStatusText);
                }));
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
