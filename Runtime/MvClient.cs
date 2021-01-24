using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Multiverse.LibraryInterfaces;
using Multiverse.Messaging;
using Reaction;

namespace Multiverse
{
    public class MvClient
    {
        public bool Connected { get; private set; } = true;
        public MvConnection LocalConnection => _client.LocalConnection;
        public RxnDictionary<int, MvConnection> Connections => _client.Connections;

        public IEnumerable<MvConnection> OtherConnections => Connections.Values
            .Select(v => v.Value)
            .Where(c => !Equals(c, LocalConnection));

        public RxnEvent OnDisconnected { get; }

        private readonly IMvLibraryClient _client;
        private readonly MvBinaryWriter _writer;
        private readonly MvBinaryReader _reader;

        private static readonly Dictionary<byte, MvMessageReceiverManager> ReceiverManagers =
            new Dictionary<byte, MvMessageReceiverManager>();

        public MvClient(IMvLibraryClient client)
        {
            _client = client;
            _writer = new MvBinaryWriter();
            _reader = new MvBinaryReader();

            OnDisconnected = _client.OnDisconnected;
            OnDisconnected.OnInvoked(null, () => Connected = false);

            _client.SetMessageReceiver(ReceiveMessage);
        }

        public async Task Disconnect() => await _client.Disconnect();

        public static void AddMessageReceiver<T>(Action<T> receiver) where T : IMvNetworkMessage
        {
            AddMessageReceiver<T>((_, msg) => receiver(msg));
        }

        public static void AddMessageReceiver<T>(MessageReceiver<T> receiver) where T : IMvNetworkMessage
        {
            var id = MvMessageTypes.GetIdForType(typeof(T));
            if (!ReceiverManagers.ContainsKey(id))
                ReceiverManagers[id] = MvMessageReceiverManager.Create<T, MvClient>();

            MvMessageReceiverManager.AddReceiver<T, MvClient>(receiver);
        }

        public static void RemoveMessageReceiver<T>(MessageReceiver<T> receiver) where T : IMvNetworkMessage
        {
            MvMessageReceiverManager.RemoveReceiver<T, MvClient>(receiver);
        }

        public void SendMessageToServer<T>(T message, bool reliable = true) where T : IMvNetworkMessage
        {
            _writer.Reset();
            _writer.WriteByte(MvMessageTypes.GetIdForType(typeof(T)));
            _writer.Write(message);
            _client.SendMessageToServer(_writer.GetData(), reliable);
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