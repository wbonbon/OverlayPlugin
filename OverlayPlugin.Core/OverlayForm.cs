using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainbowMage.OverlayPlugin
{
    // Stub to fix old addons that still import OverlayForm from here.
    public class OverlayForm : HtmlRenderer.OverlayForm
    {
        public OverlayForm(string overlayVersion, string overlayName, string url, int maxFrameRate = 30)
            : base(overlayVersion, overlayName, url, maxFrameRate)
        {
        }
    }
}
