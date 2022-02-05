using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Multiverse.Utils;
using Unity.Collections.LowLevel.Unsafe;

namespace Multiverse.Serialization
{
    public class MvBinaryWriter
    {
        public const int MaxStringLength = 1024 * 32;

        internal delegate void WriteMethod<in T>(MvBinaryWriter writer, T value);

        private static class WriteMethods<T>
        {
            internal static WriteMethod<T> WriteMethod { get; set; }
        }

        private static readonly Dictionary<Type, MethodInfo> GenericExtensionMethods;

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

            // Register all non-generic extensions
            foreach (var m in extensions.Where(m => !m.IsGenericMethod))
            {
                typeof(MvBinaryWriter).GetMethod(nameof(RegisterWriteMethod),
                        BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(m.GetParameters()[1].ParameterType)
                    .Invoke(null, new object[] {m});
            }

            GenericExtensionMethods = extensions
                .Where(m => m.IsGenericMethod)
                .ToDictionary(m => ReflectionUtils.GetGenericTypeDefinition(m.GetParameters()[1].ParameterType));
        }

        internal static WriteMethod<T> RegisterWriteMethod<T>(MethodInfo extensionMethod = null)
        {
            var type = typeof(T);
            if (extensionMethod != null)
                return WriteMethods<T>.WriteMethod = CreateWriteMethod<T>(extensionMethod);
            if (typeof(IMvSerializable).IsAssignableFrom(type))
                return WriteMethods<T>.WriteMethod = CreateWriteClass<T>();
            if (GenericExtensionMethods.ContainsKey(ReflectionUtils.GetGenericTypeDefinition(type)))
                return WriteMethods<T>.WriteMethod =
                    CreateWriteMethod<T>(GenericExtensionMethods[ReflectionUtils.GetGenericTypeDefinition(type)]);

            throw new MvException($"{type} cannot be written to MvBinaryWriter!");
        }

        private static WriteMethod<T> CreateWriteClass<T>()
        {
            var type = typeof(T);
            var isStruct = type.IsValueType;
            var exps = new List<Expression>();

            var fieldsProps = ReflectionUtils.GetAllFieldsProperties(type).ToArray();
            var writeByteMethod = Expression.Constant(WriteMethods<byte>.WriteMethod);
            var writerParam = Expression.Parameter(typeof(MvBinaryWriter));
            var instanceParam = Expression.Parameter(typeof(T));

            if (!isStruct)
                exps.Add(Expression.Invoke(writeByteMethod, writerParam, Expression.Constant((byte) 1)));

            exps.AddRange(fieldsProps.Select(fieldProp =>
            {
                var fpType = ReflectionUtils.GetFieldPropertyType(fieldProp);
                var writeMethod = typeof(MvBinaryWriter).GetMethod(nameof(Write))!.MakeGenericMethod(fpType);
                var getPropExp = Expression.PropertyOrField(instanceParam, fieldProp.Name);
                return Expression.Call(writerParam, writeMethod, getPropExp);
            }));

            var blockExp = Expression.Block(exps);

            if (isStruct)
                return Expression.Lambda<WriteMethod<T>>(blockExp, writerParam, instanceParam).Compile();

            var nullCheckExp = Expression.IfThenElse(
                Expression.Equal(instanceParam, Expression.Constant(null, type)),
                Expression.Invoke(writeByteMethod, writerParam, Expression.Constant((byte) 0)),
                blockExp);

            return Expression.Lambda<WriteMethod<T>>(nullCheckExp, writerParam, instanceParam).Compile();
        }

        private static WriteMethod<T> CreateWriteMethod<T>(MethodInfo extensionMethod)
        {
            var type = typeof(T);
            ParameterExpression[] args = {Expression.Parameter(typeof(MvBinaryWriter)), Expression.Parameter(type)};
            Expression[] convertedArgs = {args[0], args[1]};

            MethodCallExpression call;
            try
            {
                call = !extensionMethod.IsGenericMethod
                    ? Expression.Call(extensionMethod, convertedArgs)
                    : Expression.Call(extensionMethod.MakeGenericMethod(ReflectionUtils.GetGenericType(type)),
                        convertedArgs);
            }
            catch (ArgumentException)
            {
                throw new MvException($"{type} cannot be written to MvBinaryWriter!");
            }

            return Expression.Lambda<WriteMethod<T>>(call, args).Compile();
        }

        public MvBinaryWriter()
        {
            _position = 0;
        }

        public ArraySegment<byte> GetData()
        {
            return new ArraySegment<byte>(_buffer, 0, _position);
        }

        public void Reset()
        {
            _position = 0;
        }

        internal unsafe void WriteBlittable<T>(T value) where T : unmanaged
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

        internal void WriteByteArray(byte[] bytes, int index, int length)
        {
            EnsureCapacity(_position + length);
            Array.Copy(bytes, index, _buffer, _position, length);
            _position += length;
        }

        public void Write<T>(T value)
        {
            var method = WriteMethods<T>.WriteMethod;
            if (method != null)
                method(this, value);
            else
                RegisterWriteMethod<T>()(this, value);
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