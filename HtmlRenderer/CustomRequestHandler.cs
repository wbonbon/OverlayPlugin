using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Handler;

namespace RainbowMage.HtmlRenderer
{
    class CustomRequestHandler : RequestHandler
    {
        Renderer _renderer;

        public CustomRequestHandler(Renderer renderer) : base()
        {
            _renderer = renderer;
        }

        protected override void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status)
        {
            _renderer.Browser_ConsoleMessage(this, new ConsoleMessageEventArgs(LogSeverity.Error, "Browser crashed! Trying to restart.", "internal", 1));
            _renderer.BeginRender();
        }
    }
}
