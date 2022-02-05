using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Multiverse.Utils;
using Unity.Collections.LowLevel.Unsafe;

namespace Multiverse.Serialization
{
    public class MvBinaryReader
    {
        internal delegate T ReadMethod<out T>(MvBinaryReader reader);

        private static class ReadMethods<T>
        {
            public static ReadMethod<T> ReadMethod { get; set; }
        }

        private static readonly Dictionary<Type, MethodInfo> GenericExtensionMethods;

        private ArraySegment<byte> _data;
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

            // Register all non-generic extensions
            foreach (var m in extensions.Where(m => !m.IsGenericMethod))
            {
                typeof(MvBinaryReader).GetMethod(nameof(RegisterReadMethod),
                        BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(m.ReturnType)
                    .Invoke(null, new object[] {m});
            }

            GenericExtensionMethods = extensions
                .Where(m => m.IsGenericMethod)
                .ToDictionary(m => ReflectionUtils.GetGenericTypeDefinition(m.ReturnType));
        }

        internal static ReadMethod<T> RegisterReadMethod<T>(MethodInfo extensionMethod = null)
        {
            var type = typeof(T);
            if (extensionMethod != null)
                return ReadMethods<T>.ReadMethod = CreateReadMethod<T>(extensionMethod);
            if (typeof(IMvSerializable).IsAssignableFrom(type))
                return ReadMethods<T>.ReadMethod = CreateReadClass<T>();
            if (GenericExtensionMethods.ContainsKey(ReflectionUtils.GetGenericTypeDefinition(type)))
                return ReadMethods<T>.ReadMethod =
                    CreateReadMethod<T>(GenericExtensionMethods[ReflectionUtils.GetGenericTypeDefinition(type)]);

            throw new MvException($"{type} cannot be read from MvBinaryReader!");
        }

        private static ReadMethod<T> CreateReadClass<T>()
        {
            var type = typeof(T);
            var isStruct = type.IsValueType;
            var exps = new List<Expression>();

            var fieldsProps = ReflectionUtils.GetAllFieldsProperties(type).ToArray();
            var readByteMethod = Expression.Constant(ReadMethods<byte>.ReadMethod);
            var readerParam = Expression.Parameter(typeof(MvBinaryReader));
            var instanceVar = Expression.Variable(type);

            exps.Add(Expression.Assign(instanceVar, Expression.New(type)));
            exps.AddRange(fieldsProps.Select(fieldProp =>
            {
                var fpType = ReflectionUtils.GetFieldPropertyType(fieldProp);
                var readMethod = typeof(MvBinaryReader).GetMethod(nameof(Read))!.MakeGenericMethod(fpType);
                var fieldPropExp = Expression.PropertyOrField(instanceVar, fieldProp.Name);
                return Expression.Assign(fieldPropExp, Expression.Call(readerParam, readMethod));
            }));
            exps.Add(instanceVar);

            var blockExp = Expression.Block(new[] {instanceVar}, exps);

            if (isStruct)
                return Expression.Lambda<ReadMethod<T>>(blockExp, readerParam).Compile();

            var returnTarget = Expression.Label(type);
            var nullCheckExp = Expression.IfThenElse(
                Expression.Equal(Expression.Invoke(readByteMethod, readerParam), Expression.Constant((byte) 0)),
                Expression.Return(returnTarget, Expression.Constant(null, type)),
                Expression.Return(returnTarget, blockExp));

            var returnLabel = Expression.Label(returnTarget, Expression.Constant(null, type));
            return Expression.Lambda<ReadMethod<T>>(Expression.Block(nullCheckExp, returnLabel), readerParam).Compile();
        }

        private static ReadMethod<T> CreateReadMethod<T>(MethodInfo extensionMethod)
        {
            var type = typeof(T);
            ParameterExpression[] args = {Expression.Parameter(typeof(MvBinaryReader))};
            Expression[] convertedArgs = {args[0]};

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
                throw new MvException($"{type} cannot be read from MvBinaryReader!");
            }

            return Expression.Lambda<ReadMethod<T>>(call, args).Compile();
        }

        public MvBinaryReader()
        {
            _data = new ArraySegment<byte>(new byte[] { });
            _position = 0;
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

        public void Reset(ArraySegment<byte> bytes)
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

        public T Read<T>()
        {
            var method = ReadMethods<T>.ReadMethod;
            if (method != null)
                return method(this);
            return RegisterReadMethod<T>()(this);
        }

        private void EnsureLength(int length)
        {
            if (length > _data.Count)
                throw new EndOfStreamException("Buffer not long enough to read from!");
        }
    }
}