using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainbowMage.OverlayPlugin
{
    public class Registry
    {
        private static TinyIoCContainer _container;

        public static TinyIoCContainer Container
        {
            get
            {
                if (_container == null)
                    _container = new TinyIoCContainer();

                return _container;
            }
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

        public static TinyIoCContainer.RegisterOptions Register<T>(T obj)
            where T : class
        {
            return Container.Register<T>(obj);
        }
        #endregion
    }
}
