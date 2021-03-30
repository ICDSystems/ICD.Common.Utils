using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Collections;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Collections
{
	[TestFixture]
	public sealed class IcdOrderedDictionaryTest
	{
		[Test]
		public void OrderingTest()
		{
			IcdOrderedDictionary<string, string> dict = new IcdOrderedDictionary<string, string>
			{
				{"1", "a"},
				{"2", "b"},
				{"3", "c"}
			};

			Assert.IsTrue(dict.Keys.SequenceEqual(new[] {"1", "2", "3"}));
			Assert.IsTrue(dict.Values.SequenceEqual(new[] { "a", "b", "c" }));

			// Remove the key and add to the end of the dictionary
			dict.Remove("2");
			dict["2"] = "d";

			Assert.IsTrue(dict.Keys.SequenceEqual(new[] { "1", "3", "2" }));
			Assert.IsTrue(dict.Values.SequenceEqual(new[] { "a", "c", "d" }));

			// No key change
			dict["1"] = "e";

			Assert.IsTrue(dict.Keys.SequenceEqual(new[] { "1", "3", "2" }));
			Assert.IsTrue(dict.Values.SequenceEqual(new[] { "e", "c", "d" }));
		}

		[Test]
		public void GetTest()
		{
			IcdOrderedDictionary<string, string> dict = new IcdOrderedDictionary<string, string>
			{
				{"1", "a"},
				{"2", "b"},
				{"3", "c"}
			};

			KeyValuePair<string, string> kvp = dict.Get(0);
			Assert.AreEqual("1", kvp.Key);
			Assert.AreEqual("a", kvp.Value);

			kvp = dict.Get(1);
			Assert.AreEqual("2", kvp.Key);
			Assert.AreEqual("b", kvp.Value);

			kvp = dict.Get(2);
			Assert.AreEqual("3", kvp.Key);
			Assert.AreEqual("c", kvp.Value);

			// Remove the key and add to the end of the dictionary
			dict.Remove("2");
			dict["2"] = "d";

			kvp = dict.Get(0);
			Assert.AreEqual("1", kvp.Key);
			Assert.AreEqual("a", kvp.Value);

			kvp = dict.Get(1);
			Assert.AreEqual("3", kvp.Key);
			Assert.AreEqual("c", kvp.Value);

			kvp = dict.Get(2);
			Assert.AreEqual("2", kvp.Key);
			Assert.AreEqual("d", kvp.Value);

			// No key change
			dict["1"] = "e";

			kvp = dict.Get(0);
			Assert.AreEqual("1", kvp.Key);
			Assert.AreEqual("e", kvp.Value);

			kvp = dict.Get(1);
			Assert.AreEqual("3", kvp.Key);
			Assert.AreEqual("c", kvp.Value);

			kvp = dict.Get(2);
			Assert.AreEqual("2", kvp.Key);
			Assert.AreEqual("d", kvp.Value);
		}
	}
}
