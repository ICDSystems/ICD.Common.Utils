using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Extensions;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Extensions
{
	[TestFixture]
	public sealed class ListExtensionsTest
	{
		[Test]
		public void AddSortedTest()
		{
			List<int> testList = new List<int>();

			testList.AddSorted(2);
			testList.AddSorted(3);
			testList.AddSorted(1);
			testList.AddSorted(2);

			Assert.AreEqual(4, testList.Count);
			Assert.AreEqual(1, testList[0]);
			Assert.AreEqual(2, testList[1]);
			Assert.AreEqual(2, testList[2]);
			Assert.AreEqual(3, testList[3]);
		}

		[Test]
		public void AddSortedComparerTest()
		{
			List<int> testList = new List<int>();
			IComparer<int> comparer = new InverseComparer();

			testList.AddSorted(2, comparer);
			testList.AddSorted(3, comparer);
			testList.AddSorted(1, comparer);
			testList.AddSorted(2, comparer);

			Assert.AreEqual(4, testList.Count);
			Assert.AreEqual(3, testList[0]);
			Assert.AreEqual(2, testList[1]);
			Assert.AreEqual(2, testList[2]);
			Assert.AreEqual(1, testList[3]);
		}

		[Test]
		public void PadRightTest()
		{
			List<int> testList = new List<int>();
			testList.PadRight(10);

			Assert.AreEqual(10, testList.Count);
			Assert.AreEqual(0, testList.Sum());

			testList = new List<int>
			{
				1,
				2,
				3,
				4,
				5
			};

			testList.PadRight(10);

			Assert.AreEqual(10, testList.Count);
			Assert.AreEqual(15, testList.Sum());

			testList = new List<int>
			{
				1,
				2,
				3,
				4,
				5
			};

			testList.PadRight(1);

			Assert.AreEqual(5, testList.Count);
			Assert.AreEqual(15, testList.Sum());
		}

		[Test]
		public void PadRightDefaultTest()
		{
			List<int> testList = new List<int>();
			testList.PadRight(10, 1);

			Assert.AreEqual(10, testList.Count);
			Assert.AreEqual(10, testList.Sum());

			testList = new List<int>
			{
				1,
				2,
				3,
				4,
				5
			};

			testList.PadRight(10, 1);

			Assert.AreEqual(10, testList.Count);
			Assert.AreEqual(20, testList.Sum());

			testList = new List<int>
			{
				1,
				2,
				3,
				4,
				5
			};

			testList.PadRight(1, 1);

			Assert.AreEqual(5, testList.Count);
			Assert.AreEqual(15, testList.Sum());
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
