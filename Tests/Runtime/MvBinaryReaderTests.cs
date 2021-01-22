using System;
using System.Collections.Generic;
using System.IO;
using Multiverse.Messaging;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEngine;

namespace Multiverse.Tests
{
    [TestFixture]
    public class MvBinaryWriterReaderTests
    {
        private MvBinaryWriter _writer;
        private MvBinaryReader _reader;

        [SetUp]
        public void SetUp()
        {
            _writer = new MvBinaryWriter();
        }

        [Test]
        public void WriterLengthZero()
        {
            var buffer = _writer.GetData();
            Assert.AreEqual(0, buffer.Count);
        }

        [Test]
        public void WriterClear()
        {
            _writer.WriteInt(10);
            _writer.Clear();
            var buffer = _writer.GetData();
            Assert.AreEqual(0, buffer.Count);
        }

        [Test]
        public void ReadWriteBool()
        {
            TestReadWrite(true, MvSerializableTypes.ReadBool, MvSerializableTypes.WriteBool);
            TestReadWrite(false, MvSerializableTypes.ReadBool, MvSerializableTypes.WriteBool);
            TestReadWrite((byte) 128, MvSerializableTypes.ReadByte, MvSerializableTypes.WriteByte);
        }

        [Test]
        public void ReadWriteByte()
        {
            TestReadWrite(byte.MinValue, MvSerializableTypes.ReadByte, MvSerializableTypes.WriteByte);
            TestReadWrite(byte.MaxValue, MvSerializableTypes.ReadByte, MvSerializableTypes.WriteByte);
            TestReadWrite((sbyte) 0, MvSerializableTypes.ReadSByte, MvSerializableTypes.WriteSByte);
            TestReadWrite((sbyte) 4, MvSerializableTypes.ReadSByte, MvSerializableTypes.WriteSByte);
            TestReadWrite((sbyte) -4, MvSerializableTypes.ReadSByte, MvSerializableTypes.WriteSByte);
            TestReadWrite(sbyte.MinValue, MvSerializableTypes.ReadSByte, MvSerializableTypes.WriteSByte);
            TestReadWrite(sbyte.MaxValue, MvSerializableTypes.ReadSByte, MvSerializableTypes.WriteSByte);
        }

        [Test]
        public void ReadWriteChar()
        {
            TestReadWrite('a', MvSerializableTypes.ReadChar, MvSerializableTypes.WriteChar);
            TestReadWrite('A', MvSerializableTypes.ReadChar, MvSerializableTypes.WriteChar);
            TestReadWrite('∑', MvSerializableTypes.ReadChar, MvSerializableTypes.WriteChar);
        }

        [Test]
        public void ReadWriteShort()
        {
            TestReadWrite((short) 0, MvSerializableTypes.ReadShort, MvSerializableTypes.WriteShort);
            TestReadWrite((short) 256, MvSerializableTypes.ReadShort, MvSerializableTypes.WriteShort);
            TestReadWrite((short) -256, MvSerializableTypes.ReadShort, MvSerializableTypes.WriteShort);
            TestReadWrite(short.MinValue, MvSerializableTypes.ReadShort, MvSerializableTypes.WriteShort);
            TestReadWrite(short.MaxValue, MvSerializableTypes.ReadShort, MvSerializableTypes.WriteShort);
            TestReadWrite((ushort) 0, MvSerializableTypes.ReadUShort, MvSerializableTypes.WriteUShort);
            TestReadWrite((ushort) 256, MvSerializableTypes.ReadUShort, MvSerializableTypes.WriteUShort);
            TestReadWrite(ushort.MinValue, MvSerializableTypes.ReadUShort, MvSerializableTypes.WriteUShort);
            TestReadWrite(ushort.MaxValue, MvSerializableTypes.ReadUShort, MvSerializableTypes.WriteUShort);
        }

        [Test]
        public void ReadWriteInt()
        {
            TestReadWrite(0, MvSerializableTypes.ReadInt, MvSerializableTypes.WriteInt);
            TestReadWrite(256, MvSerializableTypes.ReadInt, MvSerializableTypes.WriteInt);
            TestReadWrite(-256, MvSerializableTypes.ReadInt, MvSerializableTypes.WriteInt);
            TestReadWrite(int.MinValue, MvSerializableTypes.ReadInt, MvSerializableTypes.WriteInt);
            TestReadWrite(int.MaxValue, MvSerializableTypes.ReadInt, MvSerializableTypes.WriteInt);
            TestReadWrite(256u, MvSerializableTypes.ReadUInt, MvSerializableTypes.WriteUInt);
            TestReadWrite(uint.MinValue, MvSerializableTypes.ReadUInt, MvSerializableTypes.WriteUInt);
            TestReadWrite(uint.MaxValue, MvSerializableTypes.ReadUInt, MvSerializableTypes.WriteUInt);
        }

        [Test]
        public void ReadWriteLong()
        {
            TestReadWrite(0L, MvSerializableTypes.ReadLong, MvSerializableTypes.WriteLong);
            TestReadWrite(256L, MvSerializableTypes.ReadLong, MvSerializableTypes.WriteLong);
            TestReadWrite(-256L, MvSerializableTypes.ReadLong, MvSerializableTypes.WriteLong);
            TestReadWrite(long.MinValue, MvSerializableTypes.ReadLong, MvSerializableTypes.WriteLong);
            TestReadWrite(long.MaxValue, MvSerializableTypes.ReadLong, MvSerializableTypes.WriteLong);
            TestReadWrite(256ul, MvSerializableTypes.ReadULong, MvSerializableTypes.WriteULong);
            TestReadWrite(ulong.MinValue, MvSerializableTypes.ReadULong, MvSerializableTypes.WriteULong);
            TestReadWrite(ulong.MaxValue, MvSerializableTypes.ReadULong, MvSerializableTypes.WriteULong);
        }

        [Test]
        public void ReadWriteFloat()
        {
            TestReadWrite(0f, MvSerializableTypes.ReadFloat, MvSerializableTypes.WriteFloat);
            TestReadWrite(256.256f, MvSerializableTypes.ReadFloat, MvSerializableTypes.WriteFloat);
            TestReadWrite(-256.256f, MvSerializableTypes.ReadFloat, MvSerializableTypes.WriteFloat);
            TestReadWrite(float.MinValue, MvSerializableTypes.ReadFloat, MvSerializableTypes.WriteFloat);
            TestReadWrite(float.MaxValue, MvSerializableTypes.ReadFloat, MvSerializableTypes.WriteFloat);
        }

        [Test]
        public void ReadWriteDouble()
        {
            TestReadWrite(0d, MvSerializableTypes.ReadDouble, MvSerializableTypes.WriteDouble);
            TestReadWrite(256.256d, MvSerializableTypes.ReadDouble, MvSerializableTypes.WriteDouble);
            TestReadWrite(-256.256d, MvSerializableTypes.ReadDouble, MvSerializableTypes.WriteDouble);
            TestReadWrite(double.MinValue, MvSerializableTypes.ReadDouble, MvSerializableTypes.WriteDouble);
            TestReadWrite(double.MaxValue, MvSerializableTypes.ReadDouble, MvSerializableTypes.WriteDouble);
        }

        [Test]
        public void ReadWriteDecimal()
        {
            TestReadWrite(0m, MvSerializableTypes.ReadDecimal, MvSerializableTypes.WriteDecimal);
            TestReadWrite(256.256m, MvSerializableTypes.ReadDecimal, MvSerializableTypes.WriteDecimal);
            TestReadWrite(-256.256m, MvSerializableTypes.ReadDecimal, MvSerializableTypes.WriteDecimal);
            TestReadWrite(decimal.MinValue, MvSerializableTypes.ReadDecimal, MvSerializableTypes.WriteDecimal);
            TestReadWrite(decimal.MaxValue, MvSerializableTypes.ReadDecimal, MvSerializableTypes.WriteDecimal);
        }

        [Test]
        public void ReadWriteString()
        {
            TestReadWrite("Test String", MvSerializableTypes.ReadString, MvSerializableTypes.WriteString);
            TestReadWrite("∑ Test String Unicode ∑", MvSerializableTypes.ReadString, MvSerializableTypes.WriteString);
        }

        [Test]
        public void ReadWriteByteArray()
        {
            TestReadWrite(null, MvSerializableTypes.ReadByteArray, MvSerializableTypes.WriteByteArray);
            TestReadWrite(new byte[] { }, MvSerializableTypes.ReadByteArray, MvSerializableTypes.WriteByteArray);
            TestReadWrite(new byte[] {0, 255, 100, 2}, MvSerializableTypes.ReadByteArray,
                MvSerializableTypes.WriteByteArray);
        }

        [Test]
        public void ReadWriteByteArraySegment()
        {
            TestReadWrite(new ArraySegment<byte>(new byte[] { }, 0, 0),
                MvSerializableTypes.ReadByteArraySegment, MvSerializableTypes.WriteByteArraySegment);
            TestReadWrite(new ArraySegment<byte>(new byte[] {0, 255, 100, 2}, 1, 2),
                MvSerializableTypes.ReadByteArraySegment, MvSerializableTypes.WriteByteArraySegment);
        }

        [Test]
        public void ReadWriteListInt()
        {
            TestReadWrite(null, MvSerializableTypes.ReadList<int>, MvSerializableTypes.WriteList);
            TestReadWrite(new List<int>(), MvSerializableTypes.ReadList<int>, MvSerializableTypes.WriteList);
            TestReadWrite(new List<int> {0, 255, 100, 2}, MvSerializableTypes.ReadList<int>,
                MvSerializableTypes.WriteList);
        }

        [Test]
        public void ReadWriteArrayInt()
        {
            TestReadWrite(null, MvSerializableTypes.ReadArray<int>, MvSerializableTypes.WriteArray);
            TestReadWrite(new int[] { }, MvSerializableTypes.ReadArray<int>, MvSerializableTypes.WriteArray);
            TestReadWrite(new[] {0, 255, 100, 2}, MvSerializableTypes.ReadArray<int>, MvSerializableTypes.WriteArray);
        }

        [Test]
        public void ReadWriteVector()
        {
            TestReadWrite(Vector2.zero, MvSerializableTypes.ReadVector2, MvSerializableTypes.WriteVector2);
            TestReadWrite(new Vector2(100, 200), MvSerializableTypes.ReadVector2, MvSerializableTypes.WriteVector2);
            TestReadWrite(Vector3.zero, MvSerializableTypes.ReadVector3, MvSerializableTypes.WriteVector3);
            TestReadWrite(new Vector3(100, 200, 300), MvSerializableTypes.ReadVector3,
                MvSerializableTypes.WriteVector3);
            TestReadWrite(Vector4.zero, MvSerializableTypes.ReadVector4, MvSerializableTypes.WriteVector4);
            TestReadWrite(new Vector4(100, 200, 300, 400), MvSerializableTypes.ReadVector4,
                MvSerializableTypes.WriteVector4);
            TestReadWrite(Vector2Int.zero, MvSerializableTypes.ReadVector2Int, MvSerializableTypes.WriteVector2Int);
            TestReadWrite(new Vector2Int(100, 200), MvSerializableTypes.ReadVector2Int,
                MvSerializableTypes.WriteVector2Int);
            TestReadWrite(Vector3Int.zero, MvSerializableTypes.ReadVector3Int, MvSerializableTypes.WriteVector3Int);
            TestReadWrite(new Vector3Int(100, 200, 300), MvSerializableTypes.ReadVector3Int,
                MvSerializableTypes.WriteVector3Int);
        }

        [Test]
        public void ReadWriteColor()
        {
            TestReadWrite(Color.clear, MvSerializableTypes.ReadColor, MvSerializableTypes.WriteColor);
            TestReadWrite(Color.red, MvSerializableTypes.ReadColor, MvSerializableTypes.WriteColor);
            TestReadWrite(Color.yellow, MvSerializableTypes.ReadColor, MvSerializableTypes.WriteColor);
            TestReadWrite(new Color32(0, 0, 0, 0), MvSerializableTypes.ReadColor32, MvSerializableTypes.WriteColor32);
            TestReadWrite(new Color32(100, 150, 200, 250), MvSerializableTypes.ReadColor32,
                MvSerializableTypes.WriteColor32);
        }

        [Test]
        public void ReadWriteQuaternion()
        {
            TestReadWrite(Quaternion.identity, MvSerializableTypes.ReadQuaternion, MvSerializableTypes.WriteQuaternion);
            TestReadWrite(Quaternion.Euler(10, 20, 30), MvSerializableTypes.ReadQuaternion,
                MvSerializableTypes.WriteQuaternion);
        }

        [Test]
        public void ReadWriteRect()
        {
            TestReadWrite(Rect.zero, MvSerializableTypes.ReadRect, MvSerializableTypes.WriteRect);
            TestReadWrite(new Rect(10, 20, 30, 40), MvSerializableTypes.ReadRect, MvSerializableTypes.WriteRect);
        }

        [Test]
        public void ReadWritePlane()
        {
            TestReadWrite(new Plane(Vector3.zero, 0), MvSerializableTypes.ReadPlane, MvSerializableTypes.WritePlane);
            TestReadWrite(new Plane(Vector3.one, 20), MvSerializableTypes.ReadPlane, MvSerializableTypes.WritePlane);
        }

        [Test]
        public void ReadWriteRay()
        {
            TestReadWrite(new Ray(), MvSerializableTypes.ReadRay, MvSerializableTypes.WriteRay);
            TestReadWrite(new Ray(Vector3.one, new Vector3(10, 20, 30)), MvSerializableTypes.ReadRay,
                MvSerializableTypes.WriteRay);
        }

        [Test]
        public void ReadWriteMatrix4x4()
        {
            TestReadWrite(Matrix4x4.zero, MvSerializableTypes.ReadMatrix4x4, MvSerializableTypes.WriteMatrix4x4);
            TestReadWrite(Matrix4x4.identity, MvSerializableTypes.ReadMatrix4x4, MvSerializableTypes.WriteMatrix4x4);
        }

        [Test]
        public void ReadWriteEnum()
        {
            TestReadWrite(TestEnum.OptionA, MvSerializableTypes.ReadEnum<TestEnum>, MvSerializableTypes.WriteEnum);
            TestReadWrite(TestEnum.OptionB, MvSerializableTypes.ReadEnum<TestEnum>, MvSerializableTypes.WriteEnum);
        }

        [Test]
        public void ReadWriteListEnum()
        {
            var enums = new List<TestEnum>
            {
                TestEnum.OptionA,
                TestEnum.OptionC,
                TestEnum.OptionD,
                TestEnum.OptionD
            };
            TestReadWrite(enums, MvSerializableTypes.ReadList<TestEnum>, MvSerializableTypes.WriteList);
        }

        [Test]
        public void ReadWriteArrayEnum()
        {
            var enums = new[]
            {
                TestEnum.OptionA,
                TestEnum.OptionC,
                TestEnum.OptionD,
                TestEnum.OptionD
            };
            TestReadWrite(enums, MvSerializableTypes.ReadArray<TestEnum>, MvSerializableTypes.WriteArray);
        }

        [Test]
        public void ReadWriteNetworkMessageClass()
        {
            // TODO: Timing tests vs. mirror in mirror tests folder
            // finally, send events!
            var b = new TestClassB {FloatProp = 20.25f, EnumField = TestEnum.OptionB};
            var a = new TestClassA {BProp = b, IntProp = 10, BField = b, IntField = 20};
            TestReadWrite(a, MvSerializableTypes.ReadNetworkMessageClass<TestClassA>,
                MvSerializableTypes.WriteNetworkMessageClass);
            TestReadWrite(b, MvSerializableTypes.ReadNetworkMessageClass<TestClassB>,
                MvSerializableTypes.WriteNetworkMessageClass);
            TestReadWrite(null, MvSerializableTypes.ReadNetworkMessageClass<TestClassA>,
                MvSerializableTypes.WriteNetworkMessageClass);
        }

        [Test]
        public void ReadWriteListNetworkMessageClass()
        {
            var classes = new List<TestClassA>
            {
                new TestClassA {BProp = null, IntProp = 10, BField = null, IntField = 20},
                new TestClassA {BProp = null, IntProp = 30, BField = null, IntField = 40},
                new TestClassA {BProp = null, IntProp = 40, BField = null, IntField = 60},
                null
            };
            TestReadWrite(classes, MvSerializableTypes.ReadList<TestClassA>, MvSerializableTypes.WriteList);
        }

        [Test]
        public void ReadWriteArrayNetworkMessageClass()
        {
            var classes = new[]
            {
                new TestClassA {BProp = null, IntProp = 10, BField = null, IntField = 20},
                new TestClassA {BProp = null, IntProp = 30, BField = null, IntField = 40},
                new TestClassA {BProp = null, IntProp = 50, BField = null, IntField = 60},
                null
            };
            TestReadWrite(classes, MvSerializableTypes.ReadArray<TestClassA>, MvSerializableTypes.WriteArray);
        }

        [Test]
        public void ReadWriteNetworkMessageStruct()
        {
            var b = new TestStructB {FloatProp = 20.25f, EnumField = TestEnum.OptionB};
            var a = new TestStructA {BProp = b, IntProp = 10, BField = b, IntField = 20};
            TestReadWrite(a, MvSerializableTypes.ReadNetworkMessageStruct<TestStructA>,
                MvSerializableTypes.WriteNetworkMessageStruct);
            TestReadWrite(b, MvSerializableTypes.ReadNetworkMessageStruct<TestStructB>,
                MvSerializableTypes.WriteNetworkMessageStruct);
        }

        [Test]
        public void ReadWriteNullable()
        {
            TestStructB? a = new TestStructB {FloatProp = 20.25f, EnumField = TestEnum.OptionB};
            TestReadWrite(a, MvSerializableTypes.ReadNullable<TestStructB>, MvSerializableTypes.WriteNullable);
            TestReadWrite(null, MvSerializableTypes.ReadNullable<TestStructB>, MvSerializableTypes.WriteNullable);
            TestReadWrite(10, MvSerializableTypes.ReadNullable<int>, MvSerializableTypes.WriteNullable);
            TestReadWrite(null, MvSerializableTypes.ReadNullable<int>, MvSerializableTypes.WriteNullable);
            TestReadWrite(Vector2.zero, MvSerializableTypes.ReadNullable<Vector2>, MvSerializableTypes.WriteNullable);
            TestReadWrite(null, MvSerializableTypes.ReadNullable<Vector2>, MvSerializableTypes.WriteNullable);
        }

        [Test]
        public void ReadWriteListNetworkMessageStruct()
        {
            var classes = new List<TestStructA>
            {
                new TestStructA {BProp = new TestStructB(), IntProp = 10, BField = new TestStructB(), IntField = 20},
                new TestStructA {BProp = new TestStructB(), IntProp = 30, BField = new TestStructB(), IntField = 40},
                new TestStructA {BProp = new TestStructB(), IntProp = 40, BField = new TestStructB(), IntField = 60}
            };
            TestReadWrite(classes, MvSerializableTypes.ReadList<TestStructA>, MvSerializableTypes.WriteList);
        }

        [Test]
        public void ReadWriteArrayNetworkMessageStruct()
        {
            var classes = new[]
            {
                new TestStructA {BProp = new TestStructB(), IntProp = 10, BField = new TestStructB(), IntField = 20},
                new TestStructA {BProp = new TestStructB(), IntProp = 30, BField = new TestStructB(), IntField = 40},
                new TestStructA {BProp = new TestStructB(), IntProp = 50, BField = new TestStructB(), IntField = 60}
            };
            TestReadWrite(classes, MvSerializableTypes.ReadArray<TestStructA>, MvSerializableTypes.WriteArray);
        }

        [Test]
        public void ReadWriteListListInt()
        {
            TestReadWrite(new List<List<int>>
            {
                new List<int> {1, 2, 3},
                new List<int> {4, 5, 6},
                new List<int> {7, 8, 9},
            }, MvSerializableTypes.ReadList<List<int>>, MvSerializableTypes.WriteList);
        }

        [Test]
        public void ReadWriteArrayArrayInt()
        {
            TestReadWrite(new[]
            {
                new[] {1, 2, 3},
                new[] {4, 5, 6},
                new[] {7, 8, 9},
            }, MvSerializableTypes.ReadArray<int[]>, MvSerializableTypes.WriteArray);
        }

        [Test]
        public void ReadWriteListArrayInt()
        {
            TestReadWrite(new List<int[]>
            {
                new[] {1, 2, 3},
                new[] {4, 5, 6},
                new[] {7, 8, 9},
            }, MvSerializableTypes.ReadList<int[]>, MvSerializableTypes.WriteList);
        }

        [Test]
        public void ReadWriteArrayListInt()
        {
            TestReadWrite(new[]
            {
                new List<int> {1, 2, 3},
                new List<int> {4, 5, 6},
                new List<int> {7, 8, 9},
            }, MvSerializableTypes.ReadArray<List<int>>, MvSerializableTypes.WriteArray);
        }

        [Test]
        public void ThrowsWriteInvalidClass()
        {
            Assert.Throws<MvException>(() => _writer.Write(new InvalidClass()));
        }

        [Test]
        public void ThrowsWriteInvalidStruct()
        {
            Assert.Throws<MvException>(() => _writer.Write(new InvalidStruct()));
        }

        [Test]
        public void ThrowsEndOfStream()
        {
            _writer.WriteInt(256);
            _reader = new MvBinaryReader(_writer.GetData());
            _reader.ReadInt();
            Assert.Throws<EndOfStreamException>(() => _reader.ReadInt());
        }

        private void TestReadWrite<T>(T value, Func<MvBinaryReader, T> read, Action<MvBinaryWriter, T> write)
        {
            write(_writer, value);
            write(_writer, value);
            var buffer = _writer.GetData();
            Assert.Greater(buffer.Count, 0);
            _reader = new MvBinaryReader(buffer);
            Assert.AreEqual(value, read(_reader));
            Assert.AreEqual(value, read(_reader));
            _writer.Clear();

            _writer.Write(value);
            _writer.Write(value);
            buffer = _writer.GetData();
            Assert.Greater(buffer.Count, 0);
            _reader = new MvBinaryReader(buffer);
            Assert.AreEqual(value, _reader.Read<T>());
            Assert.AreEqual(value, _reader.Read<T>());
            _writer.Clear();
        }

        private enum TestEnum
        {
            OptionA = 1,
            OptionB = 2,
            OptionC = 3,
            OptionD = 4
        }

        private class InvalidClass { }

        private class TestClassB : IMvNetworkMessage
        {
            public float FloatProp { get; set; }
            public TestEnum EnumField;

            private bool Equals(TestClassB other)
            {
                return EnumField.Equals(other.EnumField) && FloatProp.Equals(other.FloatProp);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((TestClassB) obj);
            }
        }

        private class TestClassA : IMvNetworkMessage
        {
            public TestClassB BProp { get; set; }
            public int IntProp { get; set; }
            public TestClassB BField;
            public int IntField;

            private bool Equals(TestClassA other)
            {
                return Equals(BField, other.BField) && IntField == other.IntField && Equals(BProp, other.BProp) &&
                       IntProp == other.IntProp;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((TestClassA) obj);
            }
        }

        private struct InvalidStruct { }

        private struct TestStructB : IMvNetworkMessage
        {
            public float FloatProp { get; set; }
            public TestEnum EnumField;

            private bool Equals(TestStructB other)
            {
                return EnumField.Equals(other.EnumField) && FloatProp.Equals(other.FloatProp);
            }

            public override bool Equals(object obj)
            {
                return obj is TestStructB other && Equals(other);
            }
        }

        private struct TestStructA : IMvNetworkMessage
        {
            public TestStructB BProp { get; set; }
            public int IntProp { get; set; }
            public TestStructB BField;
            public int IntField;

            private bool Equals(TestStructA other)
            {
                return BField.Equals(other.BField) && IntField == other.IntField && BProp.Equals(other.BProp) &&
                       IntProp == other.IntProp;
            }

            public override bool Equals(object obj)
            {
                return obj is TestStructA other && Equals(other);
            }
        }
    }
}