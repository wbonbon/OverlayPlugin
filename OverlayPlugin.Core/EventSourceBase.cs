using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace RainbowMage.OverlayPlugin
{
    public abstract class EventSourceBase : IEventSource
    {
        public string Name { get; protected set; }

        protected Timer timer;
        protected ILogger logger;

        public EventSourceBase(ILogger logger)
        {
            this.logger = logger;

            timer = new Timer(UpdateWrapper, null, Timeout.Infinite, 1000);
        }

        protected void UpdateWrapper(object state)
        {
            try
            {
                Update();
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, "Update: {0}", ex);
            }
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
            timer.Change(0, 1000);
        }

        public virtual void Stop()
        {
            timer.Change(-1, -1);
        }

        abstract protected void Update();

        protected void RegisterEventTypes(List<string> types)
        {
            EventDispatcher.RegisterEventTypes(types);
        }

        protected void RegisterEventHandler(string name, Func<JObject, JToken> handler)
        {
            EventDispatcher.RegisterHandler(name, handler);
        }

        protected void DispatchEvent(JObject e)
        {
            EventDispatcher.DispatchEvent(e);
        }
    }
}
