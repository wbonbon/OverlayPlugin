using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Advanced_Combat_Tracker;

namespace RainbowMage.OverlayPlugin
{
    class OverlayApi
    {
        public static event EventHandler<BroadcastMessageEventArgs> BroadcastMessage;
        public static event EventHandler<SendMessageEventArgs> SendMessage;
        public static event EventHandler<SendMessageEventArgs> OverlayMessage;

        IEventReceiver receiver;

        public OverlayApi(IEventReceiver receiver)
        {
            this.receiver = receiver;
        }

        public void broadcastMessage(string msg)
        {
            BroadcastMessage(this, new BroadcastMessageEventArgs(msg));
        }

        public void sendMessage(string target, string msg)
        {
            SendMessage(this, new SendMessageEventArgs(target, msg));
        }

        public void overlayMessage(string target, string msg)
        {
            OverlayMessage(this, new SendMessageEventArgs(target, msg));
        }

        public void endEncounter()
        {
            ActGlobals.oFormActMain.EndCombat(true);
        }

        // Also handles (unc)subscription to make switching between this and WS easier.
        public Task<JObject> callHandler(string data)
        {
            try
            {
                var message = JObject.Parse(data);
                if (!message.ContainsKey("call"))
                {
                    PluginMain.Logger.Log(LogLevel.Error, $"Received invalid handler call: {data}");
                    return null;
                }
                
                var handler = message["call"].ToString();
                if (handler == "subscribe")
                {
                    if (!message.ContainsKey("events"))
                    {
                        PluginMain.Logger.Log(LogLevel.Error, $"Missing events field in subscribe call: {data}!");
                        return null;
                    }

                    foreach (var name in message["events"].ToList())
                    {
                        EventDispatcher.Subscribe(name.ToString(), receiver);
                    }
                    return null;
                } else if (handler == "unsubscribe")
                {
                    if (!message.ContainsKey("events"))
                    {
                        PluginMain.Logger.Log(LogLevel.Error, $"Missing events field in unsubscribe call: {data}!");
                        return null;
                    }

                    foreach (var name in message["events"].ToList())
                    {
                        EventDispatcher.Unsubscribe(name.ToString(), receiver);
                    }
                    return null;
                }

                return EventDispatcher.CallHandler(message);
            }
            catch (Exception e)
            {
                PluginMain.Logger.Log(LogLevel.Error, $"JS Handler call failed: {e}");
                return null;
            }
        }
    }

    public class BroadcastMessageEventArgs : EventArgs
    {
        public string Message { get; private set; }

        public BroadcastMessageEventArgs(string message)
        {
            this.Message = message;
        }
    }

    public class SendMessageEventArgs : EventArgs
    {
        public string Target { get; private set; }
        public string Message { get; private set; }

        public SendMessageEventArgs(string target, string message)
        {
            this.Target = target;
            this.Message = message;
        }
    }

    public class EndEncounterEventArgs : EventArgs
    {
        public EndEncounterEventArgs()
        {

        }
    }
}
