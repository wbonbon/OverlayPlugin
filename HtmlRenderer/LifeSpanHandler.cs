using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace RainbowMage.HtmlRenderer
{
    class LifeSpanHandler : CefLifeSpanHandler
    {
        private readonly Renderer renderer;

        public LifeSpanHandler(Renderer renderer)
        {
            this.renderer = renderer;
        }
        
        protected override void OnAfterCreated(ChromiumWebBrowser browser)
        {
            base.OnAfterCreated(browser);

            this.renderer.OnCreated(browser);
        }

        protected override void OnBeforeClose(ChromiumWebBrowser browser)
        {
            base.OnBeforeClose(browser);

            this.renderer.OnBeforeClose(browser);
        }
    }
}
