using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RainbowMage.OverlayPlugin
{
    public static class SanityChecker
    {
        /**
         * The assembly load order is as follows:
         * 
         *   * OverlayPlugin
         *   * OverlayPlugin.Common (PluginLoader.InitPlugin: version check, Logger & Registry)
         *   * OverlayPlugin.Updater (PluginLoader.InitPluginCore: CEF installer / updater)
         *   * OverlayPlugin.Core (PluginLoader.InitPluginCore)
         *   * OverlayPlugin.Core (PluginMain.InitPlugin)
         */

        public static bool LoadSaneAssembly(string name)
        {
            var loaderVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var asm = Assembly.Load(name);
            var asmVersion = asm.GetName().Version;

            if (loaderVersion != asmVersion)
            {
                MessageBox.Show(
                    $"ACT tried to load {asm.Location} {asmVersion} which doesn't match your OverlayPlugin version " +
                    $"({loaderVersion}). Aborting plugin load.\n\n" +
                    "Please make sure the old OverlayPlugin is disabled and restart ACT." +
                    "If that doesn't fix the issue, remove the above mentioned file and any OverlayPlugin*.dll, CEF or " +
                    "HtmlRenderer.dll files in the same directory.",
                    "OverlayPlugin Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return false;
            }

            return true;
        }
    }
}
