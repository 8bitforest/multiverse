using Multiverse.Serialization;

namespace Multiverse
{
    public interface IMvMatchData : IMvSerializable { }
    public struct NoMatchData : IMvMatchData { }
}