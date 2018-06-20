using ICD.Common.Utils.Comparers;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Comparers
{
	[TestFixture]
	public sealed class SequenceComparerTest
	{
		[Test]
		public void CompareTest()
		{
			SequenceComparer<int> comparer = new SequenceComparer<int>();

			// Equal
			int[] a = {1, 2, 3};
			int[] b = {1, 2, 3};

			Assert.AreEqual(0, comparer.Compare(a, b));

			// A comes before B
			a = new[] {1, 2};
			b = new[] {1, 2, 3};

			Assert.AreEqual(-1, comparer.Compare(a, b));

			// B comes before A
			a = new[] { 2, 2, 3 };
			b = new[] { 1, 2, 3 };

			Assert.AreEqual(1, comparer.Compare(a, b));
		}
	}
}
