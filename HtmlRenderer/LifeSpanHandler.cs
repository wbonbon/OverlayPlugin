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
        private bool isFirstBrowser = true;

        public LifeSpanHandler(Renderer renderer)
        {
            this.renderer = renderer;
        }
        
        protected override void OnAfterCreated(CefBrowser browser)
        {
            base.OnAfterCreated(browser);

            // When second (or more) window are created, this caused update
            // Renderer's Browser object to new Window's Browser.
            if(this.isFirstBrowser)
                this.renderer.OnCreated(browser);

           this.isFirstBrowser = false;
        }
    }
}
