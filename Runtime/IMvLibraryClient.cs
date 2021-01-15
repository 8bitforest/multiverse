using Reaction;

namespace Multiverse
{
    public interface IMvLibraryClient
    {
        RxnEvent OnDisconnected { get; }
        
        void Disconnect();

        // * DisconnectFromServer
        // * OnServerConnected
        // * OnServerDisconnected
        // * OnOtherClientJoined?
        // * OnOtherClientLeft?
    }
}