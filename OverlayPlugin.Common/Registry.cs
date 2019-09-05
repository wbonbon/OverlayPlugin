using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainbowMage.OverlayPlugin
{
    public class Registry
    {
        public static TinyIoCContainer Container { get; private set; }
        private static List<Type> _overlays;
        private static List<IEventSource> _eventSources;

        public static IEnumerable<Type> Overlays
        {
            get
            {
                return _overlays;
            }
        }

        public static IEnumerable<IEventSource> EventSources
        {
            get
            {
                return _eventSources;
            }
        }

        public static event EventHandler AddonRegistered;

        public static void Init()
        {
            Container = new TinyIoCContainer();
            _overlays = new List<Type>();
            _eventSources = new List<IEventSource>();
        }

        public static void Clear()
        {
            Container.Dispose();
            Container = null;

            _overlays = null;
            _eventSources = null;
        }

        public static void RegisterOverlay<T>()
            where T: class, IOverlay
        {
            _overlays.Add(typeof(T));
            Container.Register<T>();

            AddonRegistered?.Invoke(null, new EventArgs());
        }

        public static void UnregisterOverlay<T>()
            where T: class, IOverlay
        {
            _overlays.Remove(typeof(T));
        }

        public static void RegisterEventSource(IEventSource source)
        {
            Container.BuildUp(source);
            _eventSources.Add(source);

            AddonRegistered?.Invoke(null, new EventArgs());
        }

        public static void RegisterEventSource<T>()
            where T: class, IEventSource
        {
            var esType = typeof(T);
            Container.Register(esType);

            RegisterEventSource((T) Container.Resolve(esType));
        }

        #region Shortcuts
        public static T Resolve<T>()
            where T : class
        {
            return Container.Resolve<T>();
        }

        public static TinyIoCContainer.RegisterOptions Register<T>()
            where T : class
        {
            return Container.Register<T>();
        }

        public static TinyIoCContainer.RegisterOptions Register<TInterface, Tclass>()
            where TInterface : class
            where Tclass : class, TInterface
        {
            return Container.Register<TInterface, Tclass>();
        }

        public static TinyIoCContainer.RegisterOptions Register<T>(T obj)
            where T : class
        {
            return Container.Register<T>(obj);
        }
        #endregion
    }
}
