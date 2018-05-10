using System.Linq;
using ICD.Common.Utils.Collections;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Collections
{
	[TestFixture]
	public sealed class IcdOrderedDictionaryTest
	{
		#region Properties

		[Test]
		public void CountTest()
		{
			IcdOrderedDictionary<int, int> dict = new IcdOrderedDictionary<int, int>
			{
				{0, 0},
				{1, 10},
				{2, 20}
			};

			Assert.AreEqual(3, dict.Count);
		}

		[Test]
		public void IsReadOnlyTest()
		{
			IcdOrderedDictionary<int, int> dict = new IcdOrderedDictionary<int, int>();

			Assert.IsFalse(dict.IsReadOnly);
		}

		[Test]
		public void KeysTest()
		{
			IcdOrderedDictionary<int, int> dict = new IcdOrderedDictionary<int, int>
			{
				{0, 0},
				{1, 10},
				{-1, -10}
			};

			int[] keys = dict.Keys.ToArray();

			Assert.AreEqual(3, keys.Length);
			Assert.AreEqual(-1, keys[0]);
			Assert.AreEqual(0, keys[1]);
			Assert.AreEqual(1, keys[2]);
		}

		[Test]
		public void ValuesTest()
		{
			IcdOrderedDictionary<int, int> dict = new IcdOrderedDictionary<int, int>
			{
				{0, 0},
				{1, 10},
				{-1, -10}
			};

			int[] values = dict.Values.ToArray();

			Assert.AreEqual(3, values.Length);
			Assert.AreEqual(-10, values[0]);
			Assert.AreEqual(0, values[1]);
			Assert.AreEqual(10, values[2]);
		}

		[Test]
		public void IndexerTest()
		{
			IcdOrderedDictionary<int, int> dict = new IcdOrderedDictionary<int, int>
			{
				{0, 0},
				{1, 10},
				{-1, -10}
			};

			Assert.AreEqual(0, dict[0]);
			Assert.AreEqual(10, dict[1]);
			Assert.AreEqual(-10, dict[-1]);
		}

		#endregion

		#region Methods

		[Test]
		public void GetEnumeratorTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void AddTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void ClearTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void ContainsKeyTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void RemoveTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void TryGetValueTest()
		{
			Assert.Inconclusive();
		}

		#endregion
	}
}
