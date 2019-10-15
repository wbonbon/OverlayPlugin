using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace RainbowMage.OverlayPlugin.Updater
{
    public class CefInstaller
    {
        const string CEF_DL = "https://github.com/ngld/OverlayPlugin/releases/download/v0.7.0/CefSharp-{CEF_VERSION}-{ARCH}.DO_NOT_DOWNLOAD";
        const string CEF_VERSION = "75.1.14";

        public static async Task<bool> EnsureCef(string cefPath)
        {
            var manifest = Path.Combine(cefPath, "version.txt");

            if (File.Exists(manifest))
            {
                var installed = File.ReadAllText(manifest).Trim();
                if (installed == CEF_VERSION)
                {
                    return true;
                }
            }

            return await InstallCef(cefPath);
        }

        public static async Task<bool> InstallCef(string cefPath)
        {
            while (!File.Exists("C:\\Windows\\system32\\msvcp140.dll"))
            {
                var response = MessageBox.Show(
                    "You're missing the MS Visual C++ Redistributable for Visual Studio 2019.\nYou'll need that for OverlayPlugin. Install it now?",
                    "OverlayPlugin",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (response == DialogResult.Yes)
                {
                    var installed = await Installer.InstallMsvcrt();

                    if (!installed)
                    {
                        MessageBox.Show(
                            "The MSVCRT installation failed.",
                            "OverlayPlugin",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                } else
                {
                    return false;
                }
            }

            var url = CEF_DL.Replace("{CEF_VERSION}", CEF_VERSION).Replace("{ARCH}", Environment.Is64BitProcess ? "x64" : "x86");

            var result = await Installer.Run(url, cefPath);
            if (!result || !Directory.Exists(cefPath))
            {
                var response = MessageBox.Show(
                    "Failed to download CEF! You will not be able to use OverlayPlugin. Retry?",
                    "OverlayPlugin Error",
                    MessageBoxButtons.YesNo
                );

                if (response == DialogResult.Yes)
                {
                    return await InstallCef(cefPath);
                } else
                {
                    return false;
                }
            } else
            {
                File.WriteAllText(Path.Combine(cefPath, "version.txt"), CEF_VERSION);
                return true;
            }
        }
    }
}
