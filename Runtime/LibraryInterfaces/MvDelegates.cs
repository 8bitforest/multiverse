using System;
using System.Collections.Generic;

namespace Multiverse.LibraryInterfaces
{
    public delegate void ServerByteMessageReceiver(int libId, ArraySegment<byte> message);
    public delegate void ClientByteMessageReceiver(ArraySegment<byte> message);
    public delegate void MessageReceiver<in T>(MvPlayer player, T message) where T : IMvMessage;

    public delegate void ErrorHandler(string message);
    
    public delegate void Connected();
    public delegate void Disconnected();

    public delegate void MatchesUpdated(IEnumerable<(int libId, byte[] data)> matches);

    public delegate void PlayerConnected(int libId);
    public delegate void PlayerDisconnected(int libId);
}