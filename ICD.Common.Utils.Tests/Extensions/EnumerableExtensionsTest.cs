using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ICD.Common.Utils.Tests.Extensions
{
	[TestFixture]
    public sealed class EnumerableExtensionsTest
    {
        [TestCase(0)]
        [TestCase(1)]
		public void FirstOrDefaultTest(int expected)
        {
            IEnumerable<int> sequence = Enumerable.Empty<int>();
            Assert.AreEqual(expected, sequence.FirstOrDefault(expected));
        }

        [TestCase(0)]
        [TestCase(1)]
        public void FirstOrDefaultPredicateTest(int expected)
        {
            IEnumerable<int> sequence = Enumerable.Range(1, 10).Except(expected);
            Assert.AreEqual(expected, sequence.FirstOrDefault(i => i == expected, expected));
        }

        [TestCase(0)]
        [TestCase(1)]
        public void TryFirstTest(int expected)
        {
            IEnumerable<int> sequence = Enumerable.Empty<int>();
            int result;
            bool exists = sequence.TryFirst(out result);

            Assert.AreEqual(0, result);
            Assert.AreEqual(false, exists);

            sequence = new int[] { expected };
            exists = sequence.TryFirst(out result);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(true, exists);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void TryFirstPredicateTest(int expected)
        {
            IEnumerable<int> sequence = Enumerable.Range(1, 10).Except(expected);
            int result;
            bool exists = sequence.TryFirst(i => i == expected, out result);

            Assert.AreEqual(0, result);
            Assert.AreEqual(false, exists);

            sequence = new int[] { expected };
            exists = sequence.TryFirst(i => i == expected, out result);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(true, exists);
        }

        [TestCase(0, 0)]
        [TestCase(1, 10)]
        public void TryElementAtTest(int index, int expected)
        {
            IEnumerable<int> sequence = Enumerable.Empty<int>();
            int result;
            bool exists = sequence.TryElementAt(index, out result);

            Assert.AreEqual(0, result);
            Assert.AreEqual(false, exists);

            sequence = new int[] { expected };
            exists = sequence.TryElementAt(0, out result);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(true, exists);
        }

        [Test]
        public void SequenceEqualTest()
        {
            int[] a = new int[] { 1, 2, 3, 4};
            int[] b = new int[] { 1, 4, 9, 16};

            Assert.IsFalse(a.SequenceEqual(b, (x, y) => x == y));
            Assert.IsTrue(a.SequenceEqual(b, (x, y) => x * x == y));
        }

        [Test]
        public void ScrambledEqualsTest()
        {
            IEnumerable<int> a = new int[] { 1, 2, 3, 4 };
            IEnumerable<int> b = new int[] { 3, 1, 2, 4 };
            IEnumerable<int> c = new int[] { 1, 2, 3, 4, 4 };
            IEnumerable<int> d = new int[] { 1, 2, 3, 4, 5 };

            Assert.IsTrue(a.ScrambledEquals(b));
            Assert.IsFalse(a.ScrambledEquals(c));
            Assert.IsFalse(a.ScrambledEquals(d));
        }

        [Test]
        public void ScrambledEqualsComparerTest()
        {
            IEnumerable<int> a = new int[] { 1, 2, 3, 4 };
            IEnumerable<int> b = new int[] { 3, 1, 2, 4 };
            IEnumerable<int> c = new int[] { 1, 2, 3, 4, 4 };
            IEnumerable<int> d = new int[] { 1, 2, 3, 4, 5 };

            Assert.IsTrue(a.ScrambledEquals(b, EqualityComparer<int>.Default));
            Assert.IsFalse(a.ScrambledEquals(c, EqualityComparer<int>.Default));
            Assert.IsFalse(a.ScrambledEquals(d, EqualityComparer<int>.Default));
        }

        [Test]
        public void FindIndexPredicateTest()
        {
            IEnumerable<int> a = new int[] { 1, 2, 3, 4 };
            Assert.AreEqual(2, a.FindIndex(i => i == 3));
        }

        [Test]
        public void SelectMultiTest()
        {
            int[] values = { 1, 2, 3 };
            int[] result = values.SelectMulti(i => 1, i => 2).ToArray();

            Assert.AreEqual(6, result.Length);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(1, result[2]);
            Assert.AreEqual(2, result[3]);
            Assert.AreEqual(1, result[4]);
            Assert.AreEqual(2, result[5]);
        }

        [Test]
        public void ExecuteTest()
        {
            int[] values = { 1, 2, 3 };

            int sum = 0;
            IEnumerable<int> sequence = values.Select(v => { sum += v; return v; });

            Assert.AreEqual(0, sum);
            sequence.Execute();
            Assert.AreEqual(6, sum);
        }

        [Test]
        public void ForEachTest()
        {
            int[] values = { 1, 2, 3 };

            int sum = 0;
            values.ForEach(v => sum += v);

            Assert.AreEqual(6, sum);
        }

        [Test]
        public void ForEachIndexTest()
        {
            int[] values = { 1, 2, 3 };

            int sum = 0;
            values.ForEach((v, i) => sum += i);

            Assert.AreEqual(3, sum);
        }

#if SIMPLSHARP
        [Test]
        public void PrependTest()
		{
			Assert.Inconclusive();
		}
#endif

        [Test]
        public void PrependManyTest()
        {
            int[] values = (new int[] { 4, 5, 6 }).PrependMany(1, 2, 3).ToArray();

            Assert.AreEqual(6, values.Length);
            Assert.AreEqual(1, values[0]);
            Assert.AreEqual(2, values[1]);
            Assert.AreEqual(3, values[2]);
            Assert.AreEqual(4, values[3]);
            Assert.AreEqual(5, values[4]);
            Assert.AreEqual(6, values[5]);
        }

#if SIMPLSHARP
		[Test]
		public void AppendTest()
		{
			Assert.Inconclusive();
		}
#endif

        [Test]
        public void AppendManyTest()
        {
            int[] values = (new int[] { 1, 2, 3 }).AppendMany(4, 5, 6).ToArray();

            Assert.AreEqual(6, values.Length);
            Assert.AreEqual(1, values[0]);
            Assert.AreEqual(2, values[1]);
            Assert.AreEqual(3, values[2]);
            Assert.AreEqual(4, values[3]);
            Assert.AreEqual(5, values[4]);
            Assert.AreEqual(6, values[5]);
        }

        [Test]
        public void OrderTest()
        {
            int[] values = (new int[] { 2, 3, 1 }).Order().ToArray();

            Assert.AreEqual(3, values.Length);
            Assert.AreEqual(1, values[0]);
            Assert.AreEqual(2, values[1]);
            Assert.AreEqual(3, values[2]);
        }

        [Test]
        public void ExceptTest()
        {
            int[] values = (new int[] { 1, 2, 3 }).Except(2).ToArray();

            Assert.AreEqual(2, values.Length);
            Assert.AreEqual(1, values[0]);
            Assert.AreEqual(3, values[1]);
        }

        [Test]
        public void ToHashSetTest()
        {
            IcdHashSet<int> values = (new int[] { 1, 2, 3 }).ToHashSet();

            Assert.AreEqual(3, values.Count);
            Assert.IsTrue(values.Contains(1));
            Assert.IsTrue(values.Contains(2));
            Assert.IsTrue(values.Contains(3));
        }

        [Test]
        public void ToDictionaryIntTest()
        {
            Dictionary<int, int> values = (new int[] { 1, 2, 3 }).ToDictionary();

            Assert.AreEqual(3, values.Count);
            Assert.AreEqual(1, values[0]);
            Assert.AreEqual(2, values[1]);
            Assert.AreEqual(3, values[2]);
        }

        [Test]
        public void ToDictionaryUIntTest()
        {
            Dictionary<uint, int> values = (new int[] { 1, 2, 3 }).ToDictionaryUInt();

            Assert.AreEqual(3, values.Count);
            Assert.AreEqual(1, values[0]);
            Assert.AreEqual(2, values[1]);
            Assert.AreEqual(3, values[2]);
        }

        [Test]
        public void ToDictionaryTest()
        {
            KeyValuePair<int, string>[] items =
            {
                new KeyValuePair<int, string>(0, "A"),
                new KeyValuePair<int, string>(1, "B"),
                new KeyValuePair<int, string>(2, "C")
            };

            Dictionary<int, string> values = items.ToDictionary();

            Assert.AreEqual(3, values.Count);
            Assert.AreEqual("A", values[0]);
            Assert.AreEqual("B", values[1]);
            Assert.AreEqual("C", values[2]);
        }

        [Test]
        public void UnanimousTest()
        {
            Assert.IsTrue((new bool[] { true, true, true}).Unanimous());
            Assert.IsTrue((new bool[] { false, false, false }).Unanimous());
            Assert.IsFalse((new bool[] { false, true, false }).Unanimous());
            Assert.IsFalse((new bool[] { }).Unanimous());
        }

        [Test]
        public void UnanimousOtherTest()
        {
            Assert.AreEqual("A", (new string[] { "A", "A", "A" }).Unanimous("B"));
            Assert.AreEqual("C", (new string[] { "B", "A", "B" }).Unanimous("C"));
            Assert.AreEqual("A", (new string[] { }).Unanimous("A"));
        }

        [Test]
        public void PartitionTest()
        {
            int[][] items = (new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }).Partition(3)
                .Select(p => p.ToArray())
                .ToArray();

            Assert.AreEqual(3, items.Length);

            Assert.AreEqual(3, items[0].Length);
            Assert.AreEqual(3, items[1].Length);
            Assert.AreEqual(2, items[2].Length);

            Assert.AreEqual(1, items[0][0]);
            Assert.AreEqual(2, items[0][1]);
            Assert.AreEqual(3, items[0][2]);

            Assert.AreEqual(4, items[1][0]);
            Assert.AreEqual(5, items[1][1]);
            Assert.AreEqual(6, items[1][2]);

            Assert.AreEqual(7, items[2][0]);
            Assert.AreEqual(8, items[2][1]);
        }

        [Test]
        public void GetAdjacentPairsTest()
        {
            IEnumerable<int> a = new int[] {};
            Assert.AreEqual(0, a.GetAdjacentPairs().Count());

            IEnumerable<int> b = new int[] { 1 };
            Assert.AreEqual(0, b.GetAdjacentPairs().Count());

            IEnumerable<int> c = new int[] { 1, 2, 3 };
            int[][] cPairs = c.GetAdjacentPairs().ToArray();

            Assert.AreEqual(2, cPairs.Length);
            Assert.AreEqual(1, cPairs[0][0]);
            Assert.AreEqual(2, cPairs[0][1]);
            Assert.AreEqual(2, cPairs[1][0]);
            Assert.AreEqual(3, cPairs[1][1]);
        }

        [Test]
        public void MinByTest()
        {
            Assert.AreEqual(1, (new int[] { 3, 6, 2, 7, 1 }).MinBy(i => i));
        }

        [Test]
        public void MinByComparerTest()
        {
            IComparer<int> comparer = new NegativeComparer();
            Assert.AreEqual(7, (new int[] { 3, 6, 2, 7, 1 }).MinBy(i => i, comparer));
        }

        private class NegativeComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return Comparer<int>.Default.Compare(-x, -y);
            }
        }

        [Test]
        public void ExceptNullsTest()
        {
            IEnumerable<int?> sequence = new int?[] { 1, 2, null, 3, 4, null };
            int[] result = sequence.ExceptNulls().ToArray();

            Assert.AreEqual(4, result.Length);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(3, result[2]);
            Assert.AreEqual(4, result[3]);
        }

        [Test]
        public void SumTest()
        {
            IEnumerable<byte> sequence = new byte[] { 1, 2, 3, 4 };
            Assert.AreEqual(10, sequence.Sum());
        }

        [Test]
        public void ConsolidateTest()
        {
            string[] sequence = EnumerableExtensions.Consolidate(new string[] { "A", "B", "B", "C" }).ToArray();

            Assert.AreEqual(3, sequence.Length, StringUtils.ArrayFormat(sequence));
            Assert.AreEqual("A", sequence[0]);
            Assert.AreEqual("B", sequence[1]);
            Assert.AreEqual("C", sequence[2]);
        }

        [Test]
        public void ConsolidateComparerTest()
        {
            string[] sequence = EnumerableExtensions.Consolidate(new string[] { "A", "B", "B", "C" }, Comparer<string>.Default).ToArray();

            Assert.AreEqual(3, sequence.Length, StringUtils.ArrayFormat(sequence));
            Assert.AreEqual("A", sequence[0]);
            Assert.AreEqual("B", sequence[1]);
            Assert.AreEqual("C", sequence[2]);
        }

        [Test]
        public void AnyAndAllTest()
        {
            IEnumerable<int> sequence = new int[] { 1, 2, 3, 4 };

            Assert.IsTrue(sequence.AnyAndAll(i => i > 0));
            Assert.IsFalse(sequence.AnyAndAll(i => i < 0));
            Assert.IsFalse(Enumerable.Empty<int>().AnyAndAll(i => i > 0));
        }
    }
}
