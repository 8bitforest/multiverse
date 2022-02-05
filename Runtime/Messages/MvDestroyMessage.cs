namespace Multiverse.Messages
{
    internal struct MvDestroyMessage : IMvMessage
    {
        public uint InstanceId { get; set; }
    }
}