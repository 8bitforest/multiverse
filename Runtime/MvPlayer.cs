namespace Multiverse
{
    public class MvPlayer
    {
        public uint Id { get; }
        public bool IsHost { get; }
        public bool IsLocal { get; }
        internal int LibId { get; }

        internal MvPlayer(uint id, int libId, bool isHost, bool isLocal)
        {
            Id = id;
            IsHost = isHost;
            IsLocal = isLocal;
            LibId = libId;
        }

        private bool Equals(MvPlayer other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MvPlayer) obj);
        }

        public override int GetHashCode()
        {
            return (int) Id;
        }
        
        public static bool operator==(MvPlayer a, MvPlayer b)
        {
            return a?.Id == b?.Id;
        }

        public static bool operator !=(MvPlayer a, MvPlayer b)
        {
            return !(a == b);
        }
    }
}