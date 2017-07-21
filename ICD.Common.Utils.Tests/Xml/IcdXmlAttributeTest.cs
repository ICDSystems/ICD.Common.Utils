using ICD.Common.Properties;
using ICD.Common.Utils.Xml;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Xml
{
	[TestFixture]
	public sealed class IcdXmlAttributeTest
	{
		[Test, UsedImplicitly]
		public void ValueAsIntTest()
		{
			IcdXmlAttribute attribute = new IcdXmlAttribute("test", "12");
			Assert.AreEqual("12", attribute.Value);
		}
	}
}