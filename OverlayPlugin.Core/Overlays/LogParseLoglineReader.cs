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
        // Part of ACTWebSocket
        // Copyright (c) 2016 ZCube; Licensed under MIT license.
        public enum MessageType
        {
            Unknown = -1,
            LogLine = 0,
            ChangeZone = 1,
            ChangePrimaryPlayer = 2,
            AddCombatant = 3,
            RemoveCombatant = 4,
            AddBuff = 5,
            RemoveBuff = 6,
            FlyingText = 7,
            OutgoingAbility = 8,
            IncomingAbility = 10,
            PartyList = 11,
            PlayerStats = 12,
            CombatantHP = 13,
            NetworkStartsCasting = 20,
            NetworkAbility = 21,
            NetworkAOEAbility = 22,
            NetworkCancelAbility = 23,
            NetworkDoT = 24,
            NetworkDeath = 25,
            NetworkBuff = 26,
            NetworkTargetIcon = 27,
            NetworkRaidMarker = 28,
            NetworkTargetMarker = 29,
            NetworkBuffRemove = 30,
            Debug = 251,
            PacketDump = 252,
            Version = 253,
            Error = 254,
            Timer = 255
        }
        
        private void LogLineReader(bool isImported, LogLineEventArgs e)
        {
            if (isImported)
            {
                return;
            }
            try
            {
                string[] chunk = e.logLine.Split(new[] { ' ' }, 2);
                string timestamp = chunk[0];
                string[] body = chunk[1].Split(new[] { ':' }, 2);

                if (body.Length < 1) // DataErr0r
                {
                    return;
                }

                sendEvent(isImported, MessageType.LogLine, timestamp, body);
            }
            catch(Exception err) { }
            
        }

        private void sendEvent(bool isImported, MessageType type, string timestamp, string[] payload)
        {
            if (this.Overlay != null &&
                this.Overlay.Renderer != null &&
                this.Overlay.Renderer.Browser != null)
            {
                JObject message = new JObject();
                message["opcode"] = Convert.ToInt32(payload[0], 16);
                message["timestamp"] = timestamp;
                message["payload"] = payload[1];
                this.Overlay.Renderer.ExecuteScript(
                    "document.dispatchEvent(new CustomEvent('onLogLine', { detail: " + message.ToString() + " } ));"
                );
            }
        }
    }
}
