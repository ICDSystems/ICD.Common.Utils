using System;

namespace ICD.Common.Utils
{
	public static class GuidUtils
	{
		public static Guid GenerateSeeded(int seed)
		{
			Random seeded = new Random(seed);
			byte[] bytes = new byte[16];

			seeded.NextBytes(bytes);

			return new Guid(bytes);
		}
	}
}
