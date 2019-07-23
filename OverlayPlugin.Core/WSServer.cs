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
using Advanced_Combat_Tracker;

namespace RainbowMage.OverlayPlugin
{
    class WSServer
    {
        HttpServer _server;
        static bool _failed = false;
        static WSServer _inst = null;
        static public EventHandler<StateChangedArgs> OnStateChanged;

        static public void Initialize(PluginConfig cfg)
        {
            _inst = new WSServer(cfg);
        }

        static public void Stop()
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

        static public bool IsRunning()
        {
            return _inst != null && _inst._server != null && _inst._server.IsListening;
        }

        static public bool IsFailed()
        {
            return _failed;
        }

        static public bool IsSSLPossible()
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
                _server.Log.Level = WebSocketSharp.LogLevel.Trace;

                if (secure)
                {
                    Log(LogLevel.Debug, "WS: Loading SSL certificate {0}...", sslPath);

                    _server.SslConfiguration.ServerCertificate = new X509Certificate2(sslPath, "changeit");
                    _server.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls12;
                }

                _server.AddWebSocketService<SocketHandler>("/MiniParse");

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
                if (OnStateChanged != null) OnStateChanged(this, new StateChangedArgs(true, false));
            }
            catch(Exception e)
            {
                _failed = true;
                Log(LogLevel.Error, "WS: Failed to start: {0}", e);
                if (OnStateChanged != null) OnStateChanged(this, new StateChangedArgs(false, true));
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

        public static void Broadcast(string msg)
        {
            if (IsRunning()) _inst._server.WebSocketServices.Broadcast(msg);
        }

        private class SocketHandler : WebSocketBehavior
        {

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
