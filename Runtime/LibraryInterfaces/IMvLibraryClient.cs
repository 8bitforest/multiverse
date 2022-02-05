using System;

namespace Multiverse.LibraryInterfaces
{
    public interface IMvLibraryClient
    {
        Disconnected Disconnected { set; }
        ClientByteMessageReceiver MessageReceiver { get; set; }

        void Disconnect();
        void SendMessageToServer(ArraySegment<byte> message, bool reliable);
    }
}