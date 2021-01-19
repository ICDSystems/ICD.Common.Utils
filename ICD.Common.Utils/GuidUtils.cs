using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils
{
	public static class GuidUtils
	{
		/// <summary>
		/// Generates a pseudo-random guid from the given seed.
		/// </summary>
		public static Guid GenerateSeeded(int seed)
		{
			Random seeded = new Random(seed);
			byte[] bytes = new byte[16];

			seeded.NextBytes(bytes);

			return new Guid(bytes);
		}

		/// <summary>
		/// Combines the two guids to make a new, deterministic guid.
		/// </summary>
		public static Guid Combine(Guid a, Guid b)
		{
			byte[] aBytes = a.ToByteArray();
			byte[] bBytes = b.ToByteArray();

			for (int index = 0; index < aBytes.Length; index++)
				aBytes[index] = (byte)(aBytes[index] ^ bBytes[index]);

			return new Guid(aBytes);
		}

		/// <summary>
		/// Combines the guids in the given order to make a new, deterministic guid.
		/// </summary>
		public static Guid Combine(IEnumerable<Guid> guids)
		{
			return guids.AggregateOrDefault(Combine, default(Guid));
		}
	}
}
