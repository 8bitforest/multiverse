using System;

namespace Multiverse.Messaging
{
    // Called by the backend library
    public delegate void ByteMessageReceiver(MvConnection connection, ArraySegment<byte> message);
    
    // Called by Multiverse after reading the message
    public delegate void MessageReceiver<in T>(MvConnection connection, T message) where T : IMvNetworkMessage;
    
    // A separate closure of this is created for each message type, calls MessageReceivers
    // internal delegate void MessageDispatcher(MvConnection connection, MvBinaryReader reader);
    
    // This clears all receivers for a specific type
    // internal delegate void ClearReceivers();
}