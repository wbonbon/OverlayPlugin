using System;
using System.Collections.Generic;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.InCombat
{
    interface IInCombatMemory60 : IInCombatMemory {}

    class InCombatMemory60 : InCombatMemory, IInCombatMemory60
    {
        private const string inCombatSignature = "84c07425450fb6c7488d0d";
        private const int inCombatSignatureBaseOffset = 0;
        private const int inCombatSignatureOffsetOffset = 5;
        private uint loggedScanErrors = 0;

        public InCombatMemory60(TinyIoCContainer container)
            : base(container, inCombatSignature, 0, 0)
        { }

        protected override bool GetPointerAddress()
        {
            if (!memory.IsValid())
                return false;

            bool success = true;

            List<string> fail = new List<string>();

            // The in combat address is set from a combination of two values, a base address and an offset.
            // They are found adjacent to the same signature, but at different offsets.
            var baseList = memory.SigScan(inCombatSignature, inCombatSignatureBaseOffset, true);
            // SigScan returns pointers, but the offset is a 32-bit immediate value.  Do not use RIP.
            var offsetList = memory.SigScan(inCombatSignature, inCombatSignatureOffsetOffset, false);
            if (baseList != null && baseList.Count > 0 && offsetList != null && offsetList.Count > 0)
            {
                var baseAddress = baseList[0];
                var offset = (int)(((UInt64)offsetList[0]) & 0xFFFFFFFF);
                inCombatAddress = IntPtr.Add(baseAddress, offset);
            }
            else
            {
                inCombatAddress = IntPtr.Zero;
                fail.Add(nameof(inCombatAddress));
                success = false;
            }

            logger.Log(LogLevel.Debug, "targetAddress: 0x{0:X}", inCombatAddress.ToInt64());

            if (!success)
            {
                if (loggedScanErrors < 10)
                {
                    logger.Log(LogLevel.Error, $"Failed to find in combat memory via {GetType().Name}: {string.Join(", ", fail)}.");
                    loggedScanErrors++;

                    if (loggedScanErrors == 10)
                    {
                        logger.Log(LogLevel.Error, "Further in combat memory errors won't be logged.");
                    }
                }
            }
            else
            {
                logger.Log(LogLevel.Info, $"Found in combat memory via {GetType().Name}.");
                loggedScanErrors = 0;
            }

            return success;
        }
    }
}
