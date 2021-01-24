using System;
using System.Collections.Generic;

namespace Multiverse.Messaging
{
    internal class MvMessageReceiverManager
    {
        // TODO: Check if <,MvClient> == <,MvServer>!!!
        private static class MvMessageReceivers<TMessage, TGroup> where TMessage : IMvNetworkMessage
        {
            public static HashSet<MessageReceiver<TMessage>> Receivers { get; } =
                new HashSet<MessageReceiver<TMessage>>();
        }

        private Action<MvConnection, MvBinaryReader> _callReceivers;
        private Action _clearReceivers;

        private MvMessageReceiverManager() { }

        public static MvMessageReceiverManager Create<TMessage, TGroup>() where TMessage : IMvNetworkMessage
        {
            return new MvMessageReceiverManager
            {
                _callReceivers = (connection, reader) =>
                {
                    var message = reader.Read<TMessage>();
                    foreach (var r in MvMessageReceivers<TMessage, TGroup>.Receivers)
                        r(connection, message);
                },
                _clearReceivers = () => MvMessageReceivers<TMessage, TGroup>.Receivers.Clear()
            };
        }

        public void CallReceivers(MvConnection connection, MvBinaryReader reader)
        {
            _callReceivers(connection, reader);
        }

        public void ClearReceivers()
        {
            _clearReceivers();
        }

        public static void AddReceiver<TMessage, TGroup>(MessageReceiver<TMessage> receiver) where TMessage : IMvNetworkMessage
        {
            MvMessageReceivers<TMessage, TGroup>.Receivers.Add(receiver);
        }
        
        public static void RemoveReceiver<TMessage, TGroup>(MessageReceiver<TMessage> receiver) where TMessage : IMvNetworkMessage
        {
            MvMessageReceivers<TMessage, TGroup>.Receivers.Remove(receiver);
        }
    }
}