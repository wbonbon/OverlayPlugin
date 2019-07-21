using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainbowMage.HtmlRenderer
{
    class BuiltinFunctionHandler
    {
        public const string BroadcastMessageFunctionName = "broadcastMessage";
        public const string SendMessageFunctionName = "sendMessage";
        public const string OverlayMessageFunctionName = "overlayMessage";
        public const string EndEncounterFunctionName = "endEncounter";

        public void broadcastMessage(string msg)
        {
            HtmlRenderer.Renderer.TriggerBroadcastMessage(this, new BroadcastMessageEventArgs(msg));
        }

        public void sendMessage(string target, string msg)
        {
            HtmlRenderer.Renderer.TriggerSendMessage(this, new SendMessageEventArgs(target, msg));
        }

        public void overlayMessage(string target, string msg)
        {
            HtmlRenderer.Renderer.TriggerOverlayMessage(this, new SendMessageEventArgs(target, msg));
        }

        public void endEncounter()
        {
            HtmlRenderer.Renderer.TriggerRendererFeatureRequest(this, new RendererFeatureRequestEventArgs("EndEncounter"));
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
