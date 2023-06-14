using System;
using System.Collections.Generic;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Collections
{
    [TestFixture]
    public class ReverseLookupDictionaryTest
    {
	    
	    //todo: Finish!
	    #region Properties

		[Test]
		public void CountTest()
		{
			ReverseLookupDictionary<int, string> dict = new ReverseLookupDictionary<int, string>();

			dict.Set(1, "10");
			dict.Set(2, "10");
			dict.Set(3, "30");

			Assert.AreEqual(2, dict.Count);
		}

		[Test]
		public void KeysTest()
		{
			ReverseLookupDictionary<int, string> dict = new ReverseLookupDictionary<int, string>();

			dict.Set(1, "10");
			dict.Set(2, "10");
			dict.Set(3, "30");

			Assert.AreEqual(2, dict.Keys.Count);

			Assert.IsTrue(dict.Keys.Contains(2));
			Assert.IsTrue(dict.Keys.Contains(3));
			Assert.IsFalse(dict.Keys.Contains(4));
		}

		[Test]
		public void ValuesTest()
		{
			ReverseLookupDictionary<int, string> dict = new ReverseLookupDictionary<int, string>();

			dict.Set(1, "10");
			dict.Set(1, "20");
			dict.Set(3, "30");

			Assert.AreEqual(2, dict.Values.Count);

			Assert.IsTrue(dict.Values.Contains("20"));
			Assert.IsTrue(dict.Values.Contains("30"));
			Assert.IsFalse(dict.Values.Contains("40"));
		}

		#endregion

		#region Methods

		[Test]
		public void ClearTest()
		{
			ReverseLookupDictionary<int, string> dict = new ReverseLookupDictionary<int, string>();

			dict.Set(1, "10");
			dict.Set(1, "20");
			dict.Set(3, "30");

			Assert.AreEqual(2, dict.Count);

			dict.Clear();

			Assert.AreEqual(0, dict.Count);
		}

		[Test]
		public void ContainsKeyTest()
		{
			ReverseLookupDictionary<int, string> dict = new ReverseLookupDictionary<int, string>();

			Assert.IsFalse(dict.ContainsKey(1));

			dict.Set(1, "10");

			Assert.IsTrue(dict.ContainsKey(1));
		}

		[Test]
		public void ContainsValueTest()
		{
			ReverseLookupDictionary<int, string> dict = new ReverseLookupDictionary<int, string>();

			Assert.IsFalse(dict.ContainsValue("10"));

			dict.Set(1, "10");

			Assert.IsTrue(dict.ContainsValue("10"));
		}

		[Test]
		public void AddTest()
		{
			ReverseLookupDictionary<int, string> dict = new ReverseLookupDictionary<int, string>();

			dict.Add(1, "10");

			Assert.AreEqual(1, dict.Count);

			Assert.Throws<ArgumentException>(() => dict.Add(1, "10"));
			Assert.Throws<ArgumentException>(() => dict.Add(1, "20"));
			
			Assert.DoesNotThrow(() => dict.Add(2, "10"));
			Assert.DoesNotThrow(() => dict.Add(3, "20"));

			Assert.AreEqual(3, dict.Count);
		}

		[Test]
		public void SetTest()
		{
			ReverseLookupDictionary<int, string> dict = new ReverseLookupDictionary<int, string>();

			dict.Set(1, "10");
			dict.Set(1, "20");
			dict.Set(3, "30");
			dict.Set(4, "20");

			Assert.AreEqual("20", dict.GetValue(1));
			Assert.AreEqual("30", dict.GetValue(3));
			Assert.AreEqual("40", dict.GetValue(4));

			CollectionAssert.AreEquivalent(new[]{1,4}, dict.GetKeys("20"));
			CollectionAssert.AreEquivalent(new[]{3}, dict.GetKeys("30"));
		}

		[Test]
		public void GetKeysTest()
		{
			ReverseLookupDictionary<int, string> dict = new ReverseLookupDictionary<int, string>();

			dict.Set(1, "10");
			dict.Set(1, "Odd");
			dict.Add(2, "Even");
			dict.Set(3, "Odd");
			dict.Add(4, "Value");
			dict.Set(4, "Even");
			dict.Add(5, "Odd");
			dict.Add(6, "Even");

			CollectionAssert.AreEquivalent(new[]{1,3,5}, dict.GetKeys("Odd"));
			CollectionAssert.AreEquivalent(new[]{2,4,6}, dict.GetKeys("Even"));
		}

		[Test]
		public void GetValueTest()
		{
			ReverseLookupDictionary<int, string> dict = new ReverseLookupDictionary<int, string>();

			dict.Set(1, "10");
			dict.Set(1, "20");
			dict.Set(3, "30");
			dict.Set(2, "20");

			Assert.AreEqual("20", dict.GetValue(1));
			Assert.AreEqual("30", dict.GetValue(3));
			Assert.AreEqual("20", dict.GetValue(2));
		}

		[Test]
		public void RemoveKeyTest()
		{
			ReverseLookupDictionary<int, string> dict = new ReverseLookupDictionary<int, string>();

			dict.Add(1, "10");

			Assert.AreEqual(1, dict.Count);

			dict.RemoveKey(1);

			Assert.AreEqual(0, dict.Count);
		}

		[Test]
		public void RemoveValueSingleTest()
		{
			ReverseLookupDictionary<int, string> dict = new ReverseLookupDictionary<int, string>();

			dict.Add(1, "10");

			Assert.AreEqual(1, dict.Count);

			dict.RemoveValue("10");

			Assert.AreEqual(0, dict.Count);
		}
		
		[Test]
		public void RemoveValueMultipleTest1()
		{
			ReverseLookupDictionary<int, string> dict = new ReverseLookupDictionary<int, string>();

			dict.Add(1, "10");
			dict.Add(2, "10");
			dict.Add(3, "10");

			Assert.AreEqual(3, dict.Count);

			dict.RemoveValue("10");

			Assert.AreEqual(0, dict.Count);
		}
		
		[Test]
		public void RemoveValueWithOthersTest()
		{
			ReverseLookupDictionary<int, string> dict = new ReverseLookupDictionary<int, string>();

			dict.Add(1, "10");
			dict.Add(2, "10");
			dict.Add(3, "10");
			dict.Add(4, "20");
			dict.Add(5, "20");
			dict.Add(6, "some other string");

			Assert.AreEqual(6, dict.Count);

			dict.RemoveValue("10");

			Assert.AreEqual(3, dict.Count);
		}

		[Test]
		public void TryGetValueTest()
		{
			ReverseLookupDictionary<int, string> dict = new ReverseLookupDictionary<int, string>();

			string value;
			Assert.IsFalse(dict.TryGetValue(1, out value));
			// ReSharper disable once ExpressionIsAlwaysNull
			Assert.AreEqual(null, value);

			dict.Add(1, "10");

			Assert.IsTrue(dict.TryGetValue(1, out value));
			Assert.AreEqual("10", value);
		}

		[Test]
		public void TryGetKeysMultipleTest()
		{
			ReverseLookupDictionary<int, string> dict = new ReverseLookupDictionary<int, string>();

			IEnumerable<int> value;
			Assert.IsFalse(dict.TryGetKeys("10", out value));

			dict.Add(1, "10");
			dict.Add(11, "100");

			Assert.IsTrue(dict.TryGetKeys("10", out value));
			CollectionAssert.AreEquivalent(new[]{1}, value);
			Assert.IsTrue(dict.TryGetKeys("100", out value));
			CollectionAssert.AreEquivalent(new[]{11}, value);
			

			dict.Add(2, "10");
			dict.Add(3, "10");
			dict.Add(12, "100");
			dict.Add(13, "100");
			
			Assert.IsTrue(dict.TryGetKeys("10", out value));
			CollectionAssert.AreEquivalent(new[]{1,2,3}, value);
			
			Assert.IsTrue(dict.TryGetKeys("100", out value));
			CollectionAssert.AreEquivalent(new[]{11,12,13}, value);
			
			Assert.IsFalse(dict.TryGetKeys("string", out value));
			
		}
		
		[Test]
		public void TryGetKeysSingleTest()
		{
			ReverseLookupDictionary<int, string> dict = new ReverseLookupDictionary<int, string>();

			IEnumerable<int> value;
			Assert.IsFalse(dict.TryGetKeys("10", out value));

			dict.Add(1, "10");

			Assert.IsTrue(dict.TryGetKeys("10", out value));
			CollectionAssert.AreEquivalent(new[]{1}, value);
		}

		#endregion
    }
}