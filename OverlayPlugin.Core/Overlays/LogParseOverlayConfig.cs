using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RainbowMage.OverlayPlugin.Overlays
{
    [Serializable]
    public class LogParseOverlayConfig : OverlayConfigBase
    {
        public event EventHandler<IncludeChangedEventArgs> IncludeCombatLogChanged;
        public event EventHandler<IncludeChangedEventArgs> IncludeChatChanged;
        public event EventHandler<IncludeChangedEventArgs> IncludeEchoChanged;
        /*
        public event EventHandler<SortKeyChangedEventArgs> SortKeyChanged;
        public event EventHandler<SortTypeChangedEventArgs> SortTypeChanged;

        private string sortKey;
        [XmlElement("SortKey")]
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
                    if (SortKeyChanged != null)
                    {
                        SortKeyChanged(this, new SortKeyChangedEventArgs(this.sortKey));
                    }
                }
            }
        }

        private LogParseSortType sortType;
        [XmlElement("SortType")]
        public LogParseSortType SortType
        {
            get
            {
                return this.sortType;
            }
            set
            {
                if (this.sortType != value)
                {
                    this.sortType = value;
                    if (SortTypeChanged != null)
                    {
                        SortTypeChanged(this, new SortTypeChangedEventArgs(this.sortType));
                    }
                }
            }
        }
        */
        private bool includeChat;
        [XmlElement("IncludeChat")]
        public bool IncludeChat
        {
            get
            {
                return this.includeChat;
            }
            set
            {
                if (this.includeChat != value)
                {
                    this.includeChat = value;
                    if (IncludeChatChanged != null)
                    {
                        IncludeChatChanged(this, new IncludeChangedEventArgs(this.includeChat));
                    }
                }
            }
        }

        private bool includeEcho;
        [XmlElement("IncludeEcho")]
        public bool IncludeEcho
        {
            get
            {
                return this.includeEcho;
            }
            set
            {
                if (this.includeEcho != value)
                {
                    this.includeEcho = value;
                    if (IncludeEchoChanged != null)
                    {
                        IncludeEchoChanged(this, new IncludeChangedEventArgs(this.includeEcho));
                    }
                }
            }
        }

        private bool includeCombatLog;
        [XmlElement("IncludeCombatLog")]
        public bool IncludeCombatLog
        {
            get
            {
                return this.includeCombatLog;
            }
            set
            {
                if (this.includeCombatLog != value)
                {
                    this.includeCombatLog = value;
                    if (IncludeCombatLogChanged != null)
                    {
                        IncludeCombatLogChanged(this, new IncludeChangedEventArgs(this.includeCombatLog));
                    }
                }
            }
        }
        public LogParseOverlayConfig(string name) : base(name)
        {
            // this.sortKey = "encdps";
            // this.sortType = LogParseSortType.NumericDescending;
            this.IncludeChat = true;
            this.IncludeEcho = true;
            this.IncludeCombatLog = true;
        }

        // XmlSerializer用
        private LogParseOverlayConfig() : base(null)
        {

        }

        public override Type OverlayType
        {
            get { return typeof(LogParseOverlay); }
        }
    }
    /*
    public enum LogParseSortType
    {
        None,
        StringAscending,
        StringDescending,
        NumericAscending,
        NumericDescending
    }
    */

    public class IncludeChangedEventArgs
    {
        public bool Include { get; private set; }
        public IncludeChangedEventArgs(bool include)
        {
            this.Include = include;
        }
    }
}
