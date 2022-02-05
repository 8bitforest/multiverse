namespace Multiverse.LibraryInterfaces
{
    public interface IMvLibrary
    {
        IMvLibraryHost GetHost();
        IMvLibraryServer GetServer();
        IMvLibraryClient GetClient();
        IMvLibraryMatchmaker GetMatchmaker();
        void CleanupAfterDisconnect();
        void SetTimeout(float seconds);
    }
}