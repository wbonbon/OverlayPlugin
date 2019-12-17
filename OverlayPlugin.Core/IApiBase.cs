using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainbowMage.OverlayPlugin
{
    interface IApiBase : IEventReceiver
    {
        string Name { get; }

        void SetAcceptFocus(bool accept);

        void OverlayMessage(string msg);

        void InitModernAPI();
    }
}
