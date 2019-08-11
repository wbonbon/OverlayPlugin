using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Advanced_Combat_Tracker;

namespace RainbowMage.OverlayPlugin
{
    class WSServer
    {
        HttpServer _server;
        static bool _failed = false;
        static WSServer _inst = null;

        public static EventHandler<StateChangedArgs> OnStateChanged;

        public static void Initialize(PluginConfig cfg)
        {
            _inst = new WSServer(cfg);
        }

        public static void Stop()
        {
            if (_inst != null)
            {
                try
                {
                    _inst._server.Stop();
                }
                catch (Exception e)
                {
                    Log(LogLevel.Error, "WS: Failed to shutdown. {0}", e);
                }
                _inst = null;
                _failed = false;

                OnStateChanged(null, new StateChangedArgs(false, false));
            }
        }

        public static bool IsRunning()
        {
            return _inst != null && _inst._server != null && _inst._server.IsListening;
        }

        public static bool IsFailed()
        {
            return _failed;
        }

        public static bool IsSSLPossible()
        {
            return File.Exists(GetCertPath());
        }

        private WSServer(PluginConfig cfg)
        {
            _failed = false;

            try
            {
                var sslPath = GetCertPath();
                var secure = cfg.WSServerSSL && File.Exists(sslPath);

                _server = new HttpServer(IPAddress.Parse(cfg.WSServerIP), cfg.WSServerPort, secure);
                _server.ReuseAddress = true;
                _server.Log.Output += (LogData d, string msg) =>
                {
                    Log(LogLevel.Info, "WS: {0}: {1} {2}", d.Level.ToString(), d.Message, msg);
                };
                _server.Log.Level = WebSocketSharp.LogLevel.Debug;

                if (secure)
                {
                    Log(LogLevel.Debug, "WS: Loading SSL certificate {0}...", sslPath);

                    _server.SslConfiguration.ServerCertificate = new X509Certificate2(sslPath, "changeit");
                    _server.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls12;
                }

                _server.AddWebSocketService<SocketHandler>("/ws");
                _server.AddWebSocketService<LegacyHandler>("/MiniParse");
                _server.AddWebSocketService<LegacyHandler>("/BeforeLogLineRead");

                _server.OnGet += (object sender, HttpRequestEventArgs e) =>
                {
                    if (e.Request.RawUrl == "/")
                    {
                        var doc = "<!DOCTYPE html><html><head><title>OverlayPlugin WSServer</title></head><body><h1>It Works!</h1><p>More to come...</p></body></html>";

                        var res = e.Response;
                        res.StatusCode = 200;
                        res.ContentType = "text/html";
                        Ext.WriteContent(res, Encoding.UTF8.GetBytes(doc));
                    }
                };

                _server.Start();
                OnStateChanged?.Invoke(this, new StateChangedArgs(true, false));
            }
            catch(Exception e)
            {
                _failed = true;
                Log(LogLevel.Error, "WS: Failed to start: {0}", e);
                OnStateChanged?.Invoke(this, new StateChangedArgs(false, true));
            }
        }

        public static string GetCertPath()
        {
            var path = Path.Combine(
                ActGlobals.oFormActMain.AppDataFolder.FullName,
                "Config",
                "OverlayPluginSSL.p12");

            return path;
        }

        private static void Log(LogLevel level, string msg, params object[] args)
        {
            PluginMain.Logger.Log(level, msg, args);
        }

        public class SocketHandler : WebSocketBehavior, IEventReceiver
        {
            public void HandleEvent(JObject e)
            {
                SendAsync(e.ToString(Formatting.None), (success) =>
                {
                    if (!success)
                    {
                        Log(LogLevel.Error, "Failed to send message: {0}", e);
                    }
                });
            }

            protected override void OnOpen()
            {

            }

            protected override void OnMessage(MessageEventArgs e)
            {
                JObject data = null;

                try
                {
                    data = JObject.Parse(e.Data);
                }
                catch(JsonException ex)
                {
                    Log(LogLevel.Error, "Invalid data received: {0}; {1}", ex, e.Data);
                    return;
                }

                if (!data.ContainsKey("call")) return;

                var msgType = data["call"].ToString();
                if (msgType == "subscribe")
                {
                    try
                    {
                        foreach (var item in data["events"].ToList())
                        {
                            EventDispatcher.Subscribe(item.ToString(), this);
                        }
                    } catch(Exception ex)
                    {
                        Log(LogLevel.Error, "Failed to process new subscription: {0}", ex);
                    }

                    return;
                } else if (msgType == "unsubscribe")
                {
                    try
                    {
                        foreach (var item in data["events"].ToList())
                        {
                            EventDispatcher.Unsubscribe(item.ToString(), this);
                        }
                    } catch (Exception ex)
                    {
                        Log(LogLevel.Error, "Failed to process unsubscription: {0}", ex);
                    }
                    return;
                }

                Task.Run(() => {
                    try
                    {
                        var response = EventDispatcher.CallHandler(data);

                        if (response == null) {
                            response = new JObject();
                            response["$isNull"] = true;
                        }

                        if (data.ContainsKey("rseq")) {
                            response["rseq"] = data["rseq"];
                        }

                        Send(response.ToString(Formatting.None));
                    } catch(Exception ex)
                    {
                        Log(LogLevel.Error, "WS: Handler call failed: {0}", ex);
                    }
                });
            }


            protected override void OnClose(CloseEventArgs e)
            {
                EventDispatcher.UnsubscribeAll(this);
            }
        }

        private class LegacyHandler : WebSocketBehavior, IEventReceiver
        {
            protected override void OnOpen()
            {
                base.OnOpen();

                EventDispatcher.Subscribe("CombatData", this);
            }

            protected override void OnClose(CloseEventArgs e)
            {
                base.OnClose(e);

                EventDispatcher.UnsubscribeAll(this);
            }

            public void HandleEvent(JObject e)
            {
                Send("{\"type\":\"broadcast\",\"msgtype\":\"CombatData\",\"msg\":" + e.ToString(Formatting.None) + "}");
            }
        }

        public class StateChangedArgs : EventArgs
        {
            public bool Running { get; private set; }
            public bool Failed { get; private set; }

            public StateChangedArgs(bool Running, bool Failed)
            {
                this.Running = Running;
                this.Failed = Failed;
            }
        }
    }
}
