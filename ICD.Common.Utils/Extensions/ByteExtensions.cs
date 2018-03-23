using System;

namespace ICD.Common.Utils.Extensions
{
	public static class ByteExtensions
	{
		public static bool GetBit(this byte b, int n)
		{
			if (n > 7 || n < 0)
				throw new ArgumentOutOfRangeException("n");

			return (b & (1 << n)) == (1 << n);
		}

		public static byte SetBit(this byte b, int n, bool v)
		{
			return v ? SetBitOn(b, n) : SetBitOff(b, n);
		}

		public static byte SetBitOn(this byte b, int n)
		{
			if (n > 7 || n < 0)
				throw new ArgumentOutOfRangeException("n");

			return (byte)(b | (1 << n));
		}

		public static byte SetBitOff(this byte b, int n)
		{
			if (n > 7 || n < 0)
				throw new ArgumentOutOfRangeException("n");

			return (byte)(b & ~(1 << n));
		}

		public static byte GetLower4Bits(this byte b)
		{
			return (byte)(b & 15);
		}

		public static byte GetUpper4Bits(this byte b)
		{
			return (byte)(b >> 4);
		}
	}
}
