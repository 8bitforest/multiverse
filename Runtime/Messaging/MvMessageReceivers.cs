using System.Collections.Generic;
using System.Linq;
using Multiverse.LibraryInterfaces;
using Multiverse.Serialization;
using UnityEngine;

namespace Multiverse.Messaging
{
    internal interface IMvMessageReceivers
    {
        public void Add<T>(MessageReceiver<T> receiver) where T : IMvMessage;
        public void Remove<T>(MessageReceiver<T> receiver) where T : IMvMessage;
        public void Clear();
        public void Call(MvPlayer player, MvBinaryReader reader);
    }

    internal class MvMessageReceivers<TMessage> : IMvMessageReceivers where TMessage : IMvMessage
    {
        private static HashSet<MessageReceiver<TMessage>> _receivers = new HashSet<MessageReceiver<TMessage>>();

        public void Add<T>(MessageReceiver<T> receiver) where T : IMvMessage
        {
            Debug.Log($"Registering {typeof(TMessage)} receiver {receiver}");
            if (receiver is MessageReceiver<TMessage> messageReceiver)
                _receivers = new HashSet<MessageReceiver<TMessage>>(_receivers.Append(messageReceiver));
            else
                throw new MvException($"Cannot add receiver of type {typeof(T)} to type {typeof(TMessage)}!");
        }

        public void Remove<T>(MessageReceiver<T> receiver) where T : IMvMessage
        {
            Debug.Log($"Removing {typeof(TMessage)} receiver {receiver}");
            if (receiver is MessageReceiver<TMessage> messageReceiver)
                _receivers = new HashSet<MessageReceiver<TMessage>>(_receivers.Where(r => r != messageReceiver));
            else
                throw new MvException($"Cannot add receiver of type {typeof(T)} to type {typeof(TMessage)}!");
        }

        public void Clear()
        {
            Debug.Log($"Clearing {typeof(TMessage)} message receivers");
            _receivers = new HashSet<MessageReceiver<TMessage>>();
        }

        public void Call(MvPlayer player, MvBinaryReader reader)
        {
            Debug.Log($"Calling {typeof(TMessage)} message receivers");
            var message = reader.Read<TMessage>();
            foreach (var r in _receivers)
                r(player, message);
        }
    }
}