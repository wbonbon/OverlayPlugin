using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainbowMage.HtmlRenderer
{
    class RenderHandler : CefRenderHandler
    {
        private Renderer renderer;

        public int Height { get; set; }
        public int Width { get; set; }

        public RenderHandler(Renderer renderer, int windowWidth, int windowHeight)
        {
            this.renderer = renderer;
            this.Width = windowWidth;
            this.Height = windowHeight;
        }

        protected override bool GetRootScreenRect(ChromiumWebBrowser browser, ref CefRectangle rect)
        {
            return GetViewRect(browser, ref rect);
        }

        protected override bool GetScreenPoint(ChromiumWebBrowser browser, int viewX, int viewY, ref int screenX, ref int screenY)
        {
            screenX = viewX;
            screenY = viewY;
            return true;
        }

        protected override bool GetViewRect(ChromiumWebBrowser browser, ref CefRectangle rect)
        {
            rect.X = 0;
            rect.Y = 0;
            rect.Width = Width;
            rect.Height = Height;
            return true;
        }

        protected override bool GetScreenInfo(ChromiumWebBrowser browser, CefScreenInfo screenInfo)
        {
            return false;
        }

        protected override void OnPopupSize(ChromiumWebBrowser browser, CefRectangle rect)
        {
        }

        protected override void OnPaint(ChromiumWebBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height)
        {
            renderer.OnPaint(browser, buffer, width, height, dirtyRects);
        }

        protected override void OnCursorChange(ChromiumWebBrowser browser, IntPtr cursorHandle, CefCursorType type, CefCursorInfo customCursorInfo)
        {

        }

        protected override void OnScrollOffsetChanged(ChromiumWebBrowser browser, double x, double y)
        {

        }
    }

    public class RenderEventArgs : EventArgs
    {
        public IntPtr Buffer { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public CefRectangle[] DirtyRects { get; private set; }

        public RenderEventArgs(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects)
        {
            this.Buffer = buffer;
            this.Width = width;
            this.Height = height;
            this.DirtyRects = dirtyRects;
        }
    }
}
