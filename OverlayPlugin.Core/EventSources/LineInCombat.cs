using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web.Services.Protocols;
using Advanced_Combat_Tracker;
using SharpCompress.Compressors.Xz;

namespace RainbowMage.OverlayPlugin.EventSources
{
    public class LineInCombat
    {
        public const uint LogFileLineID = 260;
        private ILogger logger;
        private readonly FFXIVRepository ffxiv;

        private Func<string, DateTime, bool> logWriter;

        public LineInCombat(TinyIoCContainer container)
        {
            logger = container.Resolve<ILogger>();
            ffxiv = container.Resolve<FFXIVRepository>();
            var customLogLines = container.Resolve<FFXIVCustomLogLines>();
            this.logWriter = customLogLines.RegisterCustomLogLine(new LogLineRegistryEntry()
            {
                Name = "InCombat",
                Source = "OverlayPlugin",
                ID = LogFileLineID,
                Version = 1,
            });
        }

        public void WriteLine(bool inACTCombat, bool inGameCombat)
        {
            var line = $"{(inACTCombat ? 1 : 0)}|{(inGameCombat ? 1 : 0)}";
            logWriter(line, ActGlobals.oFormActMain.LastEstimatedTime);
        }
    }
}
