using System;
using System.Threading.Tasks;
using Multiverse.Messaging;
using Reaction;

namespace Multiverse.LibraryInterfaces
{
    public interface IMvLibraryServer
    {
        RxnDictionary<int, MvConnection> Clients { get; }
        
        RxnEvent OnDisconnected { get; }

        Task Disconnect();
        void SetMessageReceiver(ByteMessageReceiver receiver);
        void SendMessageToClient(MvConnection connection, ArraySegment<byte> message, bool reliable);
        void SendMessageToAll(ArraySegment<byte> message, bool reliable);
    }
}