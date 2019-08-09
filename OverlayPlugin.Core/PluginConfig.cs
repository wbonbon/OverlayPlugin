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

namespace RainbowMage.OverlayPlugin
{
    [Serializable]
    public class PluginConfig : IPluginConfig
    {
        /// <summary>
        /// オーバーレイ設定のリスト。
        /// </summary>
        [XmlElement("Overlays")]
        public OverlayConfigList<IOverlayConfig> Overlays { get; set; }

        [XmlElement("EventSources")]
        public OverlayConfigList<IEventSourceConfig> EventSources { get; set; }

        /// <summary>
        /// 設定タブのログにおいて、常に最新のログ行を表示するかどうかを取得または設定します。
        /// </summary>
        [XmlElement("FollowLatestLog")]
        public bool FollowLatestLog { get; set; }

        [XmlElement("HideOverlaysWhenNotActive")]
        public bool HideOverlaysWhenNotActive { get; set; }

        [XmlElement("WSServerIP")]
        public string WSServerIP { get; set; }

        [XmlElement("WSServerPort")]
        public int WSServerPort { get; set; }

        [XmlElement("WSServerSSL")]
        public bool WSServerSSL { get; set; }

        [XmlElement("WSServerRunning")]
        public bool WSServerRunning { get; set; }

        /// <summary>
        /// 設定ファイルを生成したプラグインのバージョンを取得または設定します。
        /// 設定が新規に作成された場合、またはバージョン0.3未満では null が設定されます。
        /// </summary>
        [XmlIgnore]
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
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public string VersionString { get; set; }

        /// <summary>
        /// 設定が新規に作成されたことを示すフラグを取得または設定します。
        /// </summary>
        [XmlIgnore]
        public bool IsFirstLaunch { get; set; }

        internal const string DefaultMiniParseOverlayName = "Mini Parse";
        internal const string DefaultSpellTimerOverlayName = "Spell Timer";

        public PluginConfig()
        {
            this.Overlays = new OverlayConfigList<IOverlayConfig>();
            this.EventSources = new OverlayConfigList<IEventSourceConfig>();

            this.FollowLatestLog = false;
            this.HideOverlaysWhenNotActive = false;
            this.IsFirstLaunch = true;

            this.WSServerIP = "127.0.0.1";
            this.WSServerPort = 10501;
            this.WSServerRunning = false;
            this.WSServerSSL = false;
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

                if (result.Version == null)
                {
                    
                }


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

            var spellTimerOverlayConfig = new SpellTimerOverlayConfig(DefaultSpellTimerOverlayName);
            spellTimerOverlayConfig.Position = new Point(20, 520);
            spellTimerOverlayConfig.Size = new Size(200, 300);
            spellTimerOverlayConfig.IsVisible = true;
            spellTimerOverlayConfig.MaxFrameRate = 5;
            spellTimerOverlayConfig.Url = new Uri(Path.Combine(pluginDirectory, "resources", "spelltimer.html")).ToString();

            this.Overlays = new OverlayConfigList<IOverlayConfig>();
            this.Overlays.Add(miniparseOverlayConfig);
            this.Overlays.Add(spellTimerOverlayConfig);
        }

        private void UpdateFromVersion0_3_4_0OrBelow()
        {
            // TODO: Convert SortKey and SortType from OverlayConfig to EventSourceConfig
        }
    }
}
