namespace Multiverse.LibraryInterfaces
{
    public interface IMvLibrary
    {
        IMvLibraryServer GetServer();
        IMvLibraryClient GetClient();
        IMvLibraryMatchmaker GetMatchmaker();
        void CleanupAfterDisconnect();
        void SetTimeout(float seconds);
    }
}