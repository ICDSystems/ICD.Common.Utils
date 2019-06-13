using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;

namespace ICD.Common.Utils.Tests
{
	[TestFixture, UsedImplicitly]
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
			Assert.AreEqual(5, MathUtils.MapRange(-100, 100, 0, 10, 0));
			Assert.AreEqual(7, MathUtils.MapRange(-100, 100, 0, 10, 50));
			Assert.AreEqual(10, MathUtils.MapRange(-100, 100, 0, 10, 100));

			Assert.AreEqual(0, MathUtils.MapRange(0, 100, 0, 10, 0));
			Assert.AreEqual(5, MathUtils.MapRange(0, 100, 0, 10, 50));
			Assert.AreEqual(10, MathUtils.MapRange(0, 100, 0, 10, 100));

			Assert.AreEqual(0, MathUtils.MapRange(0, 10, 0, 100, 0));
			Assert.AreEqual(50, MathUtils.MapRange(0, 10, 0, 100, 5));
			Assert.AreEqual(100, MathUtils.MapRange(0, 10, 0, 100, 10));
		}

		[Test, UsedImplicitly]
		public void GetRangesTest()
        {
            IEnumerable<int> values = new [] { 1, 3, 5, 6, 7, 8, 9, 10, 12 };
            int[][] ranges = MathUtils.GetRanges(values).ToArray();

            Assert.AreEqual(4, ranges.Length);

            Assert.AreEqual(1, ranges[0][0]);
            Assert.AreEqual(1, ranges[0][1]);

            Assert.AreEqual(3, ranges[1][0]);
            Assert.AreEqual(3, ranges[1][1]);

            Assert.AreEqual(5, ranges[2][0]);
            Assert.AreEqual(10, ranges[2][1]);

            Assert.AreEqual(12, ranges[3][0]);
            Assert.AreEqual(12, ranges[3][1]);
        }

        [Test, UsedImplicitly]
        public void RoundToNearestTest()
        {
            IEnumerable<int> values = new [] { 0, 15, 30, 45 };
            Assert.AreEqual(15, MathUtils.RoundToNearest(21, values));
        }
    }
}
