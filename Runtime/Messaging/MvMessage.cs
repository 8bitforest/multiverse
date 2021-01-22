namespace Multiverse.Messaging
{
    public struct MvMessage
    {
        public byte TypeId { get; set; }
        public byte[] Data { get; set; }
    }
}