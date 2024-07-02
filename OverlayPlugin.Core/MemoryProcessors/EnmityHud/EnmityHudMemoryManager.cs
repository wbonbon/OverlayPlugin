﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.EnmityHud
{
    public interface IEnmityHudMemory : IVersionedMemory
    {
        List<EnmityHudEntry> GetEnmityHudEntries();
    }

    public class EnmityHudMemoryManager : IEnmityHudMemory
    {
        private readonly TinyIoCContainer container;
        private readonly FFXIVRepository repository;
        private IEnmityHudMemory memory = null;

        public EnmityHudMemoryManager(TinyIoCContainer container)
        {
            this.container = container;
            container.Register<IEnmityHudMemory60, EnmityHudMemory60>();
            container.Register<IEnmityHudMemory62, EnmityHudMemory62>();
            container.Register<IEnmityHudMemory70, EnmityHudMemory70>();
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
            List<IEnmityHudMemory> candidates = new List<IEnmityHudMemory>();
            candidates.Add(container.Resolve<IEnmityHudMemory60>());
            candidates.Add(container.Resolve<IEnmityHudMemory62>());
            candidates.Add(container.Resolve<IEnmityHudMemory70>());
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

        public List<EnmityHudEntry> GetEnmityHudEntries()
        {
            if (!IsValid())
            {
                return null;
            }
            return memory.GetEnmityHudEntries();
        }
    }
}
