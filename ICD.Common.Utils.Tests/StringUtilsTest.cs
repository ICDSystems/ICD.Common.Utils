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

		[Test, UsedImplicitly]
		public void NiceNameTest()
		{
			string output = StringUtils.NiceName("TodayILiveInTheUSAWithSimon");
			Assert.AreEqual("Today I Live In The USA With Simon", output);
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

		[Test, UsedImplicitly]
		public void UppercaseFirstTest()
		{
			Assert.AreEqual("Foobar", StringUtils.UppercaseFirst("foobar"));
		}

		[Test, UsedImplicitly]
		public void ToIpIdStringTest()
		{
			Assert.AreEqual("0x67", StringUtils.ToIpIdString(0x67));
		}

		[Test, UsedImplicitly]
		public void FromIpIdStringTest()
		{
			Assert.AreEqual(0x67, StringUtils.FromIpIdString("0x67"));
		}
	}
}
