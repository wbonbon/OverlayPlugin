using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainbowMage.OverlayPlugin.Overlays
{
    class MiniParseOverlayAddon : IOverlayAddonV2
    {
        public string Name
        {
            get { return "Mini Parse"; }
        }

        public string Description
        {
            get { return ""; }
        }

        public Type OverlayType
        {
            get { return typeof(MiniParseOverlay); }
        }

        public Type EventSourceType
        {
            get { return typeof(MiniParseEventSource); }
        }

        public Type OverlayConfigType
        {
            get { return typeof(MiniParseOverlayConfig); }
        }

        public Type OverlayConfigControlType
        {
            get { return typeof(MiniParseConfigPanel); }
        }

        public IOverlay CreateOverlayInstance(IOverlayConfig config)
        {
            return new MiniParseOverlay((MiniParseOverlayConfig) config);
        }

        public IOverlayConfig CreateOverlayConfigInstance(string name)
        {
            return new MiniParseOverlayConfig(name);
        }

        public System.Windows.Forms.Control CreateOverlayConfigControlInstance(IOverlay overlay)
        {
            return new MiniParseConfigPanel((MiniParseOverlay)overlay);
        }

        public IEventSourceConfig CreateEventSourceConfigInstance()
        {
            return new MiniParseEventSourceConfig();
        }

        public IEventSource CreateEventSourceInstance(IEventSourceConfig config)
        {
            return new MiniParseEventSource((MiniParseEventSourceConfig)config);
        }

        public System.Windows.Forms.Control CreateEventSourceControlInstance(IEventSource source)
        {
            return new MiniParseEventSourceConfigPanel((MiniParseEventSource) source);
        }
    
        public void Dispose()
        {
            
        }
    }
}
