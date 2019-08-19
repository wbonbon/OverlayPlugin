using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RainbowMage.OverlayPlugin.Overlays
{
    public class LabelOverlayAddon : IOverlayAddonV2
    {
        public string Name
        {
            get { return "Label"; }
        }

        public string Description
        {
            get { return "Show simple label overlay"; }
        }

        public Type OverlayType
        {
            get { return typeof(LabelOverlay); }
        }

        public Type OverlayConfigType
        {
            get { return typeof(LabelOverlayConfig); }
        }

        public Type OverlayConfigControlType
        {
            get { return typeof(LabelOverlayConfigPanel); }
        }

        public Type EventSourceType => null;

        public IOverlay CreateOverlayInstance(IOverlayConfig config)
        {
            return new LabelOverlay((LabelOverlayConfig)config);
        }

        public IOverlayConfig CreateOverlayConfigInstance(string name)
        {
            return new LabelOverlayConfig(name);
        }

        public System.Windows.Forms.Control CreateOverlayConfigControlInstance(IOverlay overlay)
        {
            return new LabelOverlayConfigPanel((LabelOverlay)overlay);
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
