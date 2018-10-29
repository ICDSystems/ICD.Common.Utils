using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Collections;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Collections
{
	[TestFixture]
	public sealed class IcdHashSetTest
	{
		[Test, UsedImplicitly]
		public void CountTest()
		{
			Assert.AreEqual(3, new IcdHashSet<int>(new[] {1, 2, 2, 3}).Count);
		}

		[Test, UsedImplicitly]
		public void AddTest()
		{
			IcdHashSet<string> set = new IcdHashSet<string>();

			Assert.IsTrue(set.Add("One"));
			Assert.IsFalse(set.Add("One"));

			Assert.Throws<ArgumentNullException>(() => set.Add(null));
		}

		[Test, UsedImplicitly]
		public void AddRangeTest()
		{
			IcdHashSet<int> set = new IcdHashSet<int>();

			set.AddRange(new []{1, 2, 2, 3});

			Assert.AreEqual(3, set.Count);
		}

		[Test, UsedImplicitly]
		public void ClearTest()
		{
			IcdHashSet<int> set = new IcdHashSet<int>(new[] {1, 2, 2, 3});
			set.Clear();

			Assert.AreEqual(0, set.Count);
		}

		[Test, UsedImplicitly]
		public void ContainsTest()
		{
			IcdHashSet<int> set = new IcdHashSet<int>(new[] { 1, 2, 2, 3 });

			Assert.IsTrue(set.Contains(1));
			Assert.IsTrue(set.Contains(2));
			Assert.IsTrue(set.Contains(3));
			Assert.IsFalse(set.Contains(4));
		}

		[Test, UsedImplicitly]
		public void RemoveTest()
		{
			IcdHashSet<int> set = new IcdHashSet<int>(new[] { 1, 2, 2, 3 });
			set.Remove(2);

			Assert.IsFalse(set.Contains(2));
		}

		[Test, UsedImplicitly]
		public void UnionTest()
		{
			IcdHashSet<int> one = new IcdHashSet<int>(new[] { 1, 2 });
			IcdHashSet<int> two = new IcdHashSet<int>(new[] { 2, 3 });
			IcdHashSet<int> union = one.Union(two);

			Assert.AreEqual(3, union.Count);
		}

		[Test, UsedImplicitly]
		public void SubtractTest()
		{
			IcdHashSet<int> one = new IcdHashSet<int>(new[] { 1, 2 });
			IcdHashSet<int> two = new IcdHashSet<int>(new[] { 2, 3 });
			IcdHashSet<int> subtract = one.Subtract(two);

			Assert.AreEqual(1, subtract.Count);
			Assert.IsTrue(subtract.Contains(1));
		}

		[Test, UsedImplicitly]
		public void IsSubsetOfTest()
		{
			IcdHashSet<int> one = new IcdHashSet<int>(new[] { 1, 2 });
			IcdHashSet<int> two = new IcdHashSet<int>(new[] { 2, 3 });
			IcdHashSet<int> three = new IcdHashSet<int>(new[] { 1, 2, 3 });

			Assert.IsFalse(one.IsSubsetOf(two));
			Assert.IsTrue(one.IsSubsetOf(three));
		}

		[Test, UsedImplicitly]
		public void IntersectionTest()
		{
			IcdHashSet<int> one = new IcdHashSet<int>(new[] { 1, 2 });
			IcdHashSet<int> two = new IcdHashSet<int>(new[] { 2, 3 });
			IcdHashSet<int> intersection = one.Intersection(two);

			Assert.AreEqual(1, intersection.Count);
			Assert.IsTrue(intersection.Contains(2));
		}

		[Test, UsedImplicitly]
		public void NonIntersectionTest()
		{
			IcdHashSet<int> one = new IcdHashSet<int>(new[] { 1, 2 });
			IcdHashSet<int> two = new IcdHashSet<int>(new[] { 2, 3 });
			IcdHashSet<int> nonIntersection = one.NonIntersection(two);

			Assert.AreEqual(2, nonIntersection.Count);
			Assert.IsTrue(nonIntersection.Contains(1));
			Assert.IsTrue(nonIntersection.Contains(3));
		}

		[Test, UsedImplicitly]
		public void IsProperSubsetOfTest()
		{
			IcdHashSet<int> one = new IcdHashSet<int>(new[] { 1, 2 });
			IcdHashSet<int> two = new IcdHashSet<int>(new[] { 1, 2 });
			IcdHashSet<int> three = new IcdHashSet<int>(new[] { 1, 2, 3 });

			Assert.IsFalse(one.IsProperSubsetOf(two));
			Assert.IsTrue(one.IsProperSubsetOf(three));
		}

		[Test, UsedImplicitly]
		public void IsSupersetOfTest()
		{
			IcdHashSet<int> one = new IcdHashSet<int>(new[] { 1, 2 });
			IcdHashSet<int> two = new IcdHashSet<int>(new[] { 2, 3 });
			IcdHashSet<int> three = new IcdHashSet<int>(new[] { 1, 2, 3 });

			Assert.IsFalse(two.IsSupersetOf(one));
			Assert.IsTrue(three.IsSupersetOf(one));
		}

		[Test, UsedImplicitly]
		public void IsProperSupersetOfTest()
		{
			IcdHashSet<int> one = new IcdHashSet<int>(new[] { 1, 2 });
			IcdHashSet<int> two = new IcdHashSet<int>(new[] { 1, 2 });
			IcdHashSet<int> three = new IcdHashSet<int>(new[] { 1, 2, 3 });

			Assert.IsFalse(two.IsProperSupersetOf(one));
			Assert.IsTrue(three.IsProperSupersetOf(one));
		}
	}
}