using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Multiverse.Utils;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Multiverse.Messaging
{
    public class MvBinaryWriter
    {
        public const int MaxStringLength = 1024 * 32;

        private delegate void WriteMethod(MvBinaryWriter writer, object value);

        private static readonly Dictionary<Type, WriteMethod> WriteMethods;
        private static readonly Dictionary<Type, MethodInfo> GenericExtensionMethods;
        private static readonly Dictionary<Type, WriteMethod> NetworkMessageWriteMethods;

        private byte[] _buffer = new byte[2048];
        private int _position;

        static MvBinaryWriter()
        {
            var extensions = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsGenericType && !t.IsNested)
                .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public))
                .Where(m => m.IsDefined(typeof(ExtensionAttribute), false))
                .Where(m => m.GetParameters().Length == 2 && m.ReturnType == typeof(void))
                .Where(m => m.GetParameters()[0].ParameterType == typeof(MvBinaryWriter)).ToArray();

            WriteMethods = extensions
                .Where(m => !m.IsGenericMethod)
                .ToDictionary(m => m.GetParameters()[1].ParameterType, CreateWriteMethod);

            GenericExtensionMethods = extensions
                .Where(m => m.IsGenericMethod)
                .ToDictionary(m => ReflectionUtils.GetGenericTypeDefinition(m.GetParameters()[1].ParameterType));

            NetworkMessageWriteMethods = new Dictionary<Type, WriteMethod>();

            Debug.Log($"Registered {WriteMethods.Count} MvBinaryWriter methods");
        }

        private static WriteMethod FindWriteMethodForType<T>()
        {
            return FindWriteMethodForType(typeof(T));
        }

        private static WriteMethod FindWriteMethodForType(Type type)
        {
            if (WriteMethods.ContainsKey(type))
                return WriteMethods[type];

            Debug.LogWarning($"Type {type} doesn't have a write method yet! Making one...");

            if (GenericExtensionMethods.ContainsKey(ReflectionUtils.GetGenericTypeDefinition(type)))
            {
                var method = GenericExtensionMethods[ReflectionUtils.GetGenericTypeDefinition(type)];
                WriteMethods[type] = CreateWriteMethod(method, type);
                return WriteMethods[type];
            }

            throw new MvException($"{type} cannot be written to MvBinaryWriter!");
        }

        private static WriteMethod CreateWriteMethod(MethodInfo extensionMethod)
        {
            return CreateWriteMethod(extensionMethod, extensionMethod.GetParameters()[1].ParameterType);
        }

        private static WriteMethod CreateWriteMethod(MethodInfo extensionMethod, Type type)
        {
            ParameterExpression[] arguments =
            {
                Expression.Parameter(typeof(MvBinaryWriter)),
                Expression.Parameter(typeof(object))
            };

            Expression[] convertedArguments =
            {
                arguments[0],
                Expression.Convert(arguments[1], type)
            };

            MethodCallExpression call;
            try
            {
                call = !extensionMethod.IsGenericMethod
                    ? Expression.Call(extensionMethod, convertedArguments)
                    : Expression.Call(extensionMethod.MakeGenericMethod(ReflectionUtils.GetGenericType(type)),
                        convertedArguments);
            }
            catch (ArgumentException)
            {
                throw new MvException($"{type} cannot be written to MvBinaryWriter!");
            }

            return Expression.Lambda<WriteMethod>(call, arguments).Compile();
        }

        public MvBinaryWriter()
        {
            _position = 0;
        }

        public ArraySegment<byte> GetData()
        {
            return new ArraySegment<byte>(_buffer, 0, _position);
        }

        public void Clear()
        {
            _position = 0;
        }

        public unsafe void WriteBlittable<T>(T value) where T : unmanaged
        {
#if UNITY_EDITOR
            if (!UnsafeUtility.IsBlittable(typeof(T)))
                throw new ArgumentException(typeof(T) + " is not blittable!");
#endif

            var size = sizeof(T);
            EnsureCapacity(_position + size);

            fixed (byte* ptr = &_buffer[_position])
                *(T*) ptr = value;

            _position += size;
        }

        public void WriteByteArray(byte[] bytes, int index, int length)
        {
            EnsureCapacity(_position + length);
            Array.Copy(bytes, index, _buffer, _position, length);
            _position += length;
        }

        public void WriteNetworkMessage<T>(T value) where T : IMvNetworkMessage
        {
            var type = typeof(T);
            if (!NetworkMessageWriteMethods.ContainsKey(type))
            {
                var fieldsProps = ReflectionUtils.GetAllFieldsProperties(type).ToArray();
                var getters = fieldsProps.Select(ReflectionUtils.GetFieldPropertyGetter).ToArray();
                var writeMethods = fieldsProps.Select(fp =>
                    FindWriteMethodForType(ReflectionUtils.GetFieldPropertyType(fp))).ToArray();

                void WriteMethod(MvBinaryWriter w, object obj)
                {
                    for (var i = 0; i < getters.Length; i++)
                        writeMethods[i](w, getters[i](obj));
                }

                NetworkMessageWriteMethods[type] = WriteMethod;
            }

            NetworkMessageWriteMethods[type](this, value);
        }

        public void Write<T>(T value)
        {
            FindWriteMethodForType<T>()(this, value);
        }

        private void EnsureCapacity(int capacity)
        {
            if (_buffer.Length >= capacity)
                return;

            var newCapacity = Math.Max(capacity, _buffer.Length * 2);
            Array.Resize(ref _buffer, newCapacity);
        }
    }
}