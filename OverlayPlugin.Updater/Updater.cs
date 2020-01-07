using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows.Forms;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Advanced_Combat_Tracker;
using Markdig;

namespace RainbowMage.OverlayPlugin.Updater
{
    public class Updater
    {
        const string CHECK_URL = "https://api.github.com/repos/{REPO}/releases/latest";
        const string ALL_RELEASES_URL = "https://api.github.com/repos/{REPO}/releases";

        public static Task<(bool, Version, string, string)> CheckForGitHubUpdate(UpdaterOptions options)
        {
            return Task.Run(() =>
            {
                Version remoteVersion;
                string response;
                try
                {
                    response = CurlWrapper.Get(CHECK_URL.Replace("{REPO}", options.repo));
                }
                catch (CurlException ex)
                {
                    MessageBox.Show(
                        string.Format(Resources.UpdateCheckException, ex.ToString()),
                        string.Format(Resources.UpdateCheckTitle, options.project),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return (false, null, "", "");
                }

                var releaseNotes = "";
                var downloadUrl = "";
                try
                {
                    var tmp = JObject.Parse(response);
                    remoteVersion = Version.Parse(tmp["tag_name"].ToString().Substring(1));
                    if (remoteVersion <= options.currentVersion)
                    {
                        // Exit early if no new version is available.
                        return (false, remoteVersion, "", "");
                    }

                    response = CurlWrapper.Get(ALL_RELEASES_URL.Replace("{REPO}", options.repo));

                    // JObject doesn't accept arrays so we have to package the response in a JSON object.
                    tmp = JObject.Parse("{\"content\":" + response + "}");

                    downloadUrl = options.downloadUrl.Replace("{REPO}", options.repo).Replace("{VERSION}", remoteVersion.ToString());

                    foreach (var rel in tmp["content"])
                    {
                        var version = Version.Parse(rel["tag_name"].ToString().Substring(1));
                        if (version < options.currentVersion) break;

                        releaseNotes += "---\n\n# " + rel["name"].ToString() + "\n\n" + rel["body"].ToString() + "\n\n";
                    }

                    if (releaseNotes.Length > 5)
                    {
                        releaseNotes = releaseNotes.Substring(5);
                    }
                }
                catch (Exception ex)
                {
                    ActGlobals.oFormActMain.Invoke((Action)(() =>
                    {
                        MessageBox.Show(
                            string.Format(Resources.UpdateParseVersionError, ex.ToString()),
                            string.Format(Resources.UpdateTitle, options.project),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }));
                    return (false, null, "", "");
                }

                try
                {
                    releaseNotes = Regex.Replace(releaseNotes, @"<!-- TRAILER BEGIN -->(?:[^<]|<(?!!-- TRAILER END -->))+<!-- TRAILER END -->", "");
                }
                catch (Exception ex)
                {
                    Registry.Resolve<ILogger>().Log(LogLevel.Error, $"Failed to remove trailers from release notes: {ex}");
                }

                releaseNotes = Markdown.ToHtml(releaseNotes);

                return (remoteVersion.CompareTo(options.currentVersion) > 0, remoteVersion, releaseNotes, downloadUrl);
            });
        }

        public static Task<(bool, Version, string, string)> CheckForManifestUpdate(UpdaterOptions options)
        {
            return Task.Run(() =>
            {
                Version remoteVersion;
                string response;
                try
                {
                    response = CurlWrapper.Get(options.manifestUrl);
                }
                catch (CurlException ex)
                {
                    MessageBox.Show(
                        string.Format(Resources.UpdateCheckException, ex.ToString()),
                        string.Format(Resources.UpdateCheckTitle, options.project),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return (false, null, "", "");
                }

                var releaseNotes = "";
                var downloadUrl = "";
                try
                {
                    var tmp = JObject.Parse(response);
                    remoteVersion = Version.Parse(tmp["version"].ToString());
                    if (remoteVersion.CompareTo(options.currentVersion) <= 0)
                    {
                        // Exit early if no new version is available.
                        return (false, remoteVersion, "", "");
                    }

                    response = CurlWrapper.Get(options.notesUrl);

                    // JObject doesn't accept arrays so we have to package the response in a JSON object.
                    tmp = JObject.Parse("{\"content\":" + response + "}");
                    downloadUrl = tmp[0]["download"].ToString();

                    foreach (var rel in tmp["content"])
                    {
                        var version = Version.Parse(rel["version"].ToString());
                        if (version.CompareTo(options.currentVersion) <= 0) break;

                        releaseNotes += rel["notes"];
                    }
                }
                catch (Exception ex)
                {
                    ActGlobals.oFormActMain.Invoke((Action)(() =>
                    {
                        MessageBox.Show(
                            string.Format(Resources.UpdateParseVersionError, ex.ToString()),
                            string.Format(Resources.UpdateTitle, options.project),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }));
                    return (false, null, "", "");
                }

                return (remoteVersion.CompareTo(options.currentVersion) > 0, remoteVersion, releaseNotes, downloadUrl);
            });
        }

        public static async Task<bool> InstallUpdate(string url, UpdaterOptions options)
        {
            var result = false;

            while (!result) {
                result = await Installer.Run(url, options.pluginDirectory, options.project + ".tmp", options.strippedDirs, true);

                if (!result)
                {
                    var response = MessageBox.Show(
                        Resources.UpdateFailedError,
                        string.Format(Resources.ErrorTitle, options.project),
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (response != DialogResult.Yes)
                    {
                        return false;
                    }
                }
            }

            MessageBox.Show(
                Resources.UpdateSuccess,
                string.Format(Resources.UpdateTitle, options.project),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            return true;
        }

        public static async Task RunAutoUpdater(UpdaterOptions options, bool manualCheck = false)
        {
            // Only check once per day.
            if (!manualCheck && options.lastCheck != null && (DateTime.Now - options.lastCheck).TotalDays < 1)
            {
                return;
            }

            bool newVersion;
            Version remoteVersion;
            string releaseNotes;
            string downloadUrl;

            if (options.repo != null)
            {
                (newVersion, remoteVersion, releaseNotes, downloadUrl) = await CheckForGitHubUpdate(options);
            } else
            {
                (newVersion, remoteVersion, releaseNotes, downloadUrl) = await CheckForManifestUpdate(options);
            }

            if (remoteVersion != null)
            {
                options.lastCheck = DateTime.Now;
            }

            if (newVersion)
            {
                // Make sure we open the UpdateQuestionForm on a UI thread.
                ActGlobals.oFormActMain.Invoke((Action)(async () =>
                {
                    var dialog = new UpdateQuestionForm(options, releaseNotes);
                    var result = dialog.ShowDialog();
                    dialog.Dispose();

                    if (result == DialogResult.Yes)
                    {
                        await InstallUpdate(downloadUrl, options);
                    }
                }));
            } else if (manualCheck && remoteVersion != null)
            {
                ActGlobals.oFormActMain.Invoke((Action)(() =>
                {
                    MessageBox.Show(
                        Resources.UpdateAlreadyLatest,
                        string.Format(Resources.UpdateTitle, options.project),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }));
            }
        }

        public static async void PerformUpdateIfNecessary(string pluginDirectory, bool manualCheck = false)
        {
            var config = Registry.Resolve<IPluginConfig>();
            var options = new UpdaterOptions
            {
                project = "OverlayPlugin",
                pluginDirectory = pluginDirectory,
                lastCheck = config.LastUpdateCheck,
                currentVersion = Assembly.GetExecutingAssembly().GetName().Version,
                repo = "ngld/OverlayPlugin",
                downloadUrl = "https://github.com/{REPO}/releases/download/v{VERSION}/OverlayPlugin-{VERSION}.7z",
            };

            await RunAutoUpdater(options, manualCheck);
            config.LastUpdateCheck = options.lastCheck;
        }
    }

    public class UpdaterOptions {
        public string project;
        public string pluginDirectory;
        public DateTime lastCheck;
        public Version currentVersion;
        public Version remoteVersion;
        public int strippedDirs;

        // GitHub parameters
        public string repo;
        public string downloadUrl;

        // Manifest parameters
        public string manifestUrl;
        public string notesUrl;
    }
}
