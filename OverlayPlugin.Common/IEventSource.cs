using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace RainbowMage.OverlayPlugin
{
    public interface IEventSource : IDisposable
    {
        /// <summary>
        /// ユーザーが設定したオーバーレイの名前を取得します。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// オーバーレイがログを出力したときに発生します。
        /// </summary>
        event EventHandler<LogEventArgs> OnLog;

        /// <summary>
        /// オーバーレイの更新を開始します。
        /// </summary>
        void Start();

        /// <summary>
        /// オーバーレイの更新を停止します。
        /// </summary>
        void Stop();
    }
}
