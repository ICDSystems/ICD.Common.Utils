using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Extensions;
using NUnit.Framework;

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
				{1, 10},
				{2, 20},
				{3, 30},
				{4, 40}
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
				{1, 10},
				{2, 10},
				{3, 30},
				{4, 40}
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
				{1, 10},
				{2, 10},
				{3, 30},
				{4, 40}
			};

			Assert.AreEqual(10, dict.GetDefault(1));
			Assert.AreEqual(0, dict.GetDefault(0));
		}

		[Test]
		public void GetDefaultValueTest()
		{
			Dictionary<int, int> dict = new Dictionary<int, int>
			{
				{1, 10},
				{2, 10},
				{3, 30},
				{4, 40}
			};

			Assert.AreEqual(10, dict.GetDefault(1, 1000));
			Assert.AreEqual(1000, dict.GetDefault(0, 1000));
		}

		[Test]
		public void GetOrAddDefaultTest()
		{
			Dictionary<int, int> dict = new Dictionary<int, int>
			{
				{1, 10},
				{2, 10},
				{3, 30},
				{4, 40}
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
				{1, 10},
				{2, 20},
				{3, 30}
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
				{1, 10},
				{2, 20},
				{3, 30}
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
				{1, 10},
				{2, 10},
				{3, 30}
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
			Dictionary<int, int> a = new Dictionary<int, int>
			{
				{1, 10},
				{2, 10}
			};

			Dictionary<int, int> b = new Dictionary<int, int>
			{
				{2, 20},
				{3, 30}
			};

			Assert.IsTrue(a.Update(b));
			Assert.IsFalse(a.Update(b));

			Assert.AreEqual(3, a.Count);
			Assert.AreEqual(10, a[1]);
			Assert.AreEqual(20, a[2]);
			Assert.AreEqual(30, a[3]);

			Assert.AreEqual(2, b.Count);
			Assert.AreEqual(20, b[2]);
			Assert.AreEqual(30, b[3]);

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
			Dictionary<int, int> a = new Dictionary<int, int>
			{
				{1, 10},
				{2, 20},
				{3, 30}
			};

			Dictionary<int, int> b = new Dictionary<int, int>
			{
				{1, 10},
				{2, 20},
				{3, 30}
			};

			Dictionary<int, int> c = new Dictionary<int, int>
			{
				{1, 10},
				{2, 20}
			};

			Assert.IsTrue(a.DictionaryEqual(b));
			Assert.IsTrue(b.DictionaryEqual(a));

			Assert.IsFalse(a.DictionaryEqual(c));
			Assert.IsFalse(c.DictionaryEqual(a));
		}

		[Test]
		public void DictionaryEqualComparerTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void DictionaryEqualFuncComparerTest()
		{
			Dictionary<int, int> a = new Dictionary<int, int>
			{
				{1, 10},
				{2, 20},
				{3, 30}
			};

			Dictionary<int, int> b = new Dictionary<int, int>
			{
				{1, 10},
				{2, 20},
				{3, 30}
			};

			Dictionary<int, int> c = new Dictionary<int, int>
			{
				{1, 10},
				{2, 20}
			};

			Assert.IsTrue(a.DictionaryEqual(b, (left, right) => left / 10 == right / 10));
			Assert.IsTrue(b.DictionaryEqual(a, (left, right) => left / 10 == right / 10));

			Assert.IsFalse(a.DictionaryEqual(c, (left, right) => left / 10 == right / 10));
			Assert.IsFalse(c.DictionaryEqual(a, (left, right) => left / 10 == right / 10));
		}

		[Test]
		public void OrderByKeyTest()
		{
			Dictionary<int, int> a = new Dictionary<int, int>
			{
				{1, 10},
				{2, 20},
				{3, 30}
			};

			KeyValuePair<int, int>[] ordered = a.OrderByKey().ToArray();

			Assert.AreEqual(3, ordered.Length);
			Assert.AreEqual(10, ordered[0].Value);
			Assert.AreEqual(20, ordered[1].Value);
			Assert.AreEqual(30, ordered[2].Value);
		}

		[Test]
		public void OrderValuesByKeyTest()
		{
			Dictionary<int, int> a = new Dictionary<int, int>
			{
				{1, 10},
				{2, 20},
				{3, 30}
			};

			int[] ordered = a.OrderValuesByKey().ToArray();

			Assert.AreEqual(3, ordered.Length);
			Assert.AreEqual(10, ordered[0]);
			Assert.AreEqual(20, ordered[1]);
			Assert.AreEqual(30, ordered[2]);
		}

		[Test]
		public void ToInverseTest()
		{
			Dictionary<int, string> forward = new Dictionary<int, string>
			{
				{ 1, "testA" },
				{ 2, "testA" },
				{ 3, "testB" },
				{ 4, "testB" }
			};

			Dictionary<string, List<int>> backwards = new Dictionary<string, List<int>>
			{
				{"testA", new List<int> {1, 2}},
				{"testB", new List<int> {3, 4}}
			};

			Dictionary<string, List<int>> inverse = forward.ToInverse();

			bool equal = inverse.DictionaryEqual(backwards, (v1, v2) => v1.ScrambledEquals(v2));

			Assert.IsTrue(equal);
		}
	}
}
