﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RainbowMage.OverlayPlugin.Updater
{
    public class CefInstaller
    {

        const string CEF_DL = "https://github.com/OverlayPlugin/CeF/releases/download/v1.0.0/CefSharp-{CEF_VERSION}-{ARCH}.7z";
        const string CEF_VERSION = "131.3.10";

        public static string GetUrl()
        {
            return CEF_DL.Replace("{CEF_VERSION}", CEF_VERSION).Replace("{ARCH}", Environment.Is64BitProcess ? "x64" : "x86");
        }

        public static async Task<bool> EnsureCef(string cefPath)
        {
            // Ensure we have a working MSVCRT first
            var lib = IntPtr.Zero;
            while (true)
            {
                lib = NativeMethods.LoadLibrary("msvcp140.dll");
                if (lib != IntPtr.Zero)
                {
                    NativeMethods.FreeLibrary(lib);

                    if (!Environment.Is64BitProcess) break;

                    lib = NativeMethods.LoadLibrary("vcruntime140_1.dll");
                    if (lib != IntPtr.Zero)
                    {
                        NativeMethods.FreeLibrary(lib);
                        break;
                    }
                }

                var response = MessageBox.Show(
                    Resources.MsvcrtMissing,
                    Resources.OverlayPluginTitle,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (response == DialogResult.Yes)
                {
                    var installed = await InstallMsvcrt();

                    if (!installed)
                    {
                        MessageBox.Show(
                            Resources.MsvcrtFailed,
                            Resources.OverlayPluginTitle,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
                else
                {
                    return false;
                }
            }

            var manifest = Path.Combine(cefPath, "version.txt");
            var importantFiles = new List<string>() { "CefSharp.dll", "CefSharp.Core.dll", "CefSharp.OffScreen.dll", "CefSharp.BrowserSubprocess.exe", "CefSharp.BrowserSubprocess.Core.dll", "libcef.dll", "libEGL.dll", "libGLESv2.dll" };

            if (File.Exists(manifest))
            {
                var installed = File.ReadAllText(manifest).Trim();
                if (installed == CEF_VERSION)
                {
                    // Verify all important files exist
                    var itsFine = true;
                    foreach (var name in importantFiles)
                    {
                        if (!File.Exists(Path.Combine(cefPath, name)))
                        {
                            itsFine = false;
                            break;
                        }
                    }

                    if (itsFine) return true;
                }
            }

            return await InstallCef(cefPath);
        }

        public static async Task<bool> InstallCef(string cefPath, string archivePath = null)
        {
            var result = await Installer.Run(archivePath == null ? GetUrl() : archivePath, cefPath, "OverlayPluginCef.tmp");
            if (!result || !Directory.Exists(cefPath))
            {
                MessageBox.Show(
                    Resources.UpdateCefDlFailed,
                    Resources.ErrorTitle,
                    MessageBoxButtons.OK
                );

                return false;
            }
            else
            {
                File.WriteAllText(Path.Combine(cefPath, "version.txt"), CEF_VERSION);
                return true;
            }
        }

        public static async Task<bool> InstallMsvcrt()
        {
            var inst = new Installer(Path.Combine(Path.GetTempPath(), "OverlayPlugin.tmp"), "msvcrt");
            var exePath = Path.Combine(inst.TempDir, "vc_redist.x64.exe");

            var libname = Environment.Is64BitProcess ? "vcruntime140_1.dll" : "vcruntime140.dll";

            return await Task.Run(() =>
            {
                if (inst.Download("https://aka.ms/vs/17/release/vc_redist.x64.exe", exePath))
                {
                    inst.Display.UpdateStatus(0, string.Format(Resources.StatusLaunchingInstaller, 2, 2));
                    inst.Display.Log(Resources.LogLaunchingInstaller);

                    try
                    {
                        var proc = Process.Start(exePath);
                        proc.WaitForExit();
                        proc.Close();
                    }
                    catch (System.ComponentModel.Win32Exception ex)
                    {
                        inst.Display.Log(string.Format(Resources.LaunchingInstallerFailed, ex.Message));
                        inst.Display.Log(Resources.LogRetry);

                        using (var proc = new Process())
                        {
                            proc.StartInfo.FileName = exePath;
                            proc.StartInfo.UseShellExecute = true;
                            proc.Start();
                        }

                        var cancel = inst.Display.GetCancelToken();

                        inst.Display.Log(Resources.LogInstallerWaiting);
                        while (NativeMethods.LoadLibrary(libname) == IntPtr.Zero && !cancel.IsCancellationRequested)
                        {
                            Thread.Sleep(500);
                        }

                        // Wait some more just to be sure that the installer is done.
                        Thread.Sleep(1000);
                    }

                    inst.Cleanup();
                    if (NativeMethods.LoadLibrary(libname) != IntPtr.Zero)
                    {
                        inst.Display.Close();
                        return true;
                    }
                    else
                    {
                        inst.Display.UpdateStatus(1, Resources.StatusError);
                        inst.Display.Log(Resources.LogInstallerFailed);
                        return false;
                    }
                }

                return false;
            });
        }
    }
}
