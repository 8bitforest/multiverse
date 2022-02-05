namespace Multiverse.Messages
{
    internal struct MvPlayerConnectedMessage : IMvMessage
    {
        public uint Id { get; set; }
        public bool IsHost { get; set; }
        public bool IsLocal { get; set; }
        public int LibId { get; set; }
    }
}