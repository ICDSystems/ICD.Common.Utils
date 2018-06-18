using System;
using ICD.Common.Utils.Collections;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Collections
{
	[TestFixture]
	public sealed class BiDictionaryTest
	{
		#region Properties

		[Test]
		public void CountTest()
		{
			BiDictionary<int, string> dict = new BiDictionary<int, string>();

			dict.Set(1, "10");
			dict.Set(2, "10");
			dict.Set(3, "30");

			Assert.AreEqual(2, dict.Count);
		}

		[Test]
		public void KeysTest()
		{
			BiDictionary<int, string> dict = new BiDictionary<int, string>();

			dict.Set(1, "10");
			dict.Set(2, "10");
			dict.Set(3, "30");

			Assert.AreEqual(2, dict.Keys.Count);

			Assert.IsTrue(dict.Keys.Contains(2));
			Assert.IsTrue(dict.Keys.Contains(3));
		}

		[Test]
		public void ValuesTest()
		{
			BiDictionary<int, string> dict = new BiDictionary<int, string>();

			dict.Set(1, "10");
			dict.Set(1, "20");
			dict.Set(3, "30");

			Assert.AreEqual(2, dict.Values.Count);

			Assert.IsTrue(dict.Values.Contains("20"));
			Assert.IsTrue(dict.Values.Contains("30"));
		}

		#endregion

		#region Methods

		[Test]
		public void ClearTest()
		{
			BiDictionary<int, string> dict = new BiDictionary<int, string>();

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
			BiDictionary<int, string> dict = new BiDictionary<int, string>();

			Assert.IsFalse(dict.ContainsKey(1));

			dict.Set(1, "10");

			Assert.IsTrue(dict.ContainsKey(1));
		}

		[Test]
		public void ContainsValueTest()
		{
			BiDictionary<int, string> dict = new BiDictionary<int, string>();

			Assert.IsFalse(dict.ContainsValue("10"));

			dict.Set(1, "10");

			Assert.IsTrue(dict.ContainsValue("10"));
		}

		[Test]
		public void AddTest()
		{
			BiDictionary<int, string> dict = new BiDictionary<int, string>();

			dict.Add(1, "10");

			Assert.AreEqual(1, dict.Count);

			Assert.Throws<ArgumentException>(() => dict.Add(1, "10"));
			Assert.Throws<ArgumentException>(() => dict.Add(1, "20"));
			Assert.Throws<ArgumentException>(() => dict.Add(2, "10"));

			Assert.DoesNotThrow(() => dict.Add(2, "20"));

			Assert.AreEqual(2, dict.Count);
		}

		[Test]
		public void SetTest()
		{
			BiDictionary<int, string> dict = new BiDictionary<int, string>();

			dict.Set(1, "10");
			dict.Set(1, "20");
			dict.Set(3, "30");

			Assert.AreEqual("20", dict.GetValue(1));
			Assert.AreEqual("30", dict.GetValue(3));

			Assert.AreEqual(1, dict.GetKey("20"));
			Assert.AreEqual(3, dict.GetKey("30"));
		}

		[Test]
		public void GetKeyTest()
		{
			BiDictionary<int, string> dict = new BiDictionary<int, string>();

			dict.Set(1, "10");
			dict.Set(1, "20");
			dict.Set(3, "30");

			Assert.AreEqual(1, dict.GetKey("20"));
			Assert.AreEqual(3, dict.GetKey("30"));
		}

		[Test]
		public void GetValueTest()
		{
			BiDictionary<int, string> dict = new BiDictionary<int, string>();

			dict.Set(1, "10");
			dict.Set(1, "20");
			dict.Set(3, "30");

			Assert.AreEqual("20", dict.GetValue(1));
			Assert.AreEqual("30", dict.GetValue(3));
		}

		[Test]
		public void RemoveKeyTest()
		{
			BiDictionary<int, string> dict = new BiDictionary<int, string>();

			dict.Add(1, "10");

			Assert.AreEqual(1, dict.Count);

			dict.RemoveKey(1);

			Assert.AreEqual(0, dict.Count);
		}

		[Test]
		public void RemoveValueTest()
		{
			BiDictionary<int, string> dict = new BiDictionary<int, string>();

			dict.Add(1, "10");

			Assert.AreEqual(1, dict.Count);

			dict.RemoveValue("10");

			Assert.AreEqual(0, dict.Count);
		}

		[Test]
		public void TryGetValueTest()
		{
			BiDictionary<int, string> dict = new BiDictionary<int, string>();

			string value;
			Assert.IsFalse(dict.TryGetValue(1, out value));
			// ReSharper disable once ExpressionIsAlwaysNull
			Assert.AreEqual(null, value);

			dict.Add(1, "10");

			Assert.IsTrue(dict.TryGetValue(1, out value));
			Assert.AreEqual("10", value);
		}

		[Test]
		public void TryGetKeyTest()
		{
			BiDictionary<int, string> dict = new BiDictionary<int, string>();

			int value;
			Assert.IsFalse(dict.TryGetKey("10", out value));
			Assert.AreEqual(0, value);

			dict.Add(1, "10");

			Assert.IsTrue(dict.TryGetKey("10", out value));
			Assert.AreEqual(1, value);
		}

		#endregion
	}
}
