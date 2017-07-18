using ICD.Common.Properties;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests
{
	[TestFixture]
	public sealed class MathUtilsTest
	{
		[Test, UsedImplicitly]
		public void ClampTest()
		{
			Assert.AreEqual(MathUtils.Clamp(-10, 0, 0), 0);
			Assert.AreEqual(MathUtils.Clamp(-10, 10, 0), 0);
			Assert.AreEqual(MathUtils.Clamp(-10, 0, 10), 0);

			Assert.AreEqual(MathUtils.Clamp(0, 0, 0), 0);
			Assert.AreEqual(MathUtils.Clamp(0, 10, 0), 0);
			Assert.AreEqual(MathUtils.Clamp(0, 0, 10), 0);

			Assert.AreEqual(MathUtils.Clamp(20, 0, 10), 10);
			Assert.AreEqual(MathUtils.Clamp(20, 10, 0), 10);
			Assert.AreEqual(MathUtils.Clamp(20, 10, 10), 10);
		}

		[Test, UsedImplicitly]
		public void MapRangeTest()
		{
			Assert.AreEqual(0, MathUtils.MapRange(0, 100, 0, 10, 0));
			Assert.AreEqual(5, MathUtils.MapRange(0, 100, 0, 10, 50));
			Assert.AreEqual(10, MathUtils.MapRange(0, 100, 0, 10, 100));

			Assert.AreEqual(0, MathUtils.MapRange(0, 10, 0, 100, 0));
			Assert.AreEqual(50, MathUtils.MapRange(0, 10, 0, 100, 5));
			Assert.AreEqual(100, MathUtils.MapRange(0, 10, 0, 100, 10));
		}
	}
}
