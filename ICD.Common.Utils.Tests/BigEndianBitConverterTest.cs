using System;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ICD.Common.Utils.Tests
{
    [TestFixture]
    public class BigEndianBitConverterTest
    {
        [TestCase(ushort.MaxValue, new byte[] {0xFF, 0xFF}, 0)]
        [TestCase(ushort.MinValue, new byte[] {0x00, 0x00}, 0)]
        [TestCase((ushort)255, new byte[]{0x00, 0xFF}, 0)]
        [TestCase((ushort)65280, new byte[]{0xFF, 0x00},0)]
        [TestCase(ushort.MaxValue, new byte[] {0x00, 0x00, 0xFF, 0xFF, 0x00, 0x00}, 2)]
        [TestCase(ushort.MinValue, new byte[] { 0xFF, 0x00, 0x00, 0xFF, 0xFF}, 1)]
        [TestCase((ushort)255, new byte[]{0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0x00, 0x00, 0x00}, 3)]
        [TestCase((ushort)65280, new byte[]{0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0xFF, 0xFF},4)]
        [TestCase((ushort)240, new byte[]{0x00, 0xFF,0x00, 0xF0, 0xFF}, 2)]
        [TestCase((ushort)15, new byte[]{0x00, 0x0F},0)]
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
        
        [TestCase(short.MaxValue, new byte[] {0x7F, 0xFF}, 0)]
        [TestCase(short.MinValue, new byte[] {0x80, 0x00}, 0)]
        [TestCase(0, new byte[] {0x00, 0x00},0)]
        [TestCase(-1, new byte[] {0xFF, 0xFF}, 0)]
        [TestCase((short)255, new byte[]{0x00, 0xFF}, 0)]
        [TestCase((short)-256, new byte[]{0xFF, 0x00},0)]
        [TestCase(short.MaxValue, new byte[] {0x00, 0x00, 0x7F, 0xFF, 0x00, 0x00}, 2)]
        [TestCase(short.MinValue, new byte[] { 0xFF, 0x80, 0x00, 0xFF, 0xFF}, 1)]
        [TestCase((short)255, new byte[]{0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0x00, 0x00, 0x00}, 3)]
        [TestCase((short)-256, new byte[]{0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0xFF, 0xFF},4)]
        [TestCase((short)240, new byte[]{0x00, 0xFF,0x00, 0xF0, 0xFF}, 2)]
        [TestCase((short)15, new byte[]{0x00, 0x0F},0)]
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
        
        [TestCase(uint.MaxValue, new byte[] {0xFF, 0xFF, 0xFF, 0xFF}, 0)]
        [TestCase(uint.MinValue, new byte[] {0x00, 0x00, 0x00, 0x00}, 0)]
        [TestCase((uint)255, new byte[]{ 0x00, 0x00, 0x00, 0xFF}, 0)]
        [TestCase((uint)4278190080, new byte[]{0xFF, 0x00, 0x00, 0x00},0)]
        [TestCase(uint.MaxValue, new byte[] {0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00}, 2)]
        [TestCase(uint.MinValue, new byte[] { 0xFF, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF}, 1)]
        [TestCase((uint)255, new byte[]{0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00}, 3)]
        [TestCase((uint)4278190080, new byte[]{0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0xFF},4)]
        [TestCase((uint)15728895, new byte[]{0x00, 0xFF,0x00, 0xF0, 0x00, 0xFF, 0xFF}, 2)]
        [TestCase((uint)1044735, new byte[]{0x00, 0x0F, 0xF0, 0xFF},0)]
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
        
        [TestCase(int.MaxValue, new byte[] {0x7F, 0xFF, 0xFF, 0xFF}, 0)]
        [TestCase(int.MinValue, new byte[] {0x80, 0x00, 0x00, 0x00}, 0)]
        [TestCase(0, new byte[] {0x00, 0x00, 0x00, 0x00}, 0)]
        [TestCase(255, new byte[]{ 0x00, 0x00, 0x00, 0xFF}, 0)]
        [TestCase(-1, new byte[]{0xFF, 0xFF, 0xFF, 0xFF},0)]
        [TestCase(-16777216, new byte[]{0xFF, 0x00, 0x00, 0x00},0)]
        [TestCase(int.MaxValue, new byte[] {0x00, 0x00, 0x7F, 0xFF, 0xFF, 0xFF, 0x00, 0x00}, 2)]
        [TestCase(int.MinValue, new byte[] { 0xFF, 0x80, 0x00, 0x00, 0x00, 0xFF, 0xFF}, 1)]
        [TestCase(255, new byte[]{0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00}, 3)]
        [TestCase(-16777216, new byte[]{0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0xFF},4)]
        [TestCase(15728895, new byte[]{0x00, 0xFF,0x00, 0xF0, 0x00, 0xFF, 0xFF}, 2)]
        [TestCase(1044735, new byte[]{0x00, 0x0F, 0xF0, 0xFF},0)]
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
    }
}