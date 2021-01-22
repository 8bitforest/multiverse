using System;
using System.Collections.Generic;
using System.Linq;
using Multiverse.Extensions;
using UnityEngine;

namespace Multiverse.Messaging
{
    internal static class MvMessageTypes
    {
        private static readonly Dictionary<Type, byte> MessageTypesDictionary;
        private static readonly Type[] MessageTypes;

        private delegate MvMessage Serializer(IMvNetworkMessage networkMessage);

        private delegate IMvNetworkMessage Deserializer(MvMessage message);

        private static readonly Dictionary<Type, Serializer> Serializers;
        private static readonly Dictionary<Type, Deserializer> Deserializers;

        static MvMessageTypes()
        {
            MessageTypesDictionary = new Dictionary<Type, byte>();
            Serializers = new Dictionary<Type, Serializer>();
            Deserializers = new Dictionary<Type, Deserializer>();

            // This will break if the server is out of date with the client
            // TODO: Sanity check to make sure server and client have same list of IMvNetworkMessages
            MessageTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsInterface && typeof(IMvNetworkMessage).IsAssignableFrom(t))
                .OrderBy(t => t.FullName.GetDeterministicHashCode())
                .ToArray();

            for (var i = 0; i < MessageTypes.Length; i++)
            {
                if (i == byte.MaxValue + 1)
                {
                    Debug.LogError("Too many IMvNetworkMessage types!");
                    break;
                }

                MessageTypesDictionary[MessageTypes[i]] = (byte) i;
                Serializers[MessageTypes[i]] = BuildSerializer(MessageTypes[i]);
                Deserializers[MessageTypes[i]] = BuildDeserializer(MessageTypes[i]);
            }


            Debug.Log($"Registered {MessageTypes.Length} message types");
        }

        private static Serializer BuildSerializer(Type messageType)
        {
            return null;
        }

        private static Deserializer BuildDeserializer(Type messageType)
        {
            return null;
        }

        public static Type GetTypeForId(byte id)
        {
            return MessageTypes[id];
        }

        public static byte GetIdForType(Type type)
        {
            return MessageTypesDictionary[type];
        }
    }
}