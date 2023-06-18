using System;
using NUnit.Framework;
// ReSharper disable AssignNullToNotNullAttribute

namespace ICD.Common.Utils.Tests
{
    [TestFixture]
    public class BigEndianBitConverterTest
    {
        [TestCase(ushort.MaxValue, new byte[] { 0xFF, 0xFF }, 0)]
        [TestCase(ushort.MinValue, new byte[] { 0x00, 0x00 }, 0)]
        [TestCase((ushort)255, new byte[] { 0x00, 0xFF }, 0)]
        [TestCase((ushort)65280, new byte[] { 0xFF, 0x00 }, 0)]
        [TestCase(ushort.MaxValue, new byte[] { 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x00 }, 2)]
        [TestCase(ushort.MinValue, new byte[] { 0xFF, 0x00, 0x00, 0xFF, 0xFF }, 1)]
        [TestCase((ushort)255, new byte[] { 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0x00, 0x00, 0x00 }, 3)]
        [TestCase((ushort)65280, new byte[] { 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0xFF, 0xFF }, 4)]
        [TestCase((ushort)240, new byte[] { 0x00, 0xFF, 0x00, 0xF0, 0xFF }, 2)]
        [TestCase((ushort)15, new byte[] { 0x00, 0x0F }, 0)]
        public void ToUshortTest(ushort expectedResult, byte[] value, int startIndex)
        {
            // We use system BitConverter for systems that are already big-endian
            if (!BitConverter.IsLittleEndian)
                Assert.Inconclusive();


            Assert.AreEqual(expectedResult, BigEndianBitConverter.ToUshort(value, startIndex));
        }

        [Test]
        public void ToUshortExceptionTest()
        {
            // We use system BitConverter for systems that are already big-endian
            if (!BitConverter.IsLittleEndian)
                Assert.Inconclusive();

            byte[] values = { 0x00, 0xFF, 0x0F, 0xF0, 0xFF, 0xAA, 0x55, 0xF1, 0x1F };

            Assert.Throws<ArgumentNullException>(() => BigEndianBitConverter.ToUshort(null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => BigEndianBitConverter.ToUshort(values, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => BigEndianBitConverter.ToUshort(values, 9));
            Assert.Throws<ArgumentException>(() => BigEndianBitConverter.ToUshort(values, 8));
        }

        [TestCase(short.MaxValue, new byte[] { 0x7F, 0xFF }, 0)]
        [TestCase(short.MinValue, new byte[] { 0x80, 0x00 }, 0)]
        [TestCase(0, new byte[] { 0x00, 0x00 }, 0)]
        [TestCase(-1, new byte[] { 0xFF, 0xFF }, 0)]
        [TestCase((short)255, new byte[] { 0x00, 0xFF }, 0)]
        [TestCase((short)-256, new byte[] { 0xFF, 0x00 }, 0)]
        [TestCase(short.MaxValue, new byte[] { 0x00, 0x00, 0x7F, 0xFF, 0x00, 0x00 }, 2)]
        [TestCase(short.MinValue, new byte[] { 0xFF, 0x80, 0x00, 0xFF, 0xFF }, 1)]
        [TestCase((short)255, new byte[] { 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0x00, 0x00, 0x00 }, 3)]
        [TestCase((short)-256, new byte[] { 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0xFF, 0xFF }, 4)]
        [TestCase((short)240, new byte[] { 0x00, 0xFF, 0x00, 0xF0, 0xFF }, 2)]
        [TestCase((short)15, new byte[] { 0x00, 0x0F }, 0)]
        public void ToShortTest(short expectedResult, byte[] value, int startIndex)
        {
            // We use system BitConverter for systems that are already big-endian
            if (!BitConverter.IsLittleEndian)
                Assert.Inconclusive();


            Assert.AreEqual(expectedResult, BigEndianBitConverter.ToShort(value, startIndex));
        }

        [Test]
        public void ToShortExceptionTest()
        {
            // We use system BitConverter for systems that are already big-endian
            if (!BitConverter.IsLittleEndian)
                Assert.Inconclusive();

            byte[] values = { 0x00, 0xFF, 0x0F, 0xF0, 0xFF, 0xAA, 0x55, 0xF1, 0x1F };

            Assert.Throws<ArgumentNullException>(() => BigEndianBitConverter.ToShort(null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => BigEndianBitConverter.ToShort(values, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => BigEndianBitConverter.ToShort(values, 9));
            Assert.Throws<ArgumentException>(() => BigEndianBitConverter.ToShort(values, 8));
        }

        [TestCase(uint.MaxValue, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, 0)]
        [TestCase(uint.MinValue, new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0)]
        [TestCase((uint)255, new byte[] { 0x00, 0x00, 0x00, 0xFF }, 0)]
        [TestCase(4278190080, new byte[] { 0xFF, 0x00, 0x00, 0x00 }, 0)]
        [TestCase(uint.MaxValue, new byte[] { 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00 }, 2)]
        [TestCase(uint.MinValue, new byte[] { 0xFF, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF }, 1)]
        [TestCase((uint)255, new byte[] { 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00 }, 3)]
        [TestCase(4278190080, new byte[] { 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0xFF }, 4)]
        [TestCase((uint)15728895, new byte[] { 0x00, 0xFF, 0x00, 0xF0, 0x00, 0xFF, 0xFF }, 2)]
        [TestCase((uint)1044735, new byte[] { 0x00, 0x0F, 0xF0, 0xFF }, 0)]
        public void ToUintTest(uint expectedResult, byte[] value, int startIndex)
        {
            // We use system BitConverter for systems that are already big-endian
            if (!BitConverter.IsLittleEndian)
                Assert.Inconclusive();


            Assert.AreEqual(expectedResult, BigEndianBitConverter.ToUint(value, startIndex));
        }

        [Test]
        public void ToUintExceptionTest()
        {
            // We use system BitConverter for systems that are already big-endian
            if (!BitConverter.IsLittleEndian)
                Assert.Inconclusive();

            byte[] values = { 0x00, 0xFF, 0x0F, 0xF0, 0xFF, 0xAA, 0x55, 0xF1, 0x1F };

            Assert.Throws<ArgumentNullException>(() => BigEndianBitConverter.ToUint(null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => BigEndianBitConverter.ToUint(values, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => BigEndianBitConverter.ToUint(values, 9));
            Assert.Throws<ArgumentException>(() => BigEndianBitConverter.ToUint(values, 6));
        }

        [TestCase(int.MaxValue, new byte[] { 0x7F, 0xFF, 0xFF, 0xFF }, 0)]
        [TestCase(int.MinValue, new byte[] { 0x80, 0x00, 0x00, 0x00 }, 0)]
        [TestCase(0, new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0)]
        [TestCase(255, new byte[] { 0x00, 0x00, 0x00, 0xFF }, 0)]
        [TestCase(-1, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, 0)]
        [TestCase(-16777216, new byte[] { 0xFF, 0x00, 0x00, 0x00 }, 0)]
        [TestCase(int.MaxValue, new byte[] { 0x00, 0x00, 0x7F, 0xFF, 0xFF, 0xFF, 0x00, 0x00 }, 2)]
        [TestCase(int.MinValue, new byte[] { 0xFF, 0x80, 0x00, 0x00, 0x00, 0xFF, 0xFF }, 1)]
        [TestCase(255, new byte[] { 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00 }, 3)]
        [TestCase(-16777216, new byte[] { 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0xFF }, 4)]
        [TestCase(15728895, new byte[] { 0x00, 0xFF, 0x00, 0xF0, 0x00, 0xFF, 0xFF }, 2)]
        [TestCase(1044735, new byte[] { 0x00, 0x0F, 0xF0, 0xFF }, 0)]
        public void ToIntTest(int expectedResult, byte[] value, int startIndex)
        {
            // We use system BitConverter for systems that are already big-endian
            if (!BitConverter.IsLittleEndian)
                Assert.Inconclusive();


            Assert.AreEqual(expectedResult, BigEndianBitConverter.ToInt(value, startIndex));
        }

        [Test]
        public void ToIntExceptionTest()
        {
            // We use system BitConverter for systems that are already big-endian
            if (!BitConverter.IsLittleEndian)
                Assert.Inconclusive();

            byte[] values = { 0x00, 0xFF, 0x0F, 0xF0, 0xFF, 0xAA, 0x55, 0xF1, 0x1F };

            Assert.Throws<ArgumentNullException>(() => BigEndianBitConverter.ToInt(null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => BigEndianBitConverter.ToInt(values, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => BigEndianBitConverter.ToInt(values, 9));
            Assert.Throws<ArgumentException>(() => BigEndianBitConverter.ToInt(values, 6));
        }

        [TestCase(ulong.MaxValue, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, 0)]
        [TestCase(ulong.MinValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0)]
        [TestCase((ulong)255, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF }, 0)]
        [TestCase(18374686479671623680, new byte[] { 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0)]
        [TestCase(ulong.MaxValue, new byte[] { 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00 },
            2)]
        [TestCase(ulong.MinValue, new byte[] { 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF }, 1)]
        [TestCase((ulong)255,
            new byte[] { 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00 }, 3)]
        [TestCase(18374686479671623680,
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF }, 4)]
        [TestCase((ulong)67555089642946815,
            new byte[] { 0x00, 0xFF, 0x00, 0xF0, 0x00, 0xFF, 0x00, 0xF0, 0x00, 0xFF, 0xFF }, 2)]
        [TestCase((ulong)4487102673719295, new byte[] { 0x00, 0x0F, 0xF0, 0xFF, 0x00, 0xF0, 0x0F, 0xFF }, 0)]
        public void ToUlongTest(ulong expectedResult, byte[] value, int startIndex)
        {
            // We use system BitConverter for systems that are already big-endian
            if (!BitConverter.IsLittleEndian)
                Assert.Inconclusive();


            Assert.AreEqual(expectedResult, BigEndianBitConverter.ToUlong(value, startIndex));
        }

        [Test]
        public void ToUlongExceptionTest()
        {
            // We use system BitConverter for systems that are already big-endian
            if (!BitConverter.IsLittleEndian)
                Assert.Inconclusive();

            byte[] values = { 0x00, 0xFF, 0x0F, 0xF0, 0xFF, 0xAA, 0x55, 0xF1, 0x1F };

            Assert.Throws<ArgumentNullException>(() => BigEndianBitConverter.ToUlong(null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => BigEndianBitConverter.ToUlong(values, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => BigEndianBitConverter.ToUlong(values, 9));
            Assert.Throws<ArgumentException>(() => BigEndianBitConverter.ToUlong(values, 2));
        }

        [TestCase(long.MaxValue, new byte[] { 0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, 0)]
        [TestCase(long.MinValue, new byte[] { 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0)]
        [TestCase(0, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0)]
        [TestCase(255, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF }, 0)]
        [TestCase(-1, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, 0)]
        [TestCase(-72057594037927936, new byte[] { 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0)]
        [TestCase(long.MaxValue, new byte[] { 0x00, 0x00, 0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00 },
            2)]
        [TestCase(long.MinValue, new byte[] { 0xFF, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF }, 1)]
        [TestCase(255,
            new byte[] { 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00 }, 3)]
        [TestCase(-72057594037927936,
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF }, 4)]
        [TestCase(67555093906456320, new byte[] { 0x00, 0xFF, 0x00, 0xF0, 0x00, 0xFF, 0xFF, 0x0F, 0xFF, 0x00, 0xF0 },
            2)]
        [TestCase(4487102659035120, new byte[] { 0x00, 0x0F, 0xF0, 0xFF, 0x00, 0x0F, 0xFF, 0xF0 }, 0)]
        public void ToLongTest(long expectedResult, byte[] value, int startIndex)
        {
            // We use system BitConverter for systems that are already big-endian
            if (!BitConverter.IsLittleEndian)
                Assert.Inconclusive();


            Assert.AreEqual(expectedResult, BigEndianBitConverter.ToLong(value, startIndex));
        }

        [Test]
        public void ToLongExceptionTest()
        {
            // We use system BitConverter for systems that are already big-endian
            if (!BitConverter.IsLittleEndian)
                Assert.Inconclusive();

            byte[] values = { 0x00, 0xFF, 0x0F, 0xF0, 0xFF, 0xAA, 0x55, 0xF1, 0x1F };

            Assert.Throws<ArgumentNullException>(() => BigEndianBitConverter.ToLong(null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => BigEndianBitConverter.ToLong(values, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => BigEndianBitConverter.ToLong(values, 9));
            Assert.Throws<ArgumentException>(() => BigEndianBitConverter.ToLong(values, 2));
        }

        [TestCase(new byte[] { 0x7F, 0xFF, 0xFF, 0xFF }, int.MaxValue)]
        [TestCase(new byte[] { 0x80, 0x00, 0x00, 0x00 }, int.MinValue)]
        [TestCase(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0)]
        [TestCase(new byte[] { 0x00, 0x00, 0x00, 0xFF }, 255)]
        [TestCase(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, -1)]
        [TestCase(new byte[] { 0xFF, 0x00, 0x00, 0x00 }, -16777216)]
        [TestCase(new byte[] { 0x00, 0xF0, 0x00, 0xFF }, 15728895)]
        [TestCase(new byte[] { 0x00, 0x0F, 0xF0, 0xFF }, 1044735)]
        public void GetBytesIntTest(byte[] expectedResult, int value)
        {
            // We use system BitConverter for systems that are already big-endian
            if (!BitConverter.IsLittleEndian)
                Assert.Inconclusive();

            CollectionAssert.AreEqual(expectedResult, BigEndianBitConverter.GetBytes(value));
        }

        [TestCase(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, uint.MaxValue)]
        [TestCase(new byte[] { 0x00, 0x00, 0x00, 0x00 }, uint.MinValue)]
        [TestCase(new byte[] { 0x00, 0x00, 0x00, 0xFF }, (uint)255)]
        [TestCase(new byte[] { 0xFF, 0x00, 0x00, 0x00 }, 4278190080)]
        [TestCase(new byte[] { 0x00, 0x00, 0x00, 0xFF }, (uint)255)]
        [TestCase(new byte[] { 0x00, 0xF0, 0x00, 0xFF }, (uint)15728895)]
        [TestCase(new byte[] { 0x00, 0x0F, 0xF0, 0xFF }, (uint)1044735)]
        public void GetBytesUintTest(byte[] expectedResult, uint value)
        {
            // We use system BitConverter for systems that are already big-endian
            if (!BitConverter.IsLittleEndian)
                Assert.Inconclusive();

            CollectionAssert.AreEqual(expectedResult, BigEndianBitConverter.GetBytes(value));
        }

        [TestCase(new byte[] { 0x7F, 0xFF }, short.MaxValue)]
        [TestCase(new byte[] { 0x80, 0x00 }, short.MinValue)]
        [TestCase(new byte[] { 0x00, 0x00 }, (short)0)]
        [TestCase(new byte[] { 0xFF, 0xFF }, (short)-1)]
        [TestCase(new byte[] { 0x00, 0xFF }, (short)255)]
        [TestCase(new byte[] { 0xFF, 0x00 }, (short)-256)]
        [TestCase(new byte[] { 0x00, 0xF0 }, (short)240)]
        [TestCase(new byte[] { 0x00, 0x0F }, (short)15)]
        public void GetBytesShortTest(byte[] expectedResult, short value)
        {
            // We use system BitConverter for systems that are already big-endian
            if (!BitConverter.IsLittleEndian)
                Assert.Inconclusive();

            CollectionAssert.AreEqual(expectedResult, BigEndianBitConverter.GetBytes(value));
        }

        [TestCase(new byte[] { 0xFF, 0xFF }, ushort.MaxValue)]
        [TestCase(new byte[] { 0x00, 0x00 }, ushort.MinValue)]
        [TestCase(new byte[] { 0x00, 0xFF }, (ushort)255)]
        [TestCase(new byte[] { 0xFF, 0x00 }, (ushort)65280)]
        [TestCase(new byte[] { 0x00, 0xF0 }, (ushort)240)]
        [TestCase(new byte[] { 0x00, 0x0F }, (ushort)15)]
        public void GetBytesUshortTest(byte[] expectedResult, ushort value)
        {
            // We use system BitConverter for systems that are already big-endian
            if (!BitConverter.IsLittleEndian)
                Assert.Inconclusive();

            CollectionAssert.AreEqual(expectedResult, BigEndianBitConverter.GetBytes(value));
        }

        [TestCase(new byte[] { 0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, long.MaxValue)]
        [TestCase(new byte[] { 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },long.MinValue)]
        [TestCase(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, (long)0)]
        [TestCase(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF }, (long)255)]
        [TestCase(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, (long)-1)]
        [TestCase(new byte[] { 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, -72057594037927936)]
        [TestCase(new byte[] { 0x00, 0xF0, 0x00, 0xFF, 0xFF, 0x0F, 0xFF, 0x00 }, 67555093906456320)]
        [TestCase(new byte[] { 0x00, 0x0F, 0xF0, 0xFF, 0x00, 0x0F, 0xFF, 0xF0 }, 4487102659035120)]
        public void GetBytesLongTest(byte[] expectedResult, long value)
        {
            // We use system BitConverter for systems that are already big-endian
            if (!BitConverter.IsLittleEndian)
                Assert.Inconclusive();

            CollectionAssert.AreEqual(expectedResult, BigEndianBitConverter.GetBytes(value));
        }

        [TestCase(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, ulong.MaxValue)]
        [TestCase(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, ulong.MinValue)]
        [TestCase(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF }, (ulong)255)]
        [TestCase(new byte[] { 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 18374686479671623680)]
        [TestCase(new byte[] { 0x00, 0xF0, 0x00, 0xFF, 0x00, 0xF0, 0x00, 0xFF }, (ulong)67555089642946815)]
        [TestCase(new byte[] { 0x00, 0x0F, 0xF0, 0xFF, 0x00, 0xF0, 0x0F, 0xFF }, (ulong)4487102673719295)]
        public void GetBytesUlongTest(byte[] expectedResult, ulong value)
        {
            // We use system BitConverter for systems that are already big-endian
            if (!BitConverter.IsLittleEndian)
                Assert.Inconclusive();

            CollectionAssert.AreEqual(expectedResult, BigEndianBitConverter.GetBytes(value));
        }

    }
}