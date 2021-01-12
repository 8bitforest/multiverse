namespace Multiverse.Common
{
    public class DefaultMvMatch : IMvMatch
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public int MaxPlayers { get; set; }
    }
}