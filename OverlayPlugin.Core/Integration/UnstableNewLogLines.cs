using System;
using System.Collections.Generic;
using Advanced_Combat_Tracker;
using RainbowMage.OverlayPlugin.NetworkProcessors;

namespace RainbowMage.OverlayPlugin.Integration
{
    public class UnstableNewLogLines
    {
        private bool inCutscene = false;
        private FFXIVRepository repo = null;

        public UnstableNewLogLines(TinyIoCContainer container)
        {
            repo = container.Resolve<FFXIVRepository>();
            container.Resolve<NetworkParser>().OnOnlineStatusChanged += OnOnlineStatusChange;
        }

        private void OnOnlineStatusChange(object sender, OnlineStatusChangedArgs ev)
        {
            if (ev.Target != repo.GetPlayerID())
                return;

            var cutsceneStatus = ev.Status == 15;
            if (cutsceneStatus != inCutscene)
            {
                inCutscene = cutsceneStatus;
                string msg;

                if (cutsceneStatus)
                {
                    msg = "Entered cutscene";
                } else
                {
                    msg = "Left cutscene";
                }

                var time = DateTime.Now;
                var line = new string[] { "00", time.ToString(), "c0fe", "", "OPLine: " + msg, ""};
                ActGlobals.oFormActMain.ParseRawLogLine(false, time, string.Join("|", line));
            }
        }
    }
}
