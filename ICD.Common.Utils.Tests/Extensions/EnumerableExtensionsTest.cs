﻿using ICD.Common.Utils.Extensions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ICD.Common.Utils.Tests_NetStandard.Extensions
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
            Assert.Inconclusive();
        }

        [Test]
        public void ExecuteTest()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void ForEachTest()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void ForEachIndexTest()
        {
            Assert.Inconclusive();
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
            Assert.Inconclusive();
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
            Assert.Inconclusive();
        }

        [Test]
        public void OrderTest()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void ExceptTest()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void ToHashSetTest()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void ToDictionaryIntTest()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void ToDictionaryUIntTest()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void ToDictionaryTest()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void UnanimousTest()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void UnanimousOtherTest()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void PartitionTest()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void MinByTest()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void MinByComparerTest()
        {
            Assert.Inconclusive();
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