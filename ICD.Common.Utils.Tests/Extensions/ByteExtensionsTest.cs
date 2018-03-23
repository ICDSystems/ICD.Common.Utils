using ICD.Common.Utils.Extensions;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Extensions
{
	[TestFixture]
	public sealed class ByteExtensionsTest
	{
		[TestCase(false, 0, 0)]
		[TestCase(true, 1, 0)]
		[TestCase(false, 247, 3)]
		[TestCase(true, 255, 3)]
		public void GetBitTest(bool expected, byte value, int index)
		{
			Assert.AreEqual(expected, value.GetBit(index));
		}

		[TestCase(1, 0, 0, true)]
		[TestCase(0, 1, 0, false)]
		[TestCase(247, 255, 3, false)]
		[TestCase(255, 247, 3, true)]
		public void SetBitTest(byte expected, byte value, int index, bool bitValue)
		{
			Assert.AreEqual(expected, value.SetBit(index, bitValue));
		}

		[TestCase(1, 0, 0)]
		[TestCase(1, 1, 0)]
		[TestCase(255, 247, 3)]
		public void SetBitOnTest(byte expected, byte value, int index)
		{
			Assert.AreEqual(expected, value.SetBitOn(index));
		}

		[TestCase(0, 0, 0)]
		[TestCase(0, 1, 0)]
		[TestCase(247, 255, 3)]
		public void SetBitOffTest(byte expected, byte value, int index)
		{
			Assert.AreEqual(expected, value.SetBitOff(index));
		}

		[TestCase(0xAB, 0x0B)]
		public void GetLower4BitsTest(byte b, byte expected)
		{
			Assert.AreEqual(expected, b.GetLower4Bits());
		}

		[TestCase(0xAB, 0x0A)]
		public void GetUpper4BitsTest(byte b, byte expected)
		{
			Assert.AreEqual(expected, b.GetUpper4Bits());
		}
	}
}
