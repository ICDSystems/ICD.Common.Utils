using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Extensions
{
	[TestFixture]
	public sealed class ListExtensionsTest
	{
		[Test]
		public void InsertSortedTest()
		{
			List<int> testList = new List<int>();

			Assert.AreEqual(0, testList.InsertSorted(2));
			Assert.AreEqual(1, testList.InsertSorted(3));
			Assert.AreEqual(0, testList.InsertSorted(1));
			Assert.AreEqual(1, testList.InsertSorted(2));

			Assert.AreEqual(4, testList.Count);
			Assert.AreEqual(1, testList[0]);
			Assert.AreEqual(2, testList[1]);
			Assert.AreEqual(2, testList[2]);
			Assert.AreEqual(3, testList[3]);
		}

		[Test]
		public void InsertSortedComparerTest()
		{
			List<int> testList = new List<int>();
			IComparer<int> comparer = new InverseComparer();

			Assert.AreEqual(0, testList.InsertSorted(2, comparer));
			Assert.AreEqual(0, testList.InsertSorted(3, comparer));
			Assert.AreEqual(2, testList.InsertSorted(1, comparer));
			Assert.AreEqual(1, testList.InsertSorted(2, comparer));

			Assert.AreEqual(4, testList.Count);
			Assert.AreEqual(3, testList[0]);
			Assert.AreEqual(2, testList[1]);
			Assert.AreEqual(2, testList[2]);
			Assert.AreEqual(1, testList[3]);
		}

		private sealed class InverseComparer : IComparer<int>
		{
			public int Compare(int x, int y)
			{
				return y.CompareTo(x);
			}
		}
	}
}
