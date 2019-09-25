using System;
using Newtonsoft.Json.Linq;

namespace RainbowMage.OverlayPlugin.EventSources
{
    [Serializable]
    public class MiniParseEventSourceConfig
    {
        public event EventHandler UpdateIntervalChanged;
        public event EventHandler SortKeyChanged;
        public event EventHandler SortDescChanged;

        private int updateInterval;
        public int UpdateInterval {
            get
            {
                return this.updateInterval;
            }
            set
            {
                if (this.updateInterval != value)
                {
                    this.updateInterval = value;
                    UpdateIntervalChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        private string sortKey;
        public string SortKey
        {
            get
            {
                return this.sortKey;
            }
            set
            {
                if (this.sortKey != value)
                {
                    this.sortKey = value;
                    SortKeyChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        private bool sortDesc;
        public bool SortDesc
        {
            get
            {
                return this.sortDesc;
            }
            set
            {
                if (this.sortDesc != value)
                {
                    this.sortDesc = value;
                    SortDescChanged?.Invoke(this, new EventArgs());
                }
            }
        }


        public MiniParseEventSourceConfig()
        {
            this.updateInterval = 1;
            this.sortKey = null;
            this.sortDesc = true;
        }

        public static MiniParseEventSourceConfig LoadConfig(IPluginConfig Config)
        {
            var result = new MiniParseEventSourceConfig();

            if (Config.EventSourceConfigs.ContainsKey("MiniParse"))
            {
                var obj = Config.EventSourceConfigs["MiniParse"];
                
                if (obj.TryGetValue("UpdateInterval", out JToken value))
                {
                    result.updateInterval = value.ToObject<int>();
                }

                if (obj.TryGetValue("SortKey", out value))
                {
                    result.sortKey = value.ToString();
                }

                if (obj.TryGetValue("SortDesc", out value))
                {
                    result.sortDesc = value.ToObject<bool>();
                }
            }

            return result;
        }

        public void SaveConfig(IPluginConfig Config)
        {
            Config.EventSourceConfigs["MiniParse"] = JObject.FromObject(this);
        }
    }

    public enum MiniParseSortType
    {
        None,
        StringAscending,
        StringDescending,
        NumericAscending,
        NumericDescending
    }
}
