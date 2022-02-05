using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Multiverse.LibraryInterfaces;
using Multiverse.Messages;
using Multiverse.Messaging;
using Multiverse.Serialization;
using Multiverse.Utils;
using Reaction;
using UnityEngine;

namespace Multiverse
{
    public class MvServer
    {
        public RxnValue<bool> Connected { get; }
        public RxnDictionary<uint, MvPlayer> Players { get; }

        private readonly IMvLibraryServer _server;
        private readonly MvBinaryWriter _writer;
        private readonly MvBinaryReader _reader;

        private static readonly Dictionary<byte, IMvMessageReceivers> Receivers =
            new Dictionary<byte, IMvMessageReceivers>();

        private readonly Dictionary<int, MvPlayer> _libIdPlayers = new Dictionary<int, MvPlayer>();

        public MvServer(IMvLibraryServer server)
        {
            _server = server;
            _writer = new MvBinaryWriter();
            _reader = new MvBinaryReader();

            Connected = new RxnValue<bool>();
            Players = new RxnDictionary<uint, MvPlayer>();

            _server.Disconnected = () => Connected.AsOwner.Set(false);
            _server.MessageReceiver = ReceiveMessage;
            _server.PlayerConnected = PlayerConnected;
            _server.PlayerDisconnected = PlayerDisconnected;

            Connected.AsOwner.Set(true);
        }

        public async Task Disconnect()
        {
            if (Connected)
                _server.Disconnect();
            await Connected.WaitUntil(false, Multiverse.Timeout);
        }
        
        public void AddMessageReceiver<T>(Action receiver) where T : IMvMessage
        {
            AddMessageReceiver<T>((p, m) => receiver());
        }

        public void AddMessageReceiver<T>(Action<T> receiver) where T : IMvMessage
        {
            AddMessageReceiver<T>((_, msg) => receiver(msg));
        }

        public void AddMessageReceiver<T>(MessageReceiver<T> receiver) where T : IMvMessage
        {
            var id = MvMessageTypes.GetIdForType(typeof(T));
            if (!Receivers.ContainsKey(id))
                Receivers[id] = new MvMessageReceivers<T>();

            Receivers[id].Add(receiver);
        }

        public void SendMessageToPlayer<T>(MvPlayer player, T message, bool reliable = true) where T : IMvMessage
        {
            Debug.Log($"[SR] >>> Server Send ({player.Id}): {typeof(T)}");
            _writer.Reset();
            _writer.WriteByte(MvMessageTypes.GetIdForType(typeof(T)));
            _writer.Write(message);
            _server.SendMessageToPlayer(player.LibId, _writer.GetData(), reliable);
        }

        public void SendMessageToAll<T>(T message, bool reliable = true) where T : IMvMessage
        {
            Debug.Log($"[SR] >>> Server Send All: {typeof(T)}");
            _writer.Reset();
            _writer.WriteByte(MvMessageTypes.GetIdForType(typeof(T)));
            _writer.Write(message);
            _server.SendMessageToAll(_writer.GetData(), reliable);
        }

        private void ReceiveMessage(int libId, ArraySegment<byte> message)
        {
            _reader.Reset(message);
            var type = _reader.ReadByte();
            Debug.Log($"[SR] <<< Server Recv: {MvMessageTypes.GetTypeForId(type)}");
            if (Receivers.ContainsKey(type))
                Receivers[type].Call(_libIdPlayers[libId], _reader);
        }

        public void ClearMessageReceivers()
        {
            foreach (var receivers in Receivers.Values)
                receivers.Clear();
        }

        private void PlayerConnected(int libId)
        {
            PlayerConnected(new MvPlayer(MvIdManager.AllocatePlayerId(), libId, false, false));
        }

        internal void PlayerConnected(MvPlayer newPlayer)
        {
            var libId = newPlayer.LibId;

            // Send the new player an array of all existing players
            // Also send the new player their id with IsLocal = true
            SendMessageToPlayer(newPlayer, new MvExistingPlayersMessage
            {
                Players = Players.Values.Select(p => new MvPlayerConnectedMessage
                {
                    Id = p.Id,
                    IsHost = p.IsHost,
                    IsLocal = false,
                    LibId = p.LibId
                }).Append(
                    new MvPlayerConnectedMessage
                    {
                        Id = newPlayer.Id,
                        IsHost = newPlayer.IsHost,
                        IsLocal = true,
                        LibId = libId
                    }).ToArray()
            });

            // Send the new player to all the existing players
            // TODO: Use SendMessageToAll?
            foreach (var player in Players.Values)
            {
                SendMessageToPlayer(player, new MvPlayerConnectedMessage
                {
                    Id = newPlayer.Id,
                    IsHost = newPlayer.IsHost,
                    IsLocal = false,
                    LibId = libId
                });
            }

            _libIdPlayers[libId] = newPlayer;
            Players.AsOwner[newPlayer.Id] = newPlayer;
        }

        private void PlayerDisconnected(int libId)
        {
            var oldPlayer = _libIdPlayers[libId];
            _libIdPlayers.Remove(libId);
            Players.AsOwner.Remove(oldPlayer.Id);

            // Send the disconnected player to all the existing players
            // TODO: Use SendMessageToAll?
            foreach (var player in Players.Values)
            {
                SendMessageToPlayer(player, new MvPlayerDisconnectedMessage
                {
                    Id = oldPlayer.Id
                });
            }
        }
    }
}