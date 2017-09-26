using ICD.Common.Utils.Extensions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ICD.Common.Utils.Tests.Extensions
{
    [TestFixture]
    public sealed class DictionaryExtensionsTest
    {
        [Test]
        public void RemoveValueTest()
        {
			Dictionary<int, int> dict = new Dictionary<int, int>
			{
				{ 1, 10 },
				{ 2, 20 },
				{ 3, 30 },
				{ 4, 40 },
			};

			Assert.IsTrue(dict.RemoveValue(10));
			Assert.IsFalse(dict.RemoveValue(0));

			Assert.IsFalse(dict.ContainsKey(1));
			Assert.AreEqual(3, dict.Count);
		}

        [Test]
        public void RemoveAllValuesTest()
        {
			Dictionary<int, int> dict = new Dictionary<int, int>
			{
				{ 1, 10 },
				{ 2, 10 },
				{ 3, 30 },
				{ 4, 40 },
			};

			dict.RemoveAllValues(10);

			Assert.IsFalse(dict.ContainsKey(1));
			Assert.IsFalse(dict.ContainsKey(2));
			Assert.AreEqual(2, dict.Count);
		}

        [Test]
        public void GetDefaultTest()
        {
			Dictionary<int, int> dict = new Dictionary<int, int>
			{
				{ 1, 10 },
				{ 2, 10 },
				{ 3, 30 },
				{ 4, 40 },
			};

			Assert.AreEqual(10, dict.GetDefault(1));
			Assert.AreEqual(0, dict.GetDefault(0));
		}

        [Test]
        public void GetDefaultValueTest()
        {
			Dictionary<int, int> dict = new Dictionary<int, int>
			{
				{ 1, 10 },
				{ 2, 10 },
				{ 3, 30 },
				{ 4, 40 },
			};

			Assert.AreEqual(10, dict.GetDefault(1, 1000));
			Assert.AreEqual(1000, dict.GetDefault(0, 1000));
		}

        [Test]
        public void GetOrAddDefaultTest()
        {
			Dictionary<int, int> dict = new Dictionary<int, int>
			{
				{ 1, 10 },
				{ 2, 10 },
				{ 3, 30 },
				{ 4, 40 },
			};

			Assert.AreEqual(10, dict.GetOrAddDefault(1, 1000));
			Assert.AreEqual(1000, dict.GetOrAddDefault(0, 1000));
			Assert.AreEqual(1000, dict[0]);
			Assert.AreEqual(5, dict.Count);
		}

        [Test]
        public void GetKeyTest()
        {
            Dictionary<int, int> dict = new Dictionary<int, int>
            {
                { 1, 10 },
                { 2, 20 },
                { 3, 30 }
            };

            Assert.AreEqual(1, dict.GetKey(10));
            Assert.AreEqual(2, dict.GetKey(20));
            Assert.AreEqual(3, dict.GetKey(30));
            Assert.Throws<KeyNotFoundException>(() => dict.GetKey(40));
        }

        [Test]
        public void TryGetKeyTest()
        {
            Dictionary<int, int> dict = new Dictionary<int, int>
            {
                { 1, 10 },
                { 2, 20 },
                { 3, 30 }
            };

            int key;

            Assert.IsTrue(dict.TryGetKey(10, out key));
            Assert.AreEqual(1, key);

            Assert.IsTrue(dict.TryGetKey(20, out key));
            Assert.AreEqual(2, key);

            Assert.IsTrue(dict.TryGetKey(30, out key));
            Assert.AreEqual(3, key);
        }

        [Test]
        public void GetKeysTest()
        {
            Dictionary<int, int> dict = new Dictionary<int, int>
            {
                { 1, 10 },
                { 2, 10 },
                { 3, 30 }
            };

            int[] aKeys = dict.GetKeys(10).ToArray();
            int[] bKeys = dict.GetKeys(20).ToArray();
            int[] cKeys = dict.GetKeys(30).ToArray();

            Assert.AreEqual(2, aKeys.Length);
            Assert.IsTrue(aKeys.Contains(1));
            Assert.IsTrue(aKeys.Contains(2));

            Assert.AreEqual(0, bKeys.Length);

            Assert.AreEqual(1, cKeys.Length);
            Assert.IsTrue(cKeys.Contains(3));
        }

        [Test]
        public void UpdateTest()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void AddRangeValueKeyFuncTest()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void AddRangeKeyValueFuncTest()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void AddRangeTest()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void DictionaryEqualTest()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void DictionaryEqualComparerTest()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void DictionaryEqualFuncComparerTest()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void OrderByKey()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void OrderValuesByKeyTest()
        {
            Assert.Inconclusive();
        }
    }
}
