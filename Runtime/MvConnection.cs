namespace Multiverse
{
    // TODO: Find a better name for this
    public class MvConnection
    {
        public string Name { get; private set; }
        public int Id { get; private set; }
        public bool IsHost { get; private set; }
        public bool IsLocal { get; private set; }

        public MvConnection(string name, int id, bool isHost, bool isLocal)
        {
            Name = name;
            Id = id;
            IsHost = isHost;
            IsLocal = isLocal;
        }

        private bool Equals(MvConnection other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MvConnection) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}