namespace Multiverse
{
    public class DefaultMvConnection : IMvConnection
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public bool IsHost { get; set; }
        public bool IsLocal { get; set; }

        private bool Equals(DefaultMvConnection other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DefaultMvConnection) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}