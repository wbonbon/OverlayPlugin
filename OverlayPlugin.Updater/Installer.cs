using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System.IO;
using System.Reflection;
using SharpCompress.Archives.SevenZip;

namespace RainbowMage.OverlayPlugin.Updater
{
    public class Installer
    {
        ProgressDisplay _display;
        string _tempDir = null;

        public ProgressDisplay Display => _display;

        public Installer()
        {
            _display = new ProgressDisplay();
            _display.Show();
        }

        public static async Task<bool> Run(string url, string dest, bool overwrite = false)
        {
            var inst = new Installer();

            // We need to use a Task here since parts of Download() and the other methods are blocking.
            return await Task.Run(async () =>
            {
                var result = false;

                if (await inst.Download(url))
                {
                    result = overwrite ? inst.InstallOverwrite(dest) : inst.InstallReplace(dest);
                    inst.Cleanup();

                    if (result)
                    {
                        inst.Display.Close();
                    }
                }

                return result;
            });
        }

        public async Task<bool> Download(string url)
        {
            _tempDir = Path.GetTempFileName();
            File.Delete(_tempDir);
            Directory.CreateDirectory(_tempDir);

            var archivePath = Path.Combine(_tempDir, "update.7z");

            _display.UpdateStatus(0, "[1/2]: Starting download...");
            _display.Log($"Downloading \"{url}\" into {archivePath}...");

            var success = false;
            var client = new HttpClient();
            var cancel = _display.GetCancelToken();
            HttpResponseMessage response;

            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            client.DefaultRequestHeaders.Add("User-Agent", "ngld/OverlayPlugin v" + currentVersion.ToString());

            try {
                try
                {
                    response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancel);
                } catch(HttpRequestException ex)
                {
                    _display.Log($"Download failed: {ex}");
                    return false;
                }

                if (!response.IsSuccessStatusCode)
                {
                    _display.Log($"Download failed: {response.ReasonPhrase}");
                    return false;
                }

                var length = response.Content.Headers.ContentLength ?? -1;
                if (length == -1)
                {
                    _display.Log("Download failed! The download has no file size (Content-Length header is missing).");
                    return false;
                }

                var stream = await response.Content.ReadAsStreamAsync();
                var buffer = new byte[81290];
                var read = 0;
                using (var file = File.OpenWrite(archivePath))
                {
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        file.Write(buffer, 0, read);

                        _display.UpdateStatus((float)file.Position / length, $"[1/2]: Downloading...");

                        if (cancel.IsCancellationRequested) break;
                    }
                }

                if (cancel.IsCancellationRequested)
                {
                    _display.UpdateStatus(0, "Cancelling...");
                    File.Delete(archivePath);

                    _display.UpdateStatus(0, "Cancelled!");
                    _display.Log("Aborted by user.");
                    return false;
                }

                _display.DisposeCancelSource();

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
                client.Dispose();

                if (!success) Cleanup();
            }

            _display.UpdateStatus(1, "[2/2]: Done.");
            _display.Log("Done.");
            return success;
        }

        public bool InstallReplace(string dest)
        {
            try
            {
                string backup = null;
                var parent = Path.GetDirectoryName(dest);
                if (!Directory.Exists(parent))
                    Directory.CreateDirectory(parent);

                if (Directory.Exists(dest))
                {
                    _display.Log("Backing up old files...");

                    backup = dest + ".bak";
                    if (Directory.Exists(backup))
                        Directory.Delete(backup, true);

                    Directory.Move(dest, backup);
                }

                try
                {
                    _display.Log("Moving directory...");
                    Directory.Move(Path.Combine(_tempDir, "contents"), dest);
                }
                catch (Exception e)
                {
                    _display.Log($"Failed to replace old directory: {e}");
                    _display.Log("Cleaning up...");
                    Directory.Delete(dest, true);

                    if (backup != null)
                    {
                        _display.Log("Restoring backup...");

                        Directory.Move(backup, dest);
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

        public bool InstallOverwrite(string dest)
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
                        var subDest = Path.Combine(dest, sub);
                        if (!Directory.Exists(subDest))
                        {
                            Directory.CreateDirectory(subDest);
                        }

                        foreach (var item in info.EnumerateDirectories())
                        {
                            queue.Add(item);
                        }

                        foreach (var item in info.EnumerateFiles())
                        {
                            File.Move(item.FullName, Path.Combine(subDest, item.Name));
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
