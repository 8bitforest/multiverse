using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Multiverse.LibraryInterfaces;
using Multiverse.Messages;
using Multiverse.Messaging;
using Multiverse.Serialization;
using Reaction;
using UnityEngine;

namespace Multiverse
{
    public class MvClient
    {
        public RxnValue<bool> Connected { get; }
        public MvPlayer LocalPlayer { get; private set; }
        public RxnDictionary<uint, MvPlayer> Players { get; }
        public IEnumerable<MvPlayer> OtherPlayers => Players.Values.Where(c => !Equals(c, LocalPlayer));

        private readonly IMvLibraryClient _client;
        private readonly MvBinaryWriter _writer;
        private readonly MvBinaryReader _reader;

        private static readonly Dictionary<byte, IMvMessageReceivers> Receivers =
            new Dictionary<byte, IMvMessageReceivers>();

        private readonly Dictionary<int, MvPlayer> _libIdPlayers = new Dictionary<int, MvPlayer>();

        public MvClient(IMvLibraryClient client)
        {
            _client = client;
            _writer = new MvBinaryWriter();
            _reader = new MvBinaryReader();

            Connected = new RxnValue<bool>();
            Players = new RxnDictionary<uint, MvPlayer>();

            _client.Disconnected = () => Connected.AsOwner.Set(false);
            _client.MessageReceiver = ReceiveMessage;

            AddMessageReceiver<MvExistingPlayersMessage>(ExistingPlayers);
            AddMessageReceiver<MvPlayerConnectedMessage>(PlayerConnected);
            AddMessageReceiver<MvPlayerDisconnectedMessage>(PlayerDisconnected);
        }

        public async Task Disconnect()
        {
            if (Connected)
                _client.Disconnect();
            await Connected.WaitUntil(false, Multiverse.Timeout);
        }

        public void AddMessageReceiver<T>(Action receiver) where T : IMvMessage
        {
            AddMessageReceiver<T>(_ => receiver());
        }

        public void AddMessageReceiver<T>(Action<T> receiver) where T : IMvMessage
        {
            var id = MvMessageTypes.GetIdForType(typeof(T));
            if (!Receivers.ContainsKey(id))
                Receivers[id] = new MvMessageReceivers<T>();

            Receivers[id].Add<T>((p, m) => receiver(m));
        }

        public void SendMessageToServer<T>(T message, bool reliable = true) where T : IMvMessage
        {
            Debug.Log($"[SR] >>> Client Send: {typeof(T)}");
            _writer.Reset();
            _writer.WriteByte(MvMessageTypes.GetIdForType(typeof(T)));
            _writer.Write(message);
            _client.SendMessageToServer(_writer.GetData(), reliable);
        }

        private void ReceiveMessage(ArraySegment<byte> message)
        {
            _reader.Reset(message);
            var type = _reader.ReadByte();
            Debug.Log($"[SR] <<< Client Recv: {MvMessageTypes.GetTypeForId(type)}");
            if (Receivers.ContainsKey(type))
                Receivers[type].Call(null, _reader);
        }

        public void ClearMessageReceivers()
        {
            foreach (var receivers in Receivers.Values)
                receivers.Clear();
        }

        private void ExistingPlayers(MvExistingPlayersMessage msg)
        {
            foreach (var player in msg.Players)
                PlayerConnected(player);

            Connected.AsOwner.Set(true);
        }

        private void PlayerConnected(MvPlayerConnectedMessage msg)
        {
            var player = new MvPlayer(msg.Id, msg.LibId, msg.IsHost, msg.IsLocal);
            if (player.IsLocal)
                LocalPlayer = player;

            _libIdPlayers[player.LibId] = player;
            Players.AsOwner[player.Id] = player;
        }

        private void PlayerDisconnected(MvPlayerDisconnectedMessage msg)
        {
            _libIdPlayers.Remove(Players[msg.Id].Value.LibId);
            Players.AsOwner.Remove(msg.Id);
        }
    }
}