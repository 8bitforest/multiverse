namespace Multiverse.Common
{
    public interface IMvMatch
    {
        public string Name { get; }
        public string Id { get; }
        public int MaxPlayers { get; }
    }
}