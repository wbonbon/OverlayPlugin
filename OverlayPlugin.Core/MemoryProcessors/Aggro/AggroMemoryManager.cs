using System;
using System.Collections.Generic;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.Aggro
{
    public interface IAggroMemory
    {
        List<AggroEntry> GetAggroList(List<Combatant.Combatant> combatantList);

        bool IsValid();
    }

    public class AggroMemoryManager : IAggroMemory
    {
        private readonly TinyIoCContainer container;
        private readonly FFXIVRepository repository;
        private IAggroMemory memory = null;

        public AggroMemoryManager(TinyIoCContainer container)
        {
            this.container = container;
            container.Register<IAggroMemory60, AggroMemory60>();
            repository = container.Resolve<FFXIVRepository>();
        }

        private void FindMemory()
        {
            List<IAggroMemory> candidates = new List<IAggroMemory>();
            candidates.Add(container.Resolve<IAggroMemory60>());

            foreach (var c in candidates)
            {
                if (c.IsValid())
                {
                    memory = c;
                    break;
                }
            }
        }

        public bool IsValid()
        {
            if (memory == null)
            {
                FindMemory();
            }
            if (memory == null || !memory.IsValid())
            {
                return false;
            }
            return true;
        }


        public List<AggroEntry> GetAggroList(List<Combatant.Combatant> combatantList)
        {
            if (!IsValid())
            {
                return null;
            }
            return memory.GetAggroList(combatantList);
        }
    }
}
