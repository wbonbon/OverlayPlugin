using RainbowMage.OverlayPlugin.Overlays;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RainbowMage.OverlayPlugin
{
    [Serializable]
    public class PluginConfig : IPluginConfig
    {
        /// <summary>
        /// 設定タブのログにおいて、常に最新のログ行を表示するかどうかを取得または設定します。
        /// </summary>
        [XmlElement("FollowLatestLog")]
        public bool FollowLatestLog { get; set; }

        [XmlElement("HideOverlaysWhenNotActive")]
        public bool HideOverlaysWhenNotActive { get; set; }

        public string WSServerIP { get; set; }

        public int WSServerPort { get; set; }

        public bool WSServerSSL { get; set; }

        public bool WSServerRunning { get; set; }

        /// <summary>
        /// 設定ファイルを生成したプラグインのバージョンを取得または設定します。
        /// 設定が新規に作成された場合、またはバージョン0.3未満では null が設定されます。
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public Version Version 
        {
            get
            {
                if (string.IsNullOrEmpty(this.VersionString))
                {
                    return null;
                }
                else
                {
                    return new Version(this.VersionString);
                }
            }
            set
            {
                if (value != null)
                {
                    this.VersionString = value.ToString();
                }
                else
                {
                    this.VersionString = null;
                }
            }
        }

        [XmlElement("Version")]
        [JsonProperty("Version")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public string VersionString { get; set; }

        /// <summary>
        /// 設定が新規に作成されたことを示すフラグを取得または設定します。
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public bool IsFirstLaunch { get; set; }

        /// <summary>
        /// オーバーレイ設定のリスト。
        /// </summary>
        [XmlElement("Overlays")]
        public OverlayConfigList<IOverlayConfig> Overlays { get; set; }

        [XmlIgnore]
        public Dictionary<string, object> EventSourceConfigs { get; set; }

        internal const string DefaultMiniParseOverlayName = "Mini Parse";

        public PluginConfig()
        {
            this.Overlays = new OverlayConfigList<IOverlayConfig>();
            this.EventSourceConfigs = new Dictionary<string, object>();

            this.FollowLatestLog = false;
            this.HideOverlaysWhenNotActive = false;
            this.IsFirstLaunch = true;
        }

        /// <summary>
        /// 指定したファイルパスに設定を保存します。
        /// </summary>
        /// <param name="path"></param>
        public void SaveXml(string path)
        {
            this.Version = typeof(PluginMain).Assembly.GetName().Version;

            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PluginConfig));
                serializer.Serialize(stream, this);
            }
        }

        public void SaveJson(string path)
        {
            using (var stream = new StreamWriter(path))
            {
                var serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.TypeNameHandling = TypeNameHandling.Auto;
                serializer.Serialize(stream, this);
            }
        }

        public static PluginConfig LoadJson(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            using (var stream = new StreamReader(path))
            {
                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(stream);
                serializer.TypeNameHandling = TypeNameHandling.Auto;

                var result = serializer.Deserialize<PluginConfig>(reader);
                result.IsFirstLaunch = false;
                return result;
            }
        }

        /// <summary>
        /// 指定したファイルパスから設定を読み込みます。
        /// </summary>
        /// <param name="pluginDirectory"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static PluginConfig LoadXml(string pluginDirectory, string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Specified file is not exists.", path);
            }

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PluginConfig));

                var result = (PluginConfig)serializer.Deserialize(stream);
                result.IsFirstLaunch = false;
                return result;
            }
        }

        /// <summary>
        /// デフォルトのオーバーレイを作成します。
        /// </summary>
        /// <param name="pluginDirectory"></param>
        public void SetDefaultOverlayConfigs(string pluginDirectory)
        {
            var miniparseOverlayConfig = new MiniParseOverlayConfig(DefaultMiniParseOverlayName);
            miniparseOverlayConfig.Position = new Point(20, 20);
            miniparseOverlayConfig.Size = new Size(500, 300);
            miniparseOverlayConfig.Url = new Uri(Path.Combine(pluginDirectory, "resources", "miniparse.html")).ToString();

            this.Overlays = new OverlayConfigList<IOverlayConfig>();
            this.Overlays.Add(miniparseOverlayConfig);

            this.WSServerIP = "127.0.0.1";
            this.WSServerPort = 10501;
            this.WSServerRunning = false;
            this.WSServerSSL = false;
        }

        private void UpdateFromVersion0_3_4_0OrBelow()
        {
            // TODO: Convert SortKey and SortType from OverlayConfig to EventSourceConfig
        }
    }
}
