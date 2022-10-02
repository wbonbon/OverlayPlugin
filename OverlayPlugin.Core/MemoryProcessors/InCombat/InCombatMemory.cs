using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.InCombat
{
    public abstract class InCombatMemory {
        protected FFXIVMemory memory;
        protected ILogger logger;

        protected IntPtr inCombatAddress = IntPtr.Zero;

        private string inCombatSignature;

        private int inCombatSignatureOffset;
        private int inCombatRIPOffset;

        public InCombatMemory(TinyIoCContainer container, string inCombatSignature, int inCombatSignatureOffset, int inCombatRIPOffset)
        {
            this.inCombatSignature = inCombatSignature;
            this.inCombatSignatureOffset = inCombatSignatureOffset;
            this.inCombatRIPOffset = inCombatRIPOffset;
            logger = container.Resolve<ILogger>();
            memory = container.Resolve<FFXIVMemory>();
            memory.RegisterOnProcessChangeHandler(ResetPointers);
        }

        private void ResetPointers(object sender, Process p)
        {
            inCombatAddress = IntPtr.Zero;
            if (p != null)
                GetPointerAddress();
        }

        private bool HasValidPointers()
        {
            if (inCombatAddress == IntPtr.Zero)
                return false;
            return true;
        }

        public bool IsValid()
        {
            if (!memory.IsValid())
                return false;

            if (!HasValidPointers())
                return false;

            return true;
        }

        protected virtual bool GetPointerAddress()
        {
            if (!memory.IsValid())
                return false;

            List<string> fail = new List<string>();

            List<IntPtr> list = memory.SigScan(inCombatSignature, inCombatSignatureOffset, true, inCombatRIPOffset);

            if (list != null && list.Count > 0)
            {
                inCombatAddress = list[0];
            }
            else
            {
                inCombatAddress = IntPtr.Zero;
                fail.Add(nameof(inCombatAddress));
            }


            logger.Log(LogLevel.Debug, "inCombatAddress: 0x{0:X}", inCombatAddress.ToInt64());

            if (fail.Count == 0)
            {
                logger.Log(LogLevel.Info, $"Found in combat memory via {GetType().Name}.");
                return true;
            }

            logger.Log(LogLevel.Error, $"Failed to find in combat memory via {GetType().Name}: {string.Join(", ", fail)}.");
            return false;
        }

        public bool GetInCombat()
        {
            if (!IsValid())
                return false;
            byte[] bytes = memory.Read8(inCombatAddress, 1);
            return bytes[0] != 0;
        }
    }
}
