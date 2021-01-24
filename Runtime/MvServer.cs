using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Multiverse.LibraryInterfaces;
using Multiverse.Messaging;
using Reaction;

namespace Multiverse
{
    public class MvServer
    {
        public bool Connected { get; private set; } = true;
        public RxnDictionary<int, MvConnection> Clients => _server.Clients;

        public RxnEvent OnDisconnected { get; }

        private readonly IMvLibraryServer _server;
        private readonly MvBinaryWriter _writer;
        private readonly MvBinaryReader _reader;

        private static readonly Dictionary<byte, MvMessageReceiverManager> ReceiverManagers =
            new Dictionary<byte, MvMessageReceiverManager>();

        public MvServer(IMvLibraryServer server)
        {
            _server = server;
            _writer = new MvBinaryWriter();
            _reader = new MvBinaryReader();

            OnDisconnected = _server.OnDisconnected;
            OnDisconnected.OnInvoked(null, () => Connected = false);

            _server.SetMessageReceiver(ReceiveMessage);
        }

        public async Task Disconnect() => await _server.Disconnect();

        public static void AddMessageReceiver<T>(Action<T> receiver) where T : IMvNetworkMessage
        {
            AddMessageReceiver<T>((_, msg) => receiver(msg));
        }

        public static void AddMessageReceiver<T>(MessageReceiver<T> receiver) where T : IMvNetworkMessage
        {
            var id = MvMessageTypes.GetIdForType(typeof(T));
            if (!ReceiverManagers.ContainsKey(id))
                ReceiverManagers[id] = MvMessageReceiverManager.Create<T, MvServer>();

            MvMessageReceiverManager.AddReceiver<T, MvServer>(receiver);
        }

        public static void RemoveMessageReceiver<T>(MessageReceiver<T> receiver) where T : IMvNetworkMessage
        {
            MvMessageReceiverManager.RemoveReceiver<T, MvServer>(receiver);
        }

        public void SendMessageToClient<T>(MvConnection connection, T message, bool reliable = true)
            where T : IMvNetworkMessage
        {
            _writer.Reset();
            _writer.WriteByte(MvMessageTypes.GetIdForType(typeof(T)));
            _writer.Write(message);
            _server.SendMessageToClient(connection, _writer.GetData(), reliable);
        }

        public void SendMessageToAll<T>(T message, bool reliable = true) where T : IMvNetworkMessage
        {
            _writer.Reset();
            _writer.WriteByte(MvMessageTypes.GetIdForType(typeof(T)));
            _writer.Write(message);
            _server.SendMessageToAll(_writer.GetData(), reliable);
        }

        private void ReceiveMessage(MvConnection connection, ArraySegment<byte> message)
        {
            _reader.Reset(message);
            var type = _reader.ReadByte();
            if (ReceiverManagers.ContainsKey(type))
                ReceiverManagers[type].CallReceivers(connection, _reader);
        }

        internal static void ClearMessageReceivers()
        {
            foreach (var manager in ReceiverManagers.Values)
                manager.ClearReceivers();
        }
    }
}