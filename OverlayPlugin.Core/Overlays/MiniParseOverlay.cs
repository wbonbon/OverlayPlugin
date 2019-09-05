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
        protected bool modernApi = false;

        public MiniParseOverlay(MiniParseOverlayConfig config, string name)
            : base(config, name)
        {
            Config.ActwsCompatibilityChanged += (o, e) =>
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
            if (Config.ActwsCompatibility && e.Message.Contains("ws://127.0.0.1/fake/") && (e.Message.Contains("SecurityError:") || e.Message.Contains("ERR_CONNECTION_")))
            {
                Overlay.Reload();
            }
        }

        private void FinishLoading(object sender, BrowserLoadEventArgs e)
        {
            if (Config.ActwsCompatibility)
            {
                var charName = JsonConvert.SerializeObject(FFXIVRepository.GetPlayerName() ?? "YOU");
                var charID = JsonConvert.SerializeObject(FFXIVRepository.GetPlayerID());
                
                ExecuteScript("__OverlayPlugin_ws_faker({ msgtype: 'SendCharName', msg: { charName: " + charName + ", charID: " + charID + " }});");
            }
        }

        private void PrepareWebsite(object sender, BrowserLoadEventArgs e)
        {
            if (Config.ActwsCompatibility)
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

                            if (queue !== null) {
                                setTimeout(() => {
                                    queue.forEach(__OverlayPlugin_ws_faker);
                                    queue = null;
                                }, 100);
                            }
                        }
                        else
                        {
                            return new realWS(url);
                        }
                    };
                })();");

                Subscribe("CombatData");
            } else {
                // Subscriptions are cleared on page navigation so we have to restore this after every load.

                modernApi = false;
                Subscribe("CombatData");
            }
        }

        public override void Navigate(string url)
        {
            if (Config.ActwsCompatibility && !url.Contains("HOST_PORT="))
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
            if (modernApi)
            {
                base.HandleEvent(e);
            }
            else if(e["type"].ToString() == "CombatData")
            {
                if (Config.ActwsCompatibility)
                {
                    ExecuteScript("__OverlayPlugin_ws_faker({'type': 'broadcast', 'msgtype': 'CombatData', 'msg':  " + e.ToString(Formatting.None) + " });");
                } else
                {
                    ExecuteScript("document.dispatchEvent(new CustomEvent('onOverlayDataUpdate', { detail: " + e.ToString(Formatting.None) + " }));");
                }
            }
        }

        public override void InitModernAPI()
        {
            // Clear the subscription set in PrepareWebsite().
            Unsubscribe("CombatData");
            modernApi = true;
        }

        protected override void Update()
        {
            
        }
    }
}
