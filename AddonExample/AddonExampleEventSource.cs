using Newtonsoft.Json.Linq;
using RainbowMage.OverlayPlugin;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AddonExample
{

    public class AddonExampleEventSource : EventSourceBase
    {
        public AddonExampleEventSourceConfig Config { get; private set; }

        // Original Timer
        System.Timers.Timer originalTimer;

        // Events
        public delegate void AddonExampleOriginalTimerFiredHandler(JSEvents.OriginalTimerFiredEvent e);
        public event AddonExampleOriginalTimerFiredHandler OnAddonExampleOriginalTimerFired;

        public delegate void AddonExampleEmbeddedTimerFiredHandler(JSEvents.EmbeddedTimerFiredEvent e);
        public event AddonExampleEmbeddedTimerFiredHandler OnAddonExampleEmbeddedTimerFired;


        public AddonExampleEventSource(RainbowMage.OverlayPlugin.ILogger logger) : base(logger)
        {
            Name = "AddonExampleES";

            // Register Events subscribe to other EventSources/Overlays
            RegisterEventTypes(new List<string>()
            {
                "onAddonExampleOriginalTimerFiredEvent", "onAddonExampleEmbeddedTimerFiredEvent",
            });


            // Register EventHandler
            // This EventHandler is called from other EventSources/Overlays
            // You can execute some process or response data.
            
            RegisterEventHandler("addonExampleSay", (msg) => {
                Advanced_Combat_Tracker.ActGlobals.oFormActMain.TTS(msg["text"].ToString());
                return null;
            });
            RegisterEventHandler("addonExampleCurrentTime", (msg) => {
                var ret = new JObject();
                ret["time"] = DateTimeOffset.UtcNow.ToString();
                return ret;
            });
        }

        public override Control CreateConfigControl()
        {
            return new AddonExampleEventSourceConfigPanel(this);
        }

        public override void LoadConfig(IPluginConfig config)
        {
            Config = AddonExampleEventSourceConfig.LoadConfig(config);
        }

        public override void SaveConfig(IPluginConfig config)
        {
            Config.SaveConfig(config);
        }

        public override void Start()
        {
            // Reset Event Dispatcher
            OnAddonExampleOriginalTimerFired -= (e) => DispatchToJS(e);
            OnAddonExampleOriginalTimerFired += (e) => DispatchToJS(e);
            OnAddonExampleEmbeddedTimerFired -= (e) => DispatchToJS(e);
            OnAddonExampleEmbeddedTimerFired += (e) => DispatchToJS(e);

            // Start the embedded timer when using it.
            // Call base.Start() or timer.Change(0, interval) to start the embedded timer manually.
            base.Start();
            //timer.Change(0, 1000);

            // Start Original Timer
            originalTimer = new System.Timers.Timer()
            {
                Interval = 1000,
                AutoReset = true,
            };
            originalTimer.Elapsed += (obj, args) =>
            {
                OnAddonExampleOriginalTimerFired(new JSEvents.OriginalTimerFiredEvent(Config.ExampleString + " fired!"));
            };
            originalTimer.Start();

            this.Log(LogLevel.Info, "Plugin Started.");
        }

        public override void Stop()
        {
            // Stop original timer
            originalTimer.Stop();

            // Stop the embedded timer when using it.
            // Call base.stop() or timer.Change(-1, -1) to stop the embedded timer manually.
            base.Stop();
            //timer.Change(-1, -1);
        }

        /// <summary>
        /// This method is called periodically when using the embedded timer.
        /// </summary>
        protected override void Update()
        {
            OnAddonExampleEmbeddedTimerFired(new JSEvents.EmbeddedTimerFiredEvent("fired!"));
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        // Sends an event called |event_name| to javascript, with an event.detail that contains
        // the fields and values of the |detail| structure.
        public void DispatchToJS(JSEvent e)
        {
            JObject ev = new JObject();
            ev["type"] = e.EventName();
            ev["detail"] = JObject.FromObject(e);
            DispatchEvent(ev);
        }
    }

    public interface JSEvent
    {
        string EventName();
    };

    public class JSEvents
    {
        public class OriginalTimerFiredEvent : JSEvent
        {
            public string message;
            public OriginalTimerFiredEvent(string message) { this.message = message; }
            public string EventName() { return "onAddonExampleOriginalTimerFiredEvent"; }
        }

        public class EmbeddedTimerFiredEvent : JSEvent
        {
            public string message;
            public EmbeddedTimerFiredEvent(string message) { this.message = message; }
            public string EventName() { return "onAddonExampleEmbeddedTimerFiredEvent"; }
        }
    }
}
