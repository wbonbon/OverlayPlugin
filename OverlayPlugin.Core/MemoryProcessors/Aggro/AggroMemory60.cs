namespace RainbowMage.OverlayPlugin.MemoryProcessors.Aggro
{
    interface IAggroMemory60 : IAggroMemory {}

    class AggroMemory60 : AggroMemory, IAggroMemory60
    {
        private const int aggroEnmityOffset = -2336;

        // Aggro uses the same signature as Enmity
        public AggroMemory60(TinyIoCContainer container)
            : base(container, Enmity.EnmityMemory60.enmitySignature, aggroEnmityOffset)
        { }
    }
}
