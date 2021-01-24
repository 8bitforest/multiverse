using System;
using System.Threading.Tasks;
using Multiverse.Messaging;
using Reaction;

namespace Multiverse.LibraryInterfaces
{
    public interface IMvLibraryClient
    {
        MvConnection LocalConnection { get; }
        RxnDictionary<int, MvConnection> Connections { get; }

        RxnEvent OnDisconnected { get; }

        Task Disconnect();
        void SetMessageReceiver(ByteMessageReceiver receiver);
        void SendMessageToServer(ArraySegment<byte> message, bool reliable);
    }
}