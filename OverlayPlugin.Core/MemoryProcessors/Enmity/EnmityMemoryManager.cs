using System.Collections.Generic;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.Enmity
{
    public interface IEnmityMemory
    {
        List<EnmityEntry> GetEnmityEntryList(List<Combatant.Combatant> combatantList);

        bool IsValid();
    }

    public class EnmityMemoryManager : IEnmityMemory
    {
        private readonly TinyIoCContainer container;
        private readonly FFXIVRepository repository;
        private IEnmityMemory memory = null;

        public EnmityMemoryManager(TinyIoCContainer container)
        {
            this.container = container;
            container.Register<IEnmityMemory60, EnmityMemory60>();
            repository = container.Resolve<FFXIVRepository>();
        }

        private void FindMemory()
        {
            List<IEnmityMemory> candidates = new List<IEnmityMemory>();
            candidates.Add(container.Resolve<IEnmityMemory60>());

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


        public List<EnmityEntry> GetEnmityEntryList(List<Combatant.Combatant> combatantList)
        {
            if (!IsValid())
            {
                return null;
            }
            return memory.GetEnmityEntryList(combatantList);
        }
    }
}
