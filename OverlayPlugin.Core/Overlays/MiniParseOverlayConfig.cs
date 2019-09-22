using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RainbowMage.OverlayPlugin.Overlays
{
    public class MiniParseOverlayConfig : OverlayConfigBase
    {
        public override Type OverlayType => typeof(MiniParseOverlay);

        private bool actwsCompatibility;
        public bool ActwsCompatibility
        {
            get
            {
                return this.actwsCompatibility;
            }
            set
            {
                this.actwsCompatibility = value;
                ActwsCompatibilityChanged?.Invoke(this, new CompatbilityChangedArgs(value));
            }
        }
        public event EventHandler<CompatbilityChangedArgs> ActwsCompatibilityChanged;


        public MiniParseOverlayConfig(string name) : base(name)
        {

        }

        public MiniParseOverlayConfig() : base(null) { }

        public class CompatbilityChangedArgs : EventArgs
        {
            public bool Compatibility { get; private set; }

            public CompatbilityChangedArgs(bool c)
            {
                Compatibility = c;
            }
        }
    }
}
