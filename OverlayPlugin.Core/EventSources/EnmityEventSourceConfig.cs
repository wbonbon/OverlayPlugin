using System;
using Newtonsoft.Json.Linq;

namespace RainbowMage.OverlayPlugin.EventSources
{
    [Serializable]
    public class EnmityEventSourceConfig
    {
        public event EventHandler ScanIntervalChanged;

        private static string configName = "Enmity";
        private int scanInterval = 100;

        public int ScanInterval
        {
            get
            {
                return this.scanInterval;
            }
            set
            {
                if (this.scanInterval != value)
                {
                    this.scanInterval = value;
                    ScanIntervalChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        public static EnmityEventSourceConfig LoadConfig(IPluginConfig Config)
        {
            var result = new EnmityEventSourceConfig();
            if (!Config.EventSourceConfigs.ContainsKey(configName))
                return result;

            var obj = Config.EventSourceConfigs[configName];

            if (obj.TryGetValue("ScanInterval", out JToken value))
            {
                result.scanInterval = value.ToObject<int>();
            }

            return result;
        }

        public void SaveConfig(IPluginConfig Config)
        {
            Config.EventSourceConfigs[configName] = JObject.FromObject(this);
        }
  }
}
