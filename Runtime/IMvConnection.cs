namespace Multiverse
{
    public interface IMvConnection
    {
        public string Name { get; }
        public int Id { get; }
        public bool IsHost { get; }
        public bool IsLocal { get; }
    }
}