using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RainbowMage.OverlayPlugin.Overlays
{
    public class MiniParseOverlayConfig : OverlayConfigBase
    {
        public override Type OverlayType => typeof(MiniParseOverlay);

        private string compatibility;
        [XmlElement("Compatibility")]
        public string Compatibility
        {
            get
            {
                return this.compatibility ?? "legacy";
            }
            set
            {
                this.compatibility = value;
                CompatibilityChanged?.Invoke(this, new CompatbilityChangedArgs(value));
            }
        }

        public event EventHandler<CompatbilityChangedArgs> CompatibilityChanged;


        public MiniParseOverlayConfig(string name) : base(name)
        {

        }

        public MiniParseOverlayConfig() : base(null) { }

        public class CompatbilityChangedArgs : EventArgs
        {
            public string Compatibility { get; private set; }

            public CompatbilityChangedArgs(string c)
            {
                Compatibility = c;
            }
        }
    }
}
