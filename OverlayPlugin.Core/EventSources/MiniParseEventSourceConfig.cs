using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RainbowMage.OverlayPlugin.Overlays
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

        public static MiniParseEventSourceConfig LoadConfig()
        {
            var allConfigs = Registry.Resolve<IPluginConfig>().EventSourceConfigs;

            if (!allConfigs.ContainsKey("MiniParse"))
            {
                allConfigs["MiniParse"] = new MiniParseEventSourceConfig();
            }

            return (MiniParseEventSourceConfig) allConfigs["MiniParse"];
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
