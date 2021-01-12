namespace Multiverse.Common
{
    public class DefaultMvMatch : IMvMatch
    {
        public string Name { get; }
        public string Id { get; }

        public DefaultMvMatch(string id) : this(id, id) { }

        public DefaultMvMatch(string name, string id)
        {
            Name = name;
            Id = id;
        }
    }
}