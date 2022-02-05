using System;

namespace Multiverse.LibraryInterfaces
{
    public interface IMvLibraryServer
    {
        Disconnected Disconnected { set; }
        ServerByteMessageReceiver MessageReceiver { set; }
        PlayerConnected PlayerConnected { set; }
        PlayerDisconnected PlayerDisconnected { set; }

        void Disconnect();
        
        void SendMessageToPlayer(int libId, ArraySegment<byte> message, bool reliable);
        void SendMessageToAll(ArraySegment<byte> message, bool reliable);
    }
}