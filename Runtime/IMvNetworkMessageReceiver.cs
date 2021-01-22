using Multiverse.Messaging;

namespace Multiverse
{
    public interface IMvNetworkMessageReceiver
    {
        void ReceiveMessage(MvMessage message);
    }
}