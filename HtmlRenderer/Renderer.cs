using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp.OffScreen;
using CefSharp;
using System.IO;
using Newtonsoft.Json;

namespace RainbowMage.HtmlRenderer
{
    public class Renderer : IDisposable
    {
        public event EventHandler<BrowserErrorEventArgs> BrowserError;
        public event EventHandler<BrowserLoadEventArgs> BrowserLoad;
        public event EventHandler<BrowserConsoleLogEventArgs> BrowserConsoleLog;

        public static event EventHandler<BroadcastMessageEventArgs> BroadcastMessage;
        public static event EventHandler<SendMessageEventArgs> SendMessage;
        public static event EventHandler<SendMessageEventArgs> OverlayMessage;
        public static event EventHandler<RendererFeatureRequestEventArgs> RendererFeatureRequest;

        public event EventHandler<OnPaintEventArgs> Render;

        public ChromiumWebBrowser Browser;
        private List<String> scriptQueue = new List<string>();
        
        public string OverlayVersion {
            get { return overlayVersion; }
        }

        public string OverlayName {
          get { return overlayName; }
        }

        //private Client Client { get; set; }

        private int clickCount;
        private MouseButtonType lastClickButton;
        private DateTime lastClickTime;
        private int lastClickPosX;
        private int lastClickPosY;
        private string overlayVersion;
        private string overlayName;

        public Renderer(string overlayVersion, string overlayName)
        {
            this.overlayVersion = overlayVersion;
            this.overlayName = overlayName;

            this.Browser = new ChromiumWebBrowser("about:blank");
            Browser.FrameLoadStart += Browser_FrameLoadStart;
            Browser.FrameLoadEnd += Browser_FrameLoadEnd;
            Browser.LoadError += Browser_LoadError;
            Browser.ConsoleMessage += Browser_ConsoleMessage;
            Browser.Paint += Browser_Paint;

            Browser.JavascriptObjectRepository.Register("OverlayPluginApi", new BuiltinFunctionHandler(), isAsync: true);
            Browser.JavascriptObjectRepository.ObjectBoundInJavascript += JavascriptObjectRepository_ObjectBoundInJavascript;
        }

        private void JavascriptObjectRepository_ObjectBoundInJavascript(object sender, CefSharp.Event.JavascriptBindingCompleteEventArgs e)
        {
            BrowserConsoleLog(sender, new BrowserConsoleLogEventArgs("Object " + e.ObjectName + " succesfully bound.", "internal", 1));
        }

        public static void TriggerBroadcastMessage(object sender, BroadcastMessageEventArgs e)
        {
            BroadcastMessage(sender, e);
        }

        public static void TriggerSendMessage(object sender, SendMessageEventArgs e)
        {
            SendMessage(sender, e);
        }

        public static void TriggerOverlayMessage(object sender, SendMessageEventArgs e)
        {
            OverlayMessage(sender, e);
        }

        public static void TriggerRendererFeatureRequest(object sender, RendererFeatureRequestEventArgs e)
        {
            RendererFeatureRequest(sender, e);
        }

        private void Browser_Paint(object sender, OnPaintEventArgs e)
        {
            Render(sender, e);
        }

        private void Browser_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            var initScript = @"(async () => {
                await CefSharp.BindObjectAsync('OverlayPluginApi');
                OverlayPluginApi.overlayName = " + JsonConvert.SerializeObject(this.overlayName) + @";
            })();";
            e.Frame.ExecuteJavaScriptAsync(initScript);

            foreach (var item in this.scriptQueue)
            {
                e.Frame.ExecuteJavaScriptAsync(item);
            }
            this.scriptQueue.Clear();
        }

        private void Browser_LoadError(object sender, LoadErrorEventArgs e)
        {
            BrowserError(sender, new BrowserErrorEventArgs(e.ErrorCode, e.ErrorText, e.FailedUrl));
        }

        private void Browser_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            BrowserConsoleLog(sender, new BrowserConsoleLogEventArgs(e.Message, e.Source, e.Line));
        }

        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            BrowserLoad(sender, new BrowserLoadEventArgs(e.HttpStatusCode, e.Url));
        }

        public void Load(string url)
        {
            if (this.Browser != null)
            {
                this.Browser.Load(url);
            }
        }

        public void BeginRender(int width, int height, string url, int maxFrameRate = 30)
        {
            EndRender();

            var cefWindowInfo = new WindowInfo();
            cefWindowInfo.SetAsWindowless(IntPtr.Zero);
            cefWindowInfo.Width = width;
            cefWindowInfo.Height = height;

            var cefBrowserSettings = new BrowserSettings();
            cefBrowserSettings.WindowlessFrameRate = maxFrameRate;
            Browser.CreateBrowser(cefWindowInfo, cefBrowserSettings);
        }

        public void EndRender()
        {
            
        }

        public void Reload()
        {
            if (this.Browser != null)
            {
                this.Browser.Reload();
            }
        }

        public void Resize(int width, int height)
        {
            if (this.Browser != null)
            {
                this.Browser.Size = new System.Drawing.Size(width, height);
            }
        }

        public void SendMouseMove(int x, int y, MouseButtonType button)
        {
            if (this.Browser != null)
            {
                var host = this.Browser.GetBrowserHost();
                var modifiers = CefEventFlags.None;
                if (button == MouseButtonType.Left)
                {
                    modifiers = CefEventFlags.LeftMouseButton;
                }
                else if (button == MouseButtonType.Middle)
                {
                    modifiers = CefEventFlags.MiddleMouseButton;
                }
                else if (button == MouseButtonType.Right)
                {
                    modifiers = CefEventFlags.RightMouseButton;
                }

                var mouseEvent = new MouseEvent(x, y, modifiers);
                host.SendMouseMoveEvent(mouseEvent, false);
            }
        }

        public void SendMouseUpDown(int x, int y, MouseButtonType button, bool isMouseUp)
        {
            if (this.Browser != null)
            {
                var host = this.Browser.GetBrowserHost();

                if (!isMouseUp)
                {
                    if (IsContinuousClick(x, y, button))
                    {
                        clickCount++;
                    }
                    else
                    {
                        clickCount = 1;
                    }
                }

                var mouseEvent = new MouseEvent(x, y, CefEventFlags.None);
                host.SendMouseClickEvent(mouseEvent, button, isMouseUp, clickCount);

                lastClickPosX = x;
                lastClickPosY = y;
                lastClickButton = button;
                lastClickTime = DateTime.Now;
            }
        }

        public void SendMouseWheel(int x, int y, int delta, bool isVertical)
        {
            if (this.Browser != null)
            {
                var host = this.Browser.GetBrowserHost();
                var mouseEvent = new MouseEvent(x, y, CefEventFlags.None);
                host.SendMouseWheelEvent(mouseEvent, isVertical ? delta : 0, !isVertical ? delta : 0);
            }
        }

        private bool IsContinuousClick(int x, int y, MouseButtonType button)
        {
            // ダブルクリックとして認識するクリックの間隔よりも大きかった場合は継続的なクリックとみなさない
            var delta = (DateTime.Now - lastClickTime).TotalMilliseconds;
            if (delta > System.Windows.Forms.SystemInformation.DoubleClickTime)
            {
                return false;
            }

            // クリック位置が違う、もしくはボタンが違う場合にも継続的なクリックとはみなさない
            if (lastClickPosX != x || lastClickPosY != y || lastClickButton != button)
            {
                return false;
            }

            return true;
        }

        public void SendKeyEvent(KeyEvent keyEvent)
        {
            if (this.Browser != null)
            {
                var host = this.Browser.GetBrowserHost();

                host.SendKeyEvent(keyEvent);
            }
        }

        public void showDevTools(bool firstwindow = true)
        {
            if (Browser != null)
            {
                WindowInfo wi = new WindowInfo();
                wi.SetAsPopup(Browser.GetBrowserHost().GetWindowHandle(), "DevTools");
                Browser.GetBrowserHost().ShowDevTools(wi);
            }
        }

        public void Dispose()
        {
        }

        static bool initialized = false;

        public static void Initialize(string pluginDirectory)
        {
            if (!initialized)
            {
                Cef.EnableHighDPISupport();
                
                var cefSettings = new CefSettings
                {
                    Locale = System.Globalization.CultureInfo.CurrentCulture.Name,
                    CachePath = Path.Combine(pluginDirectory, "Cache"),
                    MultiThreadedMessageLoop = true,
                    LogSeverity = LogSeverity.Disable,
                    BrowserSubprocessPath = Path.Combine(pluginDirectory,
                                           Environment.Is64BitProcess ? "x64" : "x86",
                                           "CefSharp.BrowserSubprocess.exe"),
                };

                Cef.Initialize(cefSettings, performDependencyCheck: true, browserProcessHandler: null);

                initialized = true;
            }
        }

        public static void Shutdown()
        {
            if (initialized)
            {
                Cef.Shutdown();
            }
        }

        public void ExecuteScript(string script)
        {
            if (Browser != null)
            {
                var frame = Browser.GetMainFrame();
                frame.ExecuteJavaScriptAsync(script);
            }
            else
            {
                this.scriptQueue.Add(script);
            }
        }
    }

    public class BrowserErrorEventArgs : EventArgs
    {
        public CefErrorCode ErrorCode { get; private set; }
        public string ErrorText { get; private set; }
        public string Url { get; private set; }
        public BrowserErrorEventArgs(CefErrorCode errorCode, string errorText, string url)
        {
            this.ErrorCode = errorCode;
            this.ErrorText = errorText;
            this.Url = url;
        }
    }
    
    public class BrowserLoadEventArgs : EventArgs
    {
        public int HttpStatusCode { get; private set; }
        public string Url { get; private set; }
        public BrowserLoadEventArgs(int httpStatusCode, string url)
        {
            this.HttpStatusCode = httpStatusCode;
            this.Url = url;
        }
    }

    public class BrowserConsoleLogEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public string Source { get; private set; }
        public int Line { get; private set; }
        public BrowserConsoleLogEventArgs(string message, string source, int line)
        {
            this.Message = message;
            this.Source = source;
            this.Line = line;
        }
    }

    public class RendererFeatureRequestEventArgs : EventArgs
    {
        public string Request { get; private set; }
        public RendererFeatureRequestEventArgs(string request)
        {
            this.Request = request;
        }
    }
}
