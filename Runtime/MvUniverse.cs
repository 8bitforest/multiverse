using System.Collections.Generic;
using System.Linq;
using Multiverse.Messages;
using Multiverse.Utils;
using UnityEngine;

namespace Multiverse
{
    public class MvUniverse
    {
        private readonly MvNetworkManager _networkManager;

        private static uint _lastTemporaryClientInstanceId;

        private readonly Dictionary<uint, MvGameObject>
            _temporaryClientInstances = new Dictionary<uint, MvGameObject>();

        internal MvUniverse()
        {
            _networkManager = MvNetworkManager.I;

            if (_networkManager.HasServer)
            {
                _networkManager.Server.AddMessageReceiver<MvInstantiatePrefabMessage>(InstantiateMessageReceivedServer);
                _networkManager.Server.AddMessageReceiver<MvDestroyMessage>(DestroyMessageReceivedServer);
                _networkManager.Server.Players.OnAddedValue(null, OnPlayerAdded);
            }

            if (!_networkManager.IsHost && _networkManager.HasClient)
            {
                _networkManager.Client.AddMessageReceiver<MvInstantiatePrefabMessage>(InstantiateMessageReceivedClient);
                _networkManager.Client.AddMessageReceiver<MvDestroyMessage>(DestroyMessageReceivedClient);
            }
        }

        private void OnPlayerAdded(MvPlayer player)
        {
            foreach (var instance in MvIdManager.GetInstances())
            {
                _networkManager.Server.SendMessageToPlayer(player, new MvInstantiatePrefabMessage
                {
                    PrefabId = instance.PrefabId,
                    InstanceId = instance.Id,
                    Position = instance.transform.position,
                    Rotation = instance.transform.rotation
                });
            }
        }

        public MvGameObject Instantiate(GameObject gameObject)
        {
            return Instantiate(gameObject, gameObject.transform.position);
        }

        public MvGameObject Instantiate(MvGameObject mvGameObject)
        {
            return Instantiate(mvGameObject, mvGameObject.transform.position);
        }

        public MvGameObject Instantiate(MvGameObject mvGameObject, Vector3 position)
        {
            return Instantiate(mvGameObject, position, mvGameObject.transform.rotation);
        }

        public MvGameObject Instantiate(GameObject gameObject, Vector3 position)
        {
            return Instantiate(gameObject, position, gameObject.transform.rotation);
        }

        public MvGameObject Instantiate(GameObject gameObject, Vector3 position, Quaternion rotation)
        {
            var mvGo = gameObject.GetComponent<MvGameObject>();
            if (!mvGo)
                throw new MvException($"Cannot instantiate {gameObject.name}, it does not have an MvGameObject!");

            return Instantiate(mvGo, position, rotation);
        }

        public MvGameObject Instantiate(MvGameObject mvGameObject, Vector3 position, Quaternion rotation)
        {
            if (!mvGameObject.IsPrefab)
                throw new MvException($"Cannot instantiate {mvGameObject.name}, it is not a prefab!");

            if (!_networkManager.HasServer && !mvGameObject.AllowClientInstantiate)
                throw new MvException($"Cannot instantiate {mvGameObject.name} on the client!");

            var instantiated = Object.Instantiate(mvGameObject.gameObject, position, rotation)
                .GetComponent<MvGameObject>();

            if (_networkManager.HasServer)
                instantiated.GenerateInstanceId();
            instantiated.AssignOwner(MvNetworkManager.I.Client?.LocalPlayer);

            var msg = new MvInstantiatePrefabMessage
            {
                PrefabId = instantiated.PrefabId,
                InstanceId = instantiated.Id,
                Position = instantiated.transform.position,
                Rotation = instantiated.transform.rotation,
                Owner = instantiated.Owner
            };

            if (_networkManager.HasServer)
                _networkManager.Server.SendMessageToAll(msg);
            else
            {
                msg.ClientInstanceId = ++_lastTemporaryClientInstanceId;
                _temporaryClientInstances[msg.ClientInstanceId] = instantiated;
                _networkManager.Client.SendMessageToServer(msg);
            }

            return instantiated;
        }

        private void InstantiateMessageReceivedServer(MvPlayer sender, MvInstantiatePrefabMessage msg)
        {
            Debug.Log($"GOT INSTANTIATE MSG FROm {sender} {msg}!");
            Debug.Log($"pref id: {sender.Id} {msg.PrefabId}!");
            var prefab = MvIdManager.GetPrefab(msg.PrefabId);
            if (!_networkManager.HasServer && !prefab.AllowClientInstantiate)
            {
                Debug.LogError($"Client tried to spawn {prefab.name}! This is not allowed for this prefab.");
                return;
            }

            var instantiated = Object.Instantiate(prefab, msg.Position, msg.Rotation);
            instantiated.GenerateInstanceId();
            instantiated.AssignOwner(sender);
            var clientMsg = new MvInstantiatePrefabMessage
            {
                PrefabId = instantiated.PrefabId,
                InstanceId = instantiated.Id,
                Position = instantiated.transform.position,
                Rotation = instantiated.transform.rotation,
                Owner = sender
            };

            var senderMsg = clientMsg;
            senderMsg.ClientInstanceId = msg.ClientInstanceId;

            Debug.Log($"SENDING INSTANTIATE MSG TO {_networkManager.Server.Players.Count} players!");
            foreach (var client in _networkManager.Server.Players.Values)
                _networkManager.Server.SendMessageToPlayer(client, client == sender ? senderMsg : clientMsg);
        }

        private void InstantiateMessageReceivedClient(MvInstantiatePrefabMessage msg)
        {
            MvGameObject mvGameObject;
            if (msg.ClientInstanceId > 0)
            {
                mvGameObject = _temporaryClientInstances[msg.ClientInstanceId];
                _temporaryClientInstances.Remove(msg.ClientInstanceId);
            }
            else
            {
                var prefab = MvIdManager.GetPrefab(msg.PrefabId);
                mvGameObject = Object.Instantiate(prefab, msg.Position, msg.Rotation).GetComponent<MvGameObject>();
            }

            mvGameObject.AssignInstanceId(msg.InstanceId);
        }

        public void Destroy(MvGameObject mvGameObject)
        {
            // TODO: Only allow destroy on client if that client is owner!

            if (!mvGameObject.IsInstance)
                throw new MvException($"Cannot destroy {mvGameObject.name}, it is not an instance!");

            if (!_networkManager.HasServer && !mvGameObject.AllowClientDestroy)
                throw new MvException($"Cannot destroy {mvGameObject.name} on the client!");
            
            if (!mvGameObject.HasControl)
                throw new MvException($"Cannot destroy {mvGameObject.name}, this client doesn't have ownership!");

            var msg = new MvDestroyMessage
            {
                InstanceId = mvGameObject.Id
            };

            Object.Destroy(mvGameObject.gameObject);
            if (_networkManager.HasServer)
                _networkManager.Server.SendMessageToAll(msg);
            else
                _networkManager.Client.SendMessageToServer(msg);
        }

        private void DestroyMessageReceivedServer(MvPlayer sender, MvDestroyMessage msg)
        {
            var instance = MvIdManager.GetInstance(msg.InstanceId);
            if (!instance.AllowClientDestroy)
            {
                Debug.LogError($"Client tried to destroy {instance.name}! This is not allowed for this instance.");
                return;
            }
            
            if (instance.Owner != sender)
            {
                Debug.LogError($"Client that is not the owner tried to destroy {instance.name}!");
                return;
            }

            Object.Destroy(instance.gameObject);

            foreach (var client in _networkManager.Server.Players.Values.Where(c => c != sender))
                _networkManager.Server.SendMessageToPlayer(client, msg);
        }

        private void DestroyMessageReceivedClient(MvDestroyMessage msg)
        {
            var instance = MvIdManager.GetInstance(msg.InstanceId);
            Object.Destroy(instance.gameObject);
        }
    }
}