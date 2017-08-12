using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace RainbowMage.OverlayPlugin.Overlays
{
    partial class LogParseOverlay : OverlayBase<LogParseOverlayConfig>
    {
        private void LogLineReader(bool isImported, LogLineEventArgs e)
        {
            if (this.Overlay != null &&
                this.Overlay.Renderer != null &&
                this.Overlay.Renderer.Browser != null)
            {
                JObject message = new JObject();
                message["isImported"] = isImported;
                message["message"] = e.logLine;
                this.Overlay.Renderer.ExecuteScript(
                    "document.dispatchEvent(new CustomEvent('onLogLine', { detail: " + message.ToString() + " } ));"
                );
            }
        }
    }
}
