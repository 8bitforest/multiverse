namespace Multiverse
{
    public class DefaultMvMatch : IMvMatch
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public int MaxPlayers { get; set; }

        protected bool Equals(DefaultMvMatch other)
        {
            return Name == other.Name && Id == other.Id && MaxPlayers == other.MaxPlayers;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DefaultMvMatch) obj);
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