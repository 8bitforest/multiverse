namespace Multiverse
{
    public class MvMatch
    {
        public uint Id { get; }
        public string Name => Data.Name;
        public int MaxPlayers => Data.MaxPlayers;
        internal int LibId { get; }

        internal MultiverseMatchData Data { get; }
        internal IMvMatchData CustomData { get; }

        internal MvMatch(uint id, int libId, MultiverseMatchData data, IMvMatchData customData)
        {
            Id = id;
            Data = data;
            CustomData = customData;
            LibId = libId;
        }

        public T GetCustomData<T>() where T : IMvMatchData
        {
            return (T) CustomData;
        }

        public override string ToString()
        {
            return $"[{Id}] {Name}";
        }
    }

    public struct MultiverseMatchData : IMvMatchData
    {
        public string Name { get; set; }
        public int MaxPlayers { get; set; }
    }
}