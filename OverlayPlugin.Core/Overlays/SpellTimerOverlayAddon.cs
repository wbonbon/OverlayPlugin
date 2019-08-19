using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RainbowMage.OverlayPlugin.Overlays
{
    class SpellTimerOverlayAddon : IOverlayAddonV2
    {
        public string Name
        {
            get { return "Spell Timer"; }
        }

        public string Description
        {
            get { return ""; }
        }

        public Type OverlayType
        {
            get { return typeof(SpellTimerOverlay); }
        }

        public Type OverlayConfigType
        {
            get { return typeof(SpellTimerOverlayConfig); }
        }

        public Type OverlayConfigControlType
        {
            get { return typeof(SpellTimerConfigPanel); }
        }

        public Type EventSourceType => null;

        public IOverlay CreateOverlayInstance(IOverlayConfig config)
        {
            return new SpellTimerOverlay((SpellTimerOverlayConfig)config);
        }

        public IOverlayConfig CreateOverlayConfigInstance(string name)
        {
            return new SpellTimerOverlayConfig(name);
        }

        public System.Windows.Forms.Control CreateOverlayConfigControlInstance(IOverlay overlay)
        {
            return new SpellTimerConfigPanel((SpellTimerOverlay)overlay);
        }

        public void Dispose()
        {

        }

        public IEventSourceConfig CreateEventSourceConfigInstance()
        {
            return null;
        }

        public IEventSource CreateEventSourceInstance(IEventSourceConfig config)
        {
            return null;
        }

        public Control CreateEventSourceControlInstance(IEventSource source)
        {
            return null;
        }
    }
}
