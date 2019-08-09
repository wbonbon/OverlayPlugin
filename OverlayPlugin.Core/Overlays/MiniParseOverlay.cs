using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RainbowMage.OverlayPlugin.Overlays
{
    public partial class MiniParseOverlay : OverlayBase<MiniParseOverlayConfig>
    {
        public MiniParseOverlay(MiniParseOverlayConfig config)
            : base(config, config.Name)
        {
            config.CompatibilityChanged += (o, e) =>
            {
                Navigate(Overlay.Url);
            };

            // Subscriptions are cleared on page navigation so we have to restore this after every load.
            Overlay.Renderer.BrowserLoad += (o, e) =>
            {
                Subscribe("CombatData");
            };
        }

        public override void Navigate(string url)
        {
            if (Config.Compatibility == "actws" && !url.Contains("HOST_PORT="))
            {
                url += "?HOST_PORT=ws://fake.ws/";
            }

            if (url != Overlay.Url)
            {
                base.Navigate(url);
            }
        }

        public override void Start()
        {
            
        }

        public override void Stop()
        {
            
        }

        public override void HandleEvent(JObject e)
        {
            if (e["type"].ToString() == "CombatData")
            {
                switch(Config.Compatibility)
                {
                    case "overlay":
                        base.HandleEvent(e);
                        break;
                    case "legacy":
                        base.HandleEvent(e);
                        ExecuteScript("document.dispatchEvent(new CustomEvent('onOverlayDataUpdate', { detail: " + e.ToString(Formatting.None) + " }));");
                        break;
                    case "actws":
                        ExecuteScript("__OverlayPlugin_ws_faker({'type': 'broadcast', 'msgtype': 'CombatData', 'msg':  " + e.ToString(Formatting.None) + " });");
                        break;
                }
            } else
            {
                base.HandleEvent(e);
            }
        }

        protected override void Update()
        {
            
        }
    }
}
