namespace Multiverse
{
    // TODO: Find a better name for this
    // This is NOT a connection. It is a reference to another "player"
    public class MvConnection
    {
        public string Name { get; }
        public int Id { get; }
        public bool IsHost { get; }
        public bool IsLocal { get; }

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