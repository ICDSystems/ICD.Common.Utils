using System.Linq;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests
{
	[TestFixture]
	public sealed class AnsiUtilsTest
	{
		[Test]
		public void ToSpansTest()
		{
			string ansi = "\x1b[30mblack\x1b[37mwhite\x1b[0mdefault";
			AnsiSpan[] spans = AnsiUtils.ToSpans(ansi).ToArray();

			Assert.AreEqual(3, spans.Length);

			Assert.AreEqual("black", spans[0].Text);
			Assert.AreEqual("30", spans[0].Code);

			Assert.AreEqual("white", spans[1].Text);
			Assert.AreEqual("37", spans[1].Code);

			Assert.AreEqual("default", spans[2].Text);
			Assert.AreEqual("0", spans[2].Code);
		}
	}
}
