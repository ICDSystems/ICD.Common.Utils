using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Extensions;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Extensions
{
	[TestFixture]
	public sealed class StringExtensionsTest
	{
		[Test]
		public void IndexOfTest()
		{
			string first;
			Assert.AreEqual(5, "test1test3test2".IndexOf(new[] {"test2", "test3"}, out first));
			Assert.AreEqual("test3", first);
		}

		[TestCase(true, "12345", '1')]
		[TestCase(false, "12345", '2')]
		public void StartsWithTest(bool expected, string value, char character)
		{
			Assert.AreEqual(expected, value.StartsWith(character));
		}

		[TestCase(true, "12345", '5')]
		[TestCase(false, "12345", '2')]
		public void EndsWithTest(bool expected, string value, char character)
		{
			Assert.AreEqual(expected, value.EndsWith(character));
		}

		[Test]
		public void SplitCharCountTest()
		{
			string[] split = "123-456-789-0".Split('-', 2).ToArray();

			Assert.AreEqual(2, split.Length);
			Assert.AreEqual("123", split[0]);
			Assert.AreEqual("456-789-0", split[1]);
		}

		[Test]
		public void SplitChunksizeTest()
		{
			string[] split = "1234567890".Split(3).ToArray();

			Assert.AreEqual(4, split.Length);
			Assert.AreEqual("123", split[0]);
			Assert.AreEqual("456", split[1]);
			Assert.AreEqual("789", split[2]);
			Assert.AreEqual("0", split[3]);
		}

		[TestCase("12345", "   12 3 4  \t 5\n")]
		public void RemoveWhitespaceTest(string expected, string value)
		{
			Assert.AreEqual(expected, value.RemoveWhitespace());
		}

		[TestCase("1234567890", "12345", "67890")]
		[TestCase("foobarfoobar", "bar", "foofoo")]
		public void RemoveStringTest(string value, string other, string expected)
		{
			Assert.AreEqual(expected, value.Remove(other));
		}

		[TestCase("1234567890", new[] {'2', '6'}, "13457890")]
		[TestCase("912529434324", new[] {'-', '(', ')', '.', '+'}, "912529434324")]
		public void RemoveCharactersTest(string value, IEnumerable<char> characters, string expected)
		{
			Assert.AreEqual(expected, value.Remove(characters));
		}

		[TestCase(true, "27652")]
		[TestCase(false, "a27652")]
		public void IsNumericTest(bool expected, string value)
		{
			Assert.AreEqual(expected, value.IsNumeric());
		}
	}
}
