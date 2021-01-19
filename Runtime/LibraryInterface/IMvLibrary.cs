namespace Multiverse
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