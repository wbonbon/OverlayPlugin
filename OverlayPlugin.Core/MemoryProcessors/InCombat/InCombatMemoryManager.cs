﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.InCombat
{
    public interface IInCombatMemory : IVersionedMemory
    {
        bool GetInCombat();
    }

    class InCombatMemoryManager : IInCombatMemory
    {
        private readonly TinyIoCContainer container;
        private readonly FFXIVRepository repository;
        private IInCombatMemory memory = null;

        public InCombatMemoryManager(TinyIoCContainer container)
        {
            this.container = container;
            container.Register<IInCombatMemory61, InCombatMemory61>();
            container.Register<IInCombatMemory70, InCombatMemory70>();
            repository = container.Resolve<FFXIVRepository>();

            var memory = container.Resolve<FFXIVMemory>();
            memory.RegisterOnProcessChangeHandler(FindMemory);
        }

        private void FindMemory(object sender, Process p)
        {
            memory = null;
            if (p == null)
            {
                return;
            }
            ScanPointers();
        }

        public void ScanPointers()
        {
            List<IInCombatMemory> candidates = new List<IInCombatMemory>();
            candidates.Add(container.Resolve<IInCombatMemory70>());
            candidates.Add(container.Resolve<IInCombatMemory61>());
            memory = FFXIVMemory.FindCandidate(candidates, repository.GetMachinaRegion());
        }

        public bool IsValid()
        {
            if (memory == null || !memory.IsValid())
            {
                return false;
            }
            return true;
        }

        public Version GetVersion()
        {
            if (!IsValid())
                return null;
            return memory.GetVersion();
        }

        public bool GetInCombat()
        {
            if (!IsValid())
            {
                return false;
            }
            return memory.GetInCombat();
        }
    }
}
