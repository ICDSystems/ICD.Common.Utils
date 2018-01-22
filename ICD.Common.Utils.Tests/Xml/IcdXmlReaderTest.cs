using ICD.Common.Utils.Xml;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Xml
{
	[TestFixture]
    public sealed class IcdXmlReaderTest
    {
	    // Whitespace is important for testing Insignificant Whitespace nodes.
	    private const string EXAMPLE_XML = "   <Level1 attr1=\"1\" attr2=\"2\">          "
	                                       + "  <Level2>    "
	                                       + "  </Level2>     "
	                                       + " <Level2>    "
	                                       + " <Level3>Some text</Level3>     "
	                                       + " </Level2>   "
	                                       + "  </Level1>     ";

	    // For testing empty elements
	    private const string EXAMPLE_XML_2 = "<Level1>"
	                                         + "<Level2 />"
	                                         + "<Level2 />"
	                                         + "</Level1>";

		#region Properties

		public void HasAttributesTest()
		{
			IcdXmlReader reader = new IcdXmlReader(EXAMPLE_XML);
			reader.ReadToNextElement();
			Assert.IsTrue(reader.HasAttributes);

			reader = new IcdXmlReader(EXAMPLE_XML_2);
			reader.ReadToNextElement();
			Assert.IsFalse(reader.HasAttributes);
		}

		public void NameTest()
		{
			IcdXmlReader reader = new IcdXmlReader(EXAMPLE_XML);
			reader.ReadToNextElement();

			Assert.AreEqual("Level1", reader.Name);

			reader.ReadToNextElement();

			Assert.AreEqual("Level2", reader.Name);
		}

		public void ValueTest()
		{
			IcdXmlReader reader = new IcdXmlReader("<Test>test</Test>");
			reader.ReadToNextElement();

			Assert.AreEqual("test", reader.Value);

			reader = new IcdXmlReader("<Test></Test>");
			reader.ReadToNextElement();

			Assert.AreEqual("", reader.Value);

			reader = new IcdXmlReader("<Test />");
			reader.ReadToNextElement();

			Assert.AreEqual(null, reader.Value);
		}

		public void NodeTypeTest()
		{
			Assert.Inconclusive();
		}

		#endregion

		#region Methods

		public void MoveToNextAttributeTest()
		{
			Assert.Inconclusive();
		}

		public void MoveToElementTest()
		{
			Assert.Inconclusive();
		}

		public void GetAttributeTest()
		{
			IcdXmlReader reader = new IcdXmlReader(EXAMPLE_XML);
			reader.ReadToNextElement();

			Assert.AreEqual("1", reader.GetAttribute("attr1"));
			Assert.AreEqual("2", reader.GetAttribute("attr2"));
			Assert.AreEqual(null, reader.GetAttribute("attr3"));
		}

		public void ReadStringTest()
		{
			Assert.Inconclusive();
		}

		public void ReadTest()
		{
			Assert.Inconclusive();
		}

		public void SkipTest()
		{
			Assert.Inconclusive();
		}

		public void ReadElementContentAsStringTest()
		{
			Assert.Inconclusive();
		}

		public void ReadOuterXmlTest()
		{
			Assert.Inconclusive();
		}

		public void ReadInnerXmlTest()
		{
			Assert.Inconclusive();
		}

		public void ReadElementContentAsLongTest()
		{
			Assert.Inconclusive();
		}

		public void ReadElementContentAsFloatTest()
		{
			Assert.Inconclusive();
		}

		#endregion
	}
}
