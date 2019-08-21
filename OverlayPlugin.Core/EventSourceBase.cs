using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json.Linq;

namespace RainbowMage.OverlayPlugin
{
    public abstract class EventSourceBase : IEventSource
    {
        public string Name { get; protected set; }

        public event EventHandler<LogEventArgs> OnLog;
        protected Timer timer;
        protected ILogger logger;

        public EventSourceBase(ILogger logger)
        {
            this.logger = logger;

            timer = new Timer(1000);
            timer.Elapsed += (o, e) =>
            {
                try
                {
                    Update();
                }
                catch (Exception ex)
                {
                    Log(LogLevel.Error, "Update: {0}", ex);
                }
            };
        }

        public abstract System.Windows.Forms.Control CreateConfigControl();

        public abstract void LoadConfig(IPluginConfig config);

        protected void Log(LogLevel level, string message, params object[] args)
        {
            logger.Log(level, message, args);
        }

        public virtual void Dispose()
        {
            timer?.Dispose();
        }

        public virtual void Start()
        {
            timer.Start();
        }

        public virtual void Stop()
        {
            timer.Stop();
        }

        abstract protected void Update();

        protected void RegisterEventTypes(List<string> types)
        {
            EventDispatcher.RegisterEventTypes(types);
        }

        protected void RegisterEventHandler(string name, Func<JObject, JObject> handler)
        {
            EventDispatcher.RegisterHandler(name, handler);
        }

        protected void DispatchEvent(JObject e)
        {
            EventDispatcher.DispatchEvent(e);
        }
    }
}
