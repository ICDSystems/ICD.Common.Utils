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
			reader.SkipToNextElement();
			Assert.IsTrue(reader.HasAttributes);

			reader = new IcdXmlReader(EXAMPLE_XML_2);
			reader.SkipToNextElement();
			Assert.IsFalse(reader.HasAttributes);
		}

		public void NameTest()
		{
			IcdXmlReader reader = new IcdXmlReader(EXAMPLE_XML);
			reader.SkipToNextElement();

			Assert.AreEqual("Level1", reader.Name);

			reader.SkipToNextElement();

			Assert.AreEqual("Level2", reader.Name);
		}

		public void ValueTest()
		{
			Assert.Inconclusive();
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
			Assert.Inconclusive();
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
