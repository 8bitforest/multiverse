namespace Multiverse.Common
{
    public interface IMvLibrary
    {
        string Name { get; }
        IMvLibraryMatchmaker GetMatchmaker();
    }
}