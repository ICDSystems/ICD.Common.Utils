using ICD.Common.Utils.Converters;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Converters
{
	[TestFixture]
	public sealed class AnsiToHtmlTest
	{
		[TestCase("black\x1b[37mwhite",
		          @"black<span style=""color:#BBBBBB"">white</span>")]
		[TestCase("black\x1b[0mblack",
		          @"blackblack")]
		[TestCase("black\x1b[37mwhite\x1b[0m",
		          @"black<span style=""color:#BBBBBB"">white</span>")]
		[TestCase("\x1b[30mblack\x1b[37mwhite",
		          @"<span style=""color:#000000"">black<span style=""color:#BBBBBB"">white</span></span>")]
		public void ConvertTest(string ansi, string expected)
		{
			Assert.AreEqual(expected, AnsiToHtml.Convert(ansi));
		}
	}
}
