using System;
using System.Text;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests
{
	[TestFixture]
	public sealed class EncodingUtilsTest
	{
		[Test]
		public void StripUtf8BomTest()
		{
			Assert.Throws<ArgumentNullException>(() => EncodingUtils.StripUtf8Bom(null));

			byte[] preamble = Encoding.UTF8.GetPreamble();
			string preambleString = Encoding.UTF8.GetString(preamble, 0, preamble.Length);

			Assert.AreEqual(string.Empty, EncodingUtils.StripUtf8Bom(string.Empty));
			Assert.AreEqual("test", EncodingUtils.StripUtf8Bom("test"));
			Assert.AreEqual("test", EncodingUtils.StripUtf8Bom(preambleString + "test"));
		}
	}
}
