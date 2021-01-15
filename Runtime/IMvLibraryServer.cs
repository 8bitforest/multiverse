using Reaction;

namespace Multiverse
{
    public interface IMvLibraryServer
    {
        RxnEvent OnDisconnected { get; }
        
        void Disconnect();
        // * OnClientConnected
        // * OnClientDisconnected
        // * Kick Client?
        // * StopServer
        // * SpawnObject
    }
}