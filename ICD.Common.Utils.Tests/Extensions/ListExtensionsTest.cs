using ICD.Common.Utils.Extensions;
using NUnit.Framework;
using System.Collections.Generic;
using System;

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

		internal class InverseComparer : IComparer<int>
		{
			public int Compare(int x, int y)
			{
				return y.CompareTo(x);
			}
		}
	}
}
