using System;
using ICD.Common.Utils.Collections;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Collections
{
	[TestFixture]
	public sealed class WeakKeyDictionaryTest
	{
		private sealed class TestClass
		{
		}

		#region Properties

		[Test]
		public void CountTest()
		{
#if DEBUG
			Assert.Inconclusive();
			return;
#endif
			WeakKeyDictionary<TestClass, int> dict = new WeakKeyDictionary<TestClass, int>();
			Assert.AreEqual(0, dict.Count);

			TestClass instance = new TestClass();

			dict.Add(instance, 0);

			Assert.AreEqual(1, dict.Count, "Expected the added item to increase the dictionary size");

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			Assert.AreEqual(1, dict.Count, "Expected the dictionary to have one uncollected item");

			// Need to actually USE the instance at some point AFTER the collection, otherwise it gets optimized out
			// and is collected prematurely.
			// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
			instance.ToString();

			// Now clear the reference to make sure the instance gets collected.
			// ReSharper disable once RedundantAssignment
			instance = null;

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			Assert.AreEqual(0, dict.Count, "Expected the dictionary to be empty after collecting");
		}

		[Test]
		public void KeysTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void ValuesTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void IndexerTest()
		{
			Assert.Inconclusive();
		}

#endregion

#region Methods

		[Test]
		public void AddTest()
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

		[Test]
		public void ClearTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void RemoveCollectedEntries()
		{
#if DEBUG
			Assert.Inconclusive();
			return;
#endif
			WeakKeyDictionary<TestClass, int> dict = new WeakKeyDictionary<TestClass, int>();

			dict.RemoveCollectedEntries();
			Assert.AreEqual(0, dict.Count, "Expected new dictionary to start empty");

			TestClass instance = new TestClass();

			dict.Add(instance, 0);
			dict.RemoveCollectedEntries();

			Assert.AreEqual(1, dict.Count, "Expected instance to add to the dictionary count");

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			dict.RemoveCollectedEntries();

			Assert.AreEqual(1, dict.Count, "Expected instance to remain uncollected and stay in the dictionary");

			// Need to actually USE the instance at some point AFTER the collection, otherwise it gets optimized out
			// and is collected prematurely.
			// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
			instance.ToString();

			// Now clear the reference to make sure the instance gets collected.
			// ReSharper disable once RedundantAssignment
			instance = null;

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			dict.RemoveCollectedEntries();

			Assert.AreEqual(0, dict.Count, "Expected instance to be collected and removed from the dictionary");
		}

		[Test]
		public void GetEnumeratorTest()
		{
			Assert.Inconclusive();
		}

#endregion
	}
}
