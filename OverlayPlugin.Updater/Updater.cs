using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows.Forms;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace RainbowMage.OverlayPlugin.Updater
{
    public class Updater
    {
        const string REL_URL = "https://api.github.com/repos/ngld/OverlayPlugin/releases";
        const string DL = "https://github.com/ngld/OverlayPlugin/releases/download/v{VERSION}/OverlayPlugin-{VERSION}.7z";

        public static async Task<(bool, Version, string)> CheckForUpdate()
        {
            var currentVersion = Version.Parse("0.6.0"); // Assembly.GetExecutingAssembly().GetName().Version;
            Version remoteVersion;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "ngld/OverlayPlugin v" + currentVersion.ToString());

            string response;
            try
            {
                response = await client.GetStringAsync(REL_URL);
            } catch (HttpRequestException ex)
            {
                MessageBox.Show("Failed to check for updates:\n" + ex.ToString(), "OverlayPlugin Update Check", MessageBoxButtons.OK, MessageBoxIcon.Error);
                client.Dispose();
                return (false, null, "");
            }

            client.Dispose();

            var releaseNotes = "";
            try
            {
                // JObject doesn't accept arrays so we have to package the response in a JSON object.
                var tmp = JObject.Parse("{\"content\":" + response + "}");
                remoteVersion = Version.Parse(tmp["content"][0]["tag_name"].ToString().Substring(1));

                foreach (var rel in tmp["content"])
                {
                    var version = Version.Parse(rel["tag_name"].ToString().Substring(1));
                    if (version.CompareTo(currentVersion) < 1) break;

                    releaseNotes += "---\n\n# " + rel["name"].ToString() + "\n\n" + rel["body"].ToString() + "\n\n";
                }

                if (releaseNotes.Length > 5)
                {
                    releaseNotes = releaseNotes.Substring(5);
                }
            } catch(Exception ex)
            {
                MessageBox.Show("Failed to parse version:\n" + ex.ToString(), "OverlayPlugin Update Check", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false, null, null);
            }

            return (remoteVersion.CompareTo(currentVersion) > 0, remoteVersion, releaseNotes);
        }

        public static async Task<bool> InstallUpdate(Version version, string pluginDirectory)
        {
            var url = DL.Replace("{VERSION}", version.ToString());

            var result = await Installer.Run(url, pluginDirectory, true);
            if (!result)
            {
                var response = MessageBox.Show(
                    "Failed to update the plugin. It might not load the next time you start ACT. Retry?",
                    "OverlayPlugin Error",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (response == DialogResult.Yes)
                {
                    return await InstallUpdate(version, pluginDirectory);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                MessageBox.Show(
                    "The update was successful. Please restart ACT to load the new plugin version.",
                    "OverlayPlugin Update",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                return true;
            }
        }

        public static async void PerformUpdateIfNecessary(string pluginDirectory, bool alwaysTalk = false)
        {
            var (newVersion, remoteVersion, releaseNotes) = await CheckForUpdate();

            if (newVersion)
            {
                var dialog = new UpdateQuestionForm(releaseNotes);
                var result = dialog.ShowDialog();
                dialog.Dispose();

                if (result == DialogResult.Yes)
                {
                    await InstallUpdate(remoteVersion, pluginDirectory);
                }
            } else if (alwaysTalk)
            {
                MessageBox.Show("You are already on the latest version.", "OverlayPlugin", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
