using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Multiverse.Messaging
{
    public static class MvSerializableTypes
    {
        private static readonly UTF8Encoding Encoding = new UTF8Encoding(false, true);
        private static readonly byte[] StringBuffer = new byte[MvBinaryWriter.MaxStringLength];

        #region Blittable Primitives

        public static bool ReadBool(this MvBinaryReader r) => r.ReadBlittable<byte>() != 0;
        public static void WriteBool(this MvBinaryWriter w, bool v) => w.WriteBlittable((byte) (v ? 1 : 0));

        public static byte ReadByte(this MvBinaryReader r) => r.ReadBlittable<byte>();
        public static void WriteByte(this MvBinaryWriter w, byte v) => w.WriteBlittable(v);

        public static sbyte ReadSByte(this MvBinaryReader r) => r.ReadBlittable<sbyte>();
        public static void WriteSByte(this MvBinaryWriter w, sbyte v) => w.WriteBlittable(v);

        public static char ReadChar(this MvBinaryReader r) => (char) r.ReadBlittable<short>();
        public static void WriteChar(this MvBinaryWriter w, char v) => w.WriteBlittable((short) v);

        public static short ReadShort(this MvBinaryReader r) => r.ReadBlittable<short>();
        public static void WriteShort(this MvBinaryWriter w, short v) => w.WriteBlittable(v);

        public static ushort ReadUShort(this MvBinaryReader r) => r.ReadBlittable<ushort>();
        public static void WriteUShort(this MvBinaryWriter w, ushort v) => w.WriteBlittable(v);

        public static int ReadInt(this MvBinaryReader r) => r.ReadBlittable<int>();
        public static void WriteInt(this MvBinaryWriter w, int v) => w.WriteBlittable(v);

        public static uint ReadUInt(this MvBinaryReader r) => r.ReadBlittable<uint>();
        public static void WriteUInt(this MvBinaryWriter w, uint v) => w.WriteBlittable(v);

        public static long ReadLong(this MvBinaryReader r) => r.ReadBlittable<long>();
        public static void WriteLong(this MvBinaryWriter w, long v) => w.WriteBlittable(v);

        public static ulong ReadULong(this MvBinaryReader r) => r.ReadBlittable<ulong>();
        public static void WriteULong(this MvBinaryWriter w, ulong v) => w.WriteBlittable(v);

        public static float ReadFloat(this MvBinaryReader r) => r.ReadBlittable<float>();
        public static void WriteFloat(this MvBinaryWriter w, float v) => w.WriteBlittable(v);

        public static double ReadDouble(this MvBinaryReader r) => r.ReadBlittable<double>();
        public static void WriteDouble(this MvBinaryWriter w, double v) => w.WriteBlittable(v);

        public static decimal ReadDecimal(this MvBinaryReader r) => r.ReadBlittable<decimal>();
        public static void WriteDecimal(this MvBinaryWriter w, decimal v) => w.WriteBlittable(v);

        public static T ReadEnum<T>(this MvBinaryReader r) where T : Enum => (T) (object) r.ReadBlittable<int>();
        public static void WriteEnum<T>(this MvBinaryWriter w, T v) where T : Enum
            => w.WriteBlittable((int) (object) v);

        #endregion

        #region Blittable Unity

        public static Vector2 ReadVector2(this MvBinaryReader r) => r.ReadBlittable<Vector2>();
        public static void WriteVector2(this MvBinaryWriter w, Vector2 v) => w.WriteBlittable(v);

        public static Vector3 ReadVector3(this MvBinaryReader r) => r.ReadBlittable<Vector3>();
        public static void WriteVector3(this MvBinaryWriter w, Vector3 v) => w.WriteBlittable(v);

        public static Vector4 ReadVector4(this MvBinaryReader r) => r.ReadBlittable<Vector4>();
        public static void WriteVector4(this MvBinaryWriter w, Vector4 v) => w.WriteBlittable(v);

        public static Vector2Int ReadVector2Int(this MvBinaryReader r) => r.ReadBlittable<Vector2Int>();
        public static void WriteVector2Int(this MvBinaryWriter w, Vector2Int v) => w.WriteBlittable(v);

        public static Vector3Int ReadVector3Int(this MvBinaryReader r) => r.ReadBlittable<Vector3Int>();
        public static void WriteVector3Int(this MvBinaryWriter w, Vector3Int v) => w.WriteBlittable(v);

        public static Color ReadColor(this MvBinaryReader r) => r.ReadBlittable<Color>();
        public static void WriteColor(this MvBinaryWriter w, Color v) => w.WriteBlittable(v);

        public static Color32 ReadColor32(this MvBinaryReader r) => r.ReadBlittable<Color32>();
        public static void WriteColor32(this MvBinaryWriter w, Color32 v) => w.WriteBlittable(v);

        public static Quaternion ReadQuaternion(this MvBinaryReader r) => r.ReadBlittable<Quaternion>();
        public static void WriteQuaternion(this MvBinaryWriter w, Quaternion v) => w.WriteBlittable(v);

        public static Rect ReadRect(this MvBinaryReader r) => r.ReadBlittable<Rect>();
        public static void WriteRect(this MvBinaryWriter w, Rect v) => w.WriteBlittable(v);

        public static Plane ReadPlane(this MvBinaryReader r) => r.ReadBlittable<Plane>();
        public static void WritePlane(this MvBinaryWriter w, Plane v) => w.WriteBlittable(v);

        public static Ray ReadRay(this MvBinaryReader r) => r.ReadBlittable<Ray>();
        public static void WriteRay(this MvBinaryWriter w, Ray v) => w.WriteBlittable(v);

        public static Matrix4x4 ReadMatrix4x4(this MvBinaryReader r) => r.ReadBlittable<Matrix4x4>();
        public static void WriteMatrix4x4(this MvBinaryWriter w, Matrix4x4 v) => w.WriteBlittable(v);

        #endregion

        // C#
        public static byte[] ReadByteArray(this MvBinaryReader reader)
        {
            var length = reader.ReadInt();
            return length == -1 ? null : reader.ReadByteArray(length);
        }

        public static void WriteByteArray(this MvBinaryWriter writer, byte[] bytes)
        {
            if (bytes == null)
            {
                writer.WriteInt(-1);
                return;
            }

            writer.WriteInt(bytes.Length);
            writer.WriteByteArray(bytes, 0, bytes.Length);
        }

        public static ArraySegment<byte> ReadByteArraySegment(this MvBinaryReader reader)
        {
            var length = reader.ReadInt();
            return length == -1 ? default : reader.ReadByteArraySegment(length);
        }

        public static void WriteByteArraySegment(this MvBinaryWriter writer, ArraySegment<byte> bytes)
        {
            writer.WriteInt(bytes.Count);
            writer.WriteByteArray(bytes.Array, bytes.Offset, bytes.Count);
        }

        public static string ReadString(this MvBinaryReader reader)
        {
            var length = reader.ReadInt();
            if (length == -1)
                return null;

            if (length >= MvBinaryWriter.MaxStringLength)
                throw new EndOfStreamException($"MvBinaryReader string too long!");

            var segment = reader.ReadByteArraySegment(length);
            return Encoding.GetString(segment.Array!, segment.Offset, segment.Count);
        }

        public static void WriteString(this MvBinaryWriter writer, string value)
        {
            if (value == null)
            {
                writer.WriteInt(-1);
                return;
            }

            if (value.Length >= MvBinaryWriter.MaxStringLength)
                throw new IndexOutOfRangeException($"MvBinaryWriter string too long: {value.Length}!");

            var length = Encoding.GetBytes(value, 0, value.Length, StringBuffer, 0);
            writer.WriteInt(length);
            writer.WriteByteArray(StringBuffer, 0, length);
        }

        public static List<T> ReadList<T>(this MvBinaryReader reader)
        {
            var length = reader.ReadInt();
            if (length == -1)
                return null;

            var result = new List<T>(length);
            for (var i = 0; i < length; i++)
                result.Add(reader.Read<T>());

            return result;
        }

        public static void WriteList<T>(this MvBinaryWriter writer, List<T> value)
        {
            if (value == null)
            {
                writer.WriteInt(-1);
                return;
            }

            writer.WriteInt(value.Count);
            foreach (var val in value)
                writer.Write(val);
        }

        public static T ReadNetworkMessageClass<T>(this MvBinaryReader reader) where T : class, IMvNetworkMessage
        {
            return reader.ReadByte() == 0 ? null : reader.ReadNetworkMessage<T>();
        }

        public static void WriteNetworkMessageClass<T>(this MvBinaryWriter writer, T value)
            where T : class, IMvNetworkMessage
        {
            if (value == null)
            {
                writer.WriteByte(0);
                return;
            }

            writer.WriteByte(1);
            writer.WriteNetworkMessage(value);
        }

        public static T ReadNetworkMessageStruct<T>(this MvBinaryReader reader) where T : struct, IMvNetworkMessage
        {
            return reader.ReadNetworkMessage<T>();
        }

        public static void WriteNetworkMessageStruct<T>(this MvBinaryWriter writer, T value)
            where T : struct, IMvNetworkMessage
        {
            writer.WriteNetworkMessage(value);
        }

        public static T? ReadNullable<T>(this MvBinaryReader reader) where T : struct
        {
            return reader.ReadByte() == 0 ? (T?) null : reader.Read<T>();
        }

        public static void WriteNullable<T>(this MvBinaryWriter writer, T? value) where T : struct
        {
            if (value == null)
            {
                writer.WriteByte(0);
                return;
            }

            writer.WriteByte(1);
            writer.Write(value.Value);
        }

        public static T[] ReadArray<T>(this MvBinaryReader reader)
        {
            var length = reader.ReadInt();
            if (length == -1)
                return null;

            var result = new T[length];
            for (var i = 0; i < length; i++)
                result[i] = reader.Read<T>();

            return result;
        }
        
        public static void WriteArray<T>(this MvBinaryWriter writer, T[] value)
        {
            if (value == null)
            {
                writer.WriteInt(-1);
                return;
            }

            writer.WriteInt(value.Length);
            foreach (var val in value)
                writer.Write(val);
        }
    }
}
