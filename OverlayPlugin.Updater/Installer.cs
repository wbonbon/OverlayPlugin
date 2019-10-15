using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using SharpCompress.Archives.SevenZip;

namespace RainbowMage.OverlayPlugin.Updater
{
    public class Installer
    {
        ProgressDisplay _display;
        string _tempDir = null;
        string _destDir = null;

        public ProgressDisplay Display => _display;

        public Installer(string dest)
        {
            _display = new ProgressDisplay();
            _display.Show();

            _destDir = dest;
            // Make sure our temporary directory is on the same drive as the destination.
            _tempDir = Path.Combine(Path.GetDirectoryName(dest), "OverlayPlugin.tmp");
        }

        public static async Task<bool> Run(string url, string _destDir, bool overwrite = false)
        {
            var inst = new Installer(_destDir);

            // We need to use a Task here since parts of Download() and the other methods are blocking.
            return await Task.Run(async () =>
            {
                var result = false;
                var archivePath = Path.Combine(inst._tempDir, "update.7z");

                if (await inst.Download(url, archivePath) && inst.Extract(archivePath))
                {
                    result = overwrite ? inst.InstallOverwrite() : inst.InstallReplace();
                    inst.Cleanup();

                    if (result)
                    {
                        inst.Display.Close();
                    }
                }

                return result;
            });
        }

        public static async Task<bool> InstallMsvcrt()
        {
            var inst = new Installer(Path.GetTempPath());
            var exePath = Path.Combine(inst._tempDir, "vc_redist.x64.exe");

            return await Task.Run(async () =>
            {
                if (await inst.Download("https://aka.ms/vs/16/release/VC_redist.x64.exe", exePath))
                {
                    inst._display.UpdateStatus(0, "[2/2]: Launching installer...");
                    inst._display.Log("Launching installer...");

                    try
                    {
                        var proc = Process.Start(exePath);
                        proc.WaitForExit();
                        proc.Close();
                    } catch(System.ComponentModel.Win32Exception ex)
                    {
                        inst._display.Log($"Failed to launch installer: {ex.Message}");
                        inst._display.Log("Trying again...");

                        using (var proc = new Process())
                        {
                            proc.StartInfo.FileName = exePath;
                            proc.StartInfo.UseShellExecute = true;
                            proc.Start();
                        }

                        var cancel = inst._display.GetCancelToken();

                        inst._display.Log("Waiting for installer to exit...");
                        while (!File.Exists("C:\\Windows\\system32\\msvcp140.dll") && !cancel.IsCancellationRequested)
                        {
                            Thread.Sleep(500);
                        }

                        // Wait some more just to be sure that the installer is done.
                        Thread.Sleep(1000);
                    }

                    inst.Cleanup();
                    return File.Exists("C:\\Windows\\system32\\msvcp140.dll");
                }

                return false;
            });
        }

        public async Task<bool> Download(string url, string dest)
        {
            try
            {
                if (Directory.Exists(_tempDir))
                {
                    Directory.Delete(_tempDir, true);
                }

                Directory.CreateDirectory(_tempDir);
            } catch (Exception ex)
            {
                _display.Log($"Failed to create or empty the temporary directory \"{_tempDir}\": {ex}");
                return false;
            }

            _display.UpdateStatus(0, "[1/2]: Starting download...");
            _display.Log($"Downloading \"{url}\" into {dest}...");

            var success = false;
            var client = new HttpClient();
            var cancel = _display.GetCancelToken();
            HttpResponseMessage response;

            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            client.DefaultRequestHeaders.Add("User-Agent", "ngld/OverlayPlugin v" + currentVersion.ToString());

            try
            {
                Stream stream = null;
                var buffer = new byte[81290];
                var read = 0;
                var failed = true;
                var retries = 10;

                using (var file = File.OpenWrite(dest))
                {
                    while (retries > 0 && failed)
                    {
                        failed = false;
                        retries--;

                        try
                        {
                            response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancel);
                        }
                        catch (HttpRequestException ex)
                        {
                            _display.Log($"Download failed: {ex.Message}");

                            if (retries > 0)
                            {
                                _display.Log("Retrying in 1 second.");
                                Thread.Sleep(1000);
                            }
                            failed = true;
                            continue;
                        }

                        if (!response.IsSuccessStatusCode)
                        {
                            _display.Log($"Download failed: {response.ReasonPhrase}");

                            if (retries > 0)
                            {
                                _display.Log("Retrying in 1 second.");
                                Thread.Sleep(1000);
                            }
                            failed = true;
                            continue;
                        }

                        var length = response.Content.Headers.ContentLength ?? -1;
                        if (length == -1)
                        {
                            // Retrying wouldn't help here.
                            _display.Log("Download failed! The download has no file size (Content-Length header is missing).");
                            return false;
                        }

                        if (response.StatusCode == HttpStatusCode.PartialContent)
                        {
                            // This is a resumed download.
                            length += file.Position;
                        }
                        else
                        {
                            // Make sure we don't append stuff if the resumption failed and we're receiving the whole file again.
                            file.Seek(0, SeekOrigin.Begin);
                        }

                        stream = await response.Content.ReadAsStreamAsync();

                        try
                        {
                            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                file.Write(buffer, 0, read);

                                _display.UpdateStatus((float)file.Position / length, $"[1/2]: Downloading...");

                                if (cancel.IsCancellationRequested)
                                {
                                    retries = 0;
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _display.Log($"The download was interrupted: {ex.Message}");

                            if (file.Position > 0 && retries > 0)
                            {
                                _display.Log("Resuming download...");
                                failed = true;

                                client.DefaultRequestHeaders.Remove("Range");
                                client.DefaultRequestHeaders.Add("Range", $"bytes={file.Position}-");
                                continue;
                            }
                        }

                        break;
                    }
                }

                if (cancel.IsCancellationRequested || failed)
                {
                    _display.UpdateStatus(0, "Cancelling...");
                    File.Delete(dest);

                    if (failed)
                    {
                        _display.UpdateStatus(0, "Out of retries.");
                        _display.Log("Out of retries.");
                    }
                    else
                    {
                        _display.UpdateStatus(0, "Cancelled!");
                        _display.Log("Aborted by user.");
                    }

                    _display.DisposeCancelSource();
                    return false;
                }

                _display.DisposeCancelSource();
            }
            catch (Exception ex)
            {
                if (cancel.IsCancellationRequested)
                {
                    _display.UpdateStatus(1, "Cancelled!");
                    _display.Log("Aborted by user.");
                    return false;
                }

                _display.Log($"Failed: {ex}");
                return false;
            }
            finally
            {
                _display.DisposeCancelSource();
                client.Dispose();

                if (!success) Cleanup();
            }

            return true;
        }

        public bool Extract(string archivePath)
        {
            var success = false;
            var cancel = _display.GetCancelToken();

            try
            {
                _display.UpdateStatus(0, "[2/2]: Preparing extraction...");
                _display.Log("Opening archive...");

                var contentsPath = Path.Combine(_tempDir, "contents");
                Directory.CreateDirectory(contentsPath);

                using (var archive = SevenZipArchive.Open(archivePath))
                {
                    // Make sure we never divide by zero.
                    var total = 1d;
                    var done = 0d;

                    foreach (var entry in archive.Entries)
                    {
                        total += entry.Size;
                    }

                    using (var reader = archive.ExtractAllEntries())
                    {
                        reader.EntryExtractionProgress += (sender, e) =>
                        {
                            var percent = (float)(done + e.ReaderProgress.BytesTransferred) / total;

                            _display.UpdateStatus(Math.Min(percent, 1), $"[2/2]: {reader.Entry.Key}");
                        };

                        cancel = _display.GetCancelToken();
                        _display.Log("Extracting files...");

                        while (reader.MoveToNextEntry())
                        {
                            if (cancel.IsCancellationRequested)
                            {
                                break;
                            }

                            var outPath = Path.Combine(contentsPath, reader.Entry.Key);

                            if (reader.Entry.IsDirectory)
                            {
                                if (!Directory.Exists(outPath))
                                {
                                    Directory.CreateDirectory(outPath);
                                }
                            }
                            else
                            {
                                using (var writer = File.OpenWrite(outPath))
                                {
                                    reader.WriteEntryTo(writer);
                                }
                            }

                            done += reader.Entry.Size;
                        }
                    }
                }

                if (cancel.IsCancellationRequested)
                {
                    _display.UpdateStatus(1, "Cancelled!");
                    _display.Log("Extraction aborted by user.");
                    return false;
                }

                success = true;
            } catch(Exception ex)
            {
                if (cancel.IsCancellationRequested)
                {
                    _display.UpdateStatus(1, "Cancelled!");
                    _display.Log("Aborted by user.");
                    return false;
                }

                _display.Log($"Failed: {ex}");
                return false;
            }
            finally
            {
                _display.DisposeCancelSource();

                if (!success) Cleanup();
            }

            _display.UpdateStatus(1, "[2/2]: Done.");
            _display.Log("Done.");
            return success;
        }

        public bool InstallReplace()
        {
            try
            {
                string backup = null;
                var parent = Path.GetDirectoryName(_destDir);
                if (!Directory.Exists(parent))
                    Directory.CreateDirectory(parent);

                if (Directory.Exists(_destDir))
                {
                    _display.Log("Backing up old files...");

                    backup = _destDir + ".bak";
                    if (Directory.Exists(backup))
                        Directory.Delete(backup, true);

                    Directory.Move(_destDir, backup);
                }

                try
                {
                    _display.Log("Moving directory...");
                    Directory.Move(Path.Combine(_tempDir, "contents"), _destDir);
                }
                catch (Exception e)
                {
                    _display.Log($"Failed to replace old directory: {e}");
                    _display.Log("Cleaning up...");

                    if (Directory.Exists(_destDir))
                    {
                        Directory.Delete(_destDir, true);
                    }

                    if (backup != null)
                    {
                        _display.Log("Restoring backup...");

                        Directory.Move(backup, _destDir);
                    }

                    _display.Log("Done.");
                    return false;
                }

                if (backup != null)
                {
                    _display.Log("Removing old backup...");
                    Directory.Delete(backup, true);
                }

                return true;
            }
            catch (Exception e)
            {
                _display.Log($"Fatal error: {e}");
                return false;
            }
        }

        public bool InstallOverwrite()
        {
            try
            {
                try
                {
                    _display.Log("Overwriting old files...");

                    var prefix = Path.Combine(_tempDir, "contents");
                    var queue = new List<DirectoryInfo>() { new DirectoryInfo(prefix) };
                    while (queue.Count() > 0)
                    {
                        var info = queue[0];
                        queue.RemoveAt(0);

                        var sub = info.FullName.Substring(prefix.Length).TrimStart('\\', '/');
                        var sub_destDir = Path.Combine(_destDir, sub);
                        if (!Directory.Exists(sub_destDir))
                        {
                            Directory.CreateDirectory(sub_destDir);
                        }

                        foreach (var item in info.EnumerateDirectories())
                        {
                            queue.Add(item);
                        }

                        foreach (var item in info.EnumerateFiles())
                        {
                            File.Delete(Path.Combine(sub_destDir, item.Name));
                            File.Move(item.FullName, Path.Combine(sub_destDir, item.Name));
                        }
                    }
                }
                catch (Exception e)
                {
                    _display.Log($"Failed to overwrite old files: {e}");
                    _display.Log("WARNING: The plugin might be in an unusable state!!");
                    _display.Log("Run the update check again or reinstall the plugin, otherwise it might not survive the next ACT restart.");
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                _display.Log($"Fatal error: {e}");
                return false;
            }
        }

        public void Cleanup()
        {
            if (Directory.Exists(_tempDir))
            {
                _display.Log("Deleting temporary files...");

                try
                {
                    Directory.Delete(_tempDir, true);
                    _display.Log("Done.");
                }
                catch (Exception ex)
                {
                    _display.Log($"Failed to delete {_tempDir}: {ex}");
                }
            }
        }
    }
}
