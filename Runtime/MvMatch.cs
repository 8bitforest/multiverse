namespace Multiverse
{
    public class MvMatch
    {
        public string Name { get; private set; }
        public string Id { get; private set; }
        public int MaxPlayers { get; private set; }

        public MvMatch(string name, string id, int maxPlayers)
        {
            Name = name;
            Id = id;
            MaxPlayers = maxPlayers;
        }

        private bool Equals(MvMatch other)
        {
            return Name == other.Name && Id == other.Id && MaxPlayers == other.MaxPlayers;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MvMatch) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ MaxPlayers;
                return hashCode;
            }
        }
    }
}