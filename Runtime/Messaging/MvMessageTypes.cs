using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Multiverse.Extensions;
using Multiverse.Serialization;
using UnityEngine;

namespace Multiverse.Messaging
{
    internal static class MvMessageTypes
    {
        private static readonly Dictionary<Type, byte> MessageTypesDictionary;
        private static readonly Type[] MessageTypes;

        static MvMessageTypes()
        {
            MessageTypesDictionary = new Dictionary<Type, byte>();

            // This will break if the server is out of date with the client
            // TODO: Sanity check to make sure server and client have same list of IMvNetworkMessages
            MessageTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsInterface && typeof(IMvMessage).IsAssignableFrom(t))
                .OrderBy(t => t.FullName.GetDeterministicHashCode())
                .ToArray();

            for (var i = 0; i < MessageTypes.Length; i++)
            {
                if (i == byte.MaxValue + 1)
                {
                    Debug.LogError("Too many IMvMessage types!");
                    break;
                }

                MessageTypesDictionary[MessageTypes[i]] = (byte) i;
                typeof(MvBinaryWriter).GetMethod(nameof(MvBinaryWriter.RegisterWriteMethod),
                        BindingFlags.Static | BindingFlags.NonPublic)!
                    .MakeGenericMethod(MessageTypes[i])
                    .Invoke(null, new object[] {null});
                typeof(MvBinaryReader).GetMethod(nameof(MvBinaryReader.RegisterReadMethod),
                        BindingFlags.Static | BindingFlags.NonPublic)!
                    .MakeGenericMethod(MessageTypes[i])
                    .Invoke(null, new object[] {null});
            }

            Debug.Log($"Registered {MessageTypes.Length}/{byte.MaxValue} message types");
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