using System.Collections.Generic;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.EnmityHud
{
    public interface IEnmityHudMemory
    {
        List<EnmityHudEntry> GetEnmityHudEntries();

        bool IsValid();
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
            repository = container.Resolve<FFXIVRepository>();
        }

        private void FindMemory()
        {
            List<IEnmityHudMemory> candidates = new List<IEnmityHudMemory>();
            // For CN/KR, try the lang-specific candidate first, then fall back to intl
            if (
                repository.GetLanguage() == FFXIV_ACT_Plugin.Common.Language.Chinese ||
                repository.GetLanguage() == FFXIV_ACT_Plugin.Common.Language.Korean)
            {
                candidates.Add(container.Resolve<IEnmityHudMemory60>());
            }
            candidates.Add(container.Resolve<IEnmityHudMemory62>());

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
