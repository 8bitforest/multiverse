using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Multiverse.Utils;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Multiverse.Messaging
{
    public class MvBinaryReader
    {
        private delegate object ReadMethod(MvBinaryReader reader);

        private static readonly Dictionary<Type, ReadMethod> ReadMethods;
        private static readonly Dictionary<Type, MethodInfo> GenericExtensionMethods;
        private static readonly Dictionary<Type, ReadMethod> ClassStructReadMethods;

        private readonly ArraySegment<byte> _data;
        private int _position;

        static MvBinaryReader()
        {
            var extensions = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsGenericType && !t.IsNested)
                .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public))
                .Where(m => m.IsDefined(typeof(ExtensionAttribute), false))
                .Where(m => m.GetParameters().Length == 1 && m.ReturnType != typeof(void))
                .Where(m => m.GetParameters()[0].ParameterType == typeof(MvBinaryReader)).ToArray();

            ReadMethods = extensions
                .Where(m => !m.IsGenericMethod)
                .ToDictionary(m => m.ReturnType, CreateReadMethod);

            GenericExtensionMethods = extensions
                .Where(m => m.IsGenericMethod)
                .ToDictionary(m => ReflectionUtils.GetGenericTypeDefinition(m.ReturnType));

            ClassStructReadMethods = new Dictionary<Type, ReadMethod>();

            Debug.Log($"Registered {ReadMethods.Count} MvBinaryReader methods");
        }

        private static ReadMethod FindReadMethodForType<T>()
        {
            return FindReadMethodForType(typeof(T));
        }

        private static ReadMethod FindReadMethodForType(Type type)
        {
            if (ReadMethods.ContainsKey(type))
                return ReadMethods[type];

            Debug.LogWarning($"Type {type} doesn't have a read method yet! Making one...");

            if (GenericExtensionMethods.ContainsKey(ReflectionUtils.GetGenericTypeDefinition(type)))
            {
                var method = GenericExtensionMethods[ReflectionUtils.GetGenericTypeDefinition(type)];
                ReadMethods[type] = CreateReadMethod(method, type);
                return ReadMethods[type];
            }

            throw new MvException($"{type} cannot be written to MvBinaryWriter!");
        }

        private static ReadMethod CreateReadMethod(MethodInfo extensionMethod)
        {
            return CreateReadMethod(extensionMethod, extensionMethod.ReturnType);
        }

        private static ReadMethod CreateReadMethod(MethodInfo extensionMethod, Type type)
        {
            ParameterExpression[] arguments = {Expression.Parameter(typeof(MvBinaryReader))};
            var convertedCall = !extensionMethod.IsGenericMethod
                ? Expression.Convert(Expression.Call(extensionMethod, arguments.Cast<Expression>()), typeof(object))
                : Expression.Convert(
                    Expression.Call(extensionMethod.MakeGenericMethod(ReflectionUtils.GetGenericType(type)),
                        arguments.Cast<Expression>()), typeof(object));
            return Expression.Lambda<ReadMethod>(convertedCall, arguments).Compile();
        }

        public MvBinaryReader(byte[] bytes)
        {
            _data = new ArraySegment<byte>(bytes, 0, bytes.Length);
            _position = 0;
        }

        public MvBinaryReader(ArraySegment<byte> bytes)
        {
            _data = bytes;
            _position = 0;
        }

        internal unsafe T ReadBlittable<T>() where T : unmanaged
        {
#if UNITY_EDITOR
            if (!UnsafeUtility.IsBlittable(typeof(T)))
                throw new ArgumentException(typeof(T) + " is not blittable!");
#endif
            var size = sizeof(T);
            EnsureLength(_position + size);

            T value;
            fixed (byte* ptr = &_data.Array![_data.Offset + _position])
                value = *(T*) ptr;

            _position += size;
            return value;
        }

        internal ArraySegment<byte> ReadByteArraySegment(int length)
        {
            EnsureLength(_position + length);
            var segment = new ArraySegment<byte>(_data.Array!, _data.Offset + _position, length);
            _position += length;
            return segment;
        }

        internal byte[] ReadByteArray(int length)
        {
            EnsureLength(_position + length);
            var bytes = new byte[length];
            Array.Copy(_data.Array!, _data.Offset + _position, bytes, 0, length);
            _position += length;
            return bytes;
        }

        internal T ReadNetworkMessage<T>() where T : IMvNetworkMessage
        {
            var type = typeof(T);
            if (!ClassStructReadMethods.ContainsKey(type))
            {
                var fieldsProps = ReflectionUtils.GetAllFieldsProperties(type).ToArray();
                var setters = fieldsProps.Select(ReflectionUtils.GetFieldPropertySetter).ToArray();
                var readMethods = fieldsProps.Select(fp =>
                    FindReadMethodForType(ReflectionUtils.GetFieldPropertyType(fp))).ToArray();

                object ReadMethod(MvBinaryReader r)
                {
                    var obj = Activator.CreateInstance(type);
                    for (var i = 0; i < setters.Length; i++)
                        setters[i](obj, readMethods[i](r));
                    return obj;
                }

                ClassStructReadMethods[type] = ReadMethod;
            }

            return (T) ClassStructReadMethods[type](this);
        }

        public T Read<T>()
        {
            return (T) FindReadMethodForType<T>()(this);
        }

        private void EnsureLength(int length)
        {
            if (length > _data.Count)
                throw new EndOfStreamException("Buffer not long enough to read from!");
        }
    }
}