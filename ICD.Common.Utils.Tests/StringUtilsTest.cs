﻿using System.Linq;
using ICD.Common.Properties;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests
{
	[TestFixture]
	public sealed class StringUtilsTest
	{
		[Test, UsedImplicitly]
		public void ToHexLiteralTest()
		{
			const string hex = "\x08\x22\x00\x00\x00\x02";
			string output = StringUtils.ToHexLiteral(hex);

			Assert.AreEqual(@"\x08\x22\x00\x00\x00\x02", output);
		}

		[Test, UsedImplicitly]
		public void FromHexLiteralTest()
		{
			const string literal = @"\x08\x22\x00\x00\x00\x02";
			string output = StringUtils.FromHexLiteral(literal);

			Assert.AreEqual("\x08\x22\x00\x00\x00\x02", output);
		}

		[TestCase("FF", new byte[] {0xFF})]
		[TestCase("01FF", new byte[] { 0x01, 0xFF })]
		public void HexToBytes(string value, byte[] expected)
		{
			byte[] bytes = StringUtils.HexToBytes(value);
			Assert.IsTrue(bytes.SequenceEqual(expected));
		}

		[TestCase("1", 1)]
		[TestCase("FF", 0xFF)]
		public void HexToByte(string value, byte expected)
		{
			Assert.AreEqual(expected, StringUtils.HexToByte(value));
		}

		[TestCase("Test", "Test")]
		[TestCase("test", "Test")]
		[TestCase("TodayILiveInTheUSAWithSimon", "Today I Live In The USA With Simon")]
		[TestCase("CONST_VALUE", "CONST VALUE")]
		[TestCase("m_PrivateMember", "Private Member")]
		[TestCase("variableName", "Variable Name")]
		[TestCase("Comma, Delimited", "Comma, Delimited")]
		[TestCase("Comma,Delimited", "Comma, Delimited")]
		public void NiceNameTest(string input, string expected)
		{
			string output = StringUtils.NiceName(input);
			Assert.AreEqual(expected, output);
		}

		[Test, UsedImplicitly]
		public void SafeNumericFormatTest()
		{
			Assert.AreEqual(string.Empty, StringUtils.SafeNumericFormat("# # ###.###.####", string.Empty));
			Assert.AreEqual("9 1 252.943.4324", StringUtils.SafeNumericFormat("# # ###.###.####", "0000912529434324"));
			Assert.AreEqual("9 1 252.943.4324", StringUtils.SafeNumericFormat("# # ###.###.####", "912529434324"));
			Assert.AreEqual("1 252.943.4324", StringUtils.SafeNumericFormat("# # ###.###.####", "12529434324"));
			Assert.AreEqual("252.943.4324", StringUtils.SafeNumericFormat("# # ###.###.####", "2529434324"));
			Assert.AreEqual("052.943.4324", StringUtils.SafeNumericFormat("# # ###.###.####", "0529434324"));
		}

		[Test, UsedImplicitly]
		public void ReverseTest()
		{
			Assert.AreEqual("rabooF", StringUtils.Reverse("Foobar"));
		}

		[Test, UsedImplicitly]
		public void ArrayFormatTest()
		{
			int[] items = {1, 4, 3, 2, 5};
			Assert.AreEqual("[1, 4, 3, 2, 5]", StringUtils.ArrayFormat(items));
		}

		[Test, UsedImplicitly]
		public void RangeFormatTest()
		{
			Assert.AreEqual("[-3 - 5]", StringUtils.RangeFormat(-3, 5));
		}

		[TestCase("foobar", "Foobar")]
		public void UppercaseFirstTest(string value, string expected)
		{
			Assert.AreEqual(expected, StringUtils.UppercaseFirst(value));
		}

		[TestCase("test", "Test")]
		[TestCase("test test", "Test Test")]
		public static void ToTitleCase(string input, string expected)
		{
			Assert.AreEqual(expected, StringUtils.ToTitleCase(input));
		}

		[TestCase((byte)0x67, "0x67")]
		public void ToIpIdStringTest(byte ipid, string expected)
		{
			Assert.AreEqual(expected, StringUtils.ToIpIdString(ipid));
		}

		[TestCase("0x67", (byte)0x67)]
		public void FromIpIdStringTest(string value, byte expected)
		{
			Assert.AreEqual(expected, StringUtils.FromIpIdString(value));
		}

		[TestCase("0x67", true, (byte)0x67)]
		public void TryFromIpIdStringTest(string value, bool expected, byte expectedOutput)
		{
			byte output;
			Assert.AreEqual(expected, StringUtils.TryFromIpIdString(value, out output));
			Assert.AreEqual(expectedOutput, output);
		}

		[UsedImplicitly]
		[TestCase(null, true)]
		[TestCase("", true)]
		[TestCase("\n", true)]
		[TestCase("\r", true)]
		[TestCase("Test", false)]
		[TestCase("Test\n", false)]
		[TestCase("\nTest", false)]
		public void IsNullOrWhitespaceTest(string value, bool expectedResult)
		{
			Assert.AreEqual(StringUtils.IsNullOrWhitespace(value), expectedResult);
		}

		[Test, UsedImplicitly]
		public void TryParseIntTest()
		{
			int testVal;
			Assert.IsFalse(StringUtils.TryParse("fish", out testVal));
			Assert.IsTrue(StringUtils.TryParse("1", out testVal));
			Assert.AreEqual(1, testVal);
		}

		[Test, UsedImplicitly]
		public void TryParseUintTest()
		{
			uint testVal;
			Assert.IsFalse(StringUtils.TryParse("fish", out testVal));
			Assert.IsTrue(StringUtils.TryParse("1", out testVal));
			Assert.AreEqual((uint)1, testVal);
		}

		[Test, UsedImplicitly]
		public void TryParseShortTest()
		{
			short testVal;
			Assert.IsFalse(StringUtils.TryParse("fish", out testVal));
			Assert.IsTrue(StringUtils.TryParse("1", out testVal));
			Assert.AreEqual((short)1, testVal);
		}

		[Test, UsedImplicitly]
		public void TryParseUshortTest()
		{
			ushort testVal;
			Assert.IsFalse(StringUtils.TryParse("fish", out testVal));
			Assert.IsTrue(StringUtils.TryParse("1", out testVal));
			Assert.AreEqual((ushort)1, testVal);
		}

		[Test, UsedImplicitly]
		public void TryParseLongTest()
		{
			long testVal;
			Assert.IsFalse(StringUtils.TryParse("fish", out testVal));
			Assert.IsTrue(StringUtils.TryParse("1", out testVal));
			Assert.AreEqual((long)1, testVal);
		}

		[Test, UsedImplicitly]
		public void TryParseUlongTest()
		{
			ulong testVal;
			Assert.IsFalse(StringUtils.TryParse("fish", out testVal));
			Assert.IsTrue(StringUtils.TryParse("1", out testVal));
			Assert.AreEqual((ulong)1, testVal);
		}

		[Test, UsedImplicitly]
		public void  TryParseFloatTest()
		{
			float testVal;
			Assert.IsFalse(StringUtils.TryParse("fish", out testVal));
			Assert.IsTrue(StringUtils.TryParse("1.1", out testVal));
			Assert.AreEqual((float)1.1, testVal);
		}

		[Test, UsedImplicitly]
		public void TryParseBoolTest()
		{
			bool testVal;
			Assert.IsFalse(StringUtils.TryParse("fish", out testVal));
			Assert.IsTrue(StringUtils.TryParse("true", out testVal));
			Assert.AreEqual(true, testVal);
		}

		[TestCase("test", "\"test\"")]
		[TestCase("\"test\"", "\"test\"")]
		[TestCase("test test", "\"test test\"")]
		public void EnquoteTest(string input, string expected)
		{
			Assert.AreEqual(expected, StringUtils.Enquote(input));
		}

		[TestCase("\"test\"", "test")]
		[TestCase("\"test test\"", "test test")]
		public void UnEnquoteTest(string input, string expected)
		{
			Assert.AreEqual(expected, StringUtils.UnEnquote(input));
		}
	}
}
