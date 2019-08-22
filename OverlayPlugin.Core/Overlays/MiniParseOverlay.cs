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
using RainbowMage.HtmlRenderer;

namespace RainbowMage.OverlayPlugin.Overlays
{
    public partial class MiniParseOverlay : OverlayBase<MiniParseOverlayConfig>
    {
        public MiniParseOverlay(MiniParseOverlayConfig config, string name)
            : base(config, name)
        {
            Config.CompatibilityChanged += (o, e) =>
            {
                Navigate(Overlay.Url);
            };

            Overlay.Renderer.BrowserStartLoading += PrepareWebsite;
            Overlay.Renderer.BrowserLoad += FinishLoading;
            Overlay.Renderer.BrowserConsoleLog += Renderer_BrowserConsoleLog;
        }

        public override Control CreateConfigControl()
        {
            return new MiniParseConfigPanel(this);
        }

        private void Renderer_BrowserConsoleLog(object sender, BrowserConsoleLogEventArgs e)
        {
            if (Config.Compatibility == "actws" && e.Message.Contains("ws://127.0.0.1/fake/") && (e.Message.Contains("SecurityError:") || e.Message.Contains("ERR_CONNECTION_")))
            {
                Overlay.Reload();
            }
        }

        private void FinishLoading(object sender, BrowserLoadEventArgs e)
        {
            if (Config.Compatibility == "actws")
            {
                var charName = JsonConvert.SerializeObject(FFXIVRepository.GetPlayerName() ?? "YOU");
                var charID = JsonConvert.SerializeObject(FFXIVRepository.GetPlayerID());
                
                ExecuteScript("__OverlayPlugin_ws_faker({ msgtype: 'SendCharName', msg: { charName: " + charName + ", charID: " + charID + " }});");
            }
        }

        private void PrepareWebsite(object sender, BrowserLoadEventArgs e)
        {
            if (Config.Compatibility == "actws")
            {
                // Install a fake WebSocket so we can directly call the event handler.
                ExecuteScript(@"(function() {
                    let realWS = window.WebSocket;
                    let queue = [];
                    window.__OverlayPlugin_ws_faker = (msg) => queue.push(msg);

                    window.WebSocket = function(url) {
                        if (url.indexOf('ws://127.0.0.1/fake/') > -1)
                        {
                            window.__OverlayPlugin_ws_faker = (msg) => {
                                if (this.onmessage) this.onmessage({ data: JSON.stringify(msg) });
                            };
                            console.log('ACTWS compatibility shim enabled.');

                            setTimeout(() => {
                                queue.forEach(__OverlayPlugin_ws_faker);
                                queue = null;
                            }, 100);
                        }
                        else
                        {
                            return new realWS(url);
                        }
                    };
                })();");

                Subscribe("CombatData");
            } else if (Config.Compatibility == "legacy") {
                // Subscriptions are cleared on page navigation so we have to restore this after every load.

                Subscribe("CombatData");
            }
        }

        public override void Navigate(string url)
        {
            if (Config.Compatibility == "actws" && !url.Contains("HOST_PORT="))
            {
                url += "?HOST_PORT=ws://127.0.0.1/fake/";
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
