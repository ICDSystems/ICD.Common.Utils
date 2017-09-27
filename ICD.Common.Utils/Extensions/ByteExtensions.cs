using System;

namespace ICD.Common.Utils.Extensions
{
	public static class ByteExtensions
	{
		public static bool GetBit(this byte b, int n)
		{
			if (n > 7 || n < 0)
				throw new ArgumentException();

			return (b & (1 << n)) == (1 << n);
		}

		public static byte SetBit(this byte b, int n, bool v)
		{
			if (v)
				return SetBitOn(b, n);
			return SetBitOff(b, n);
		}

		public static byte SetBitOn(this byte b, int n)
		{
			if (n > 7 || n < 0)
				throw new ArgumentException();

			return (byte)(b | (1 << n));
		}

		public static byte SetBitOff(this byte b, int n)
		{
			if (n > 7 || n < 0)
				throw new ArgumentException();

			return (byte)(b & ~(1 << n));
		}
	}
}
