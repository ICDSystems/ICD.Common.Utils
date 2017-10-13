using System;
using System.Linq;
using ICD.Common.Utils.Xml;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Extensions
{
	[TestFixture]
    public sealed class XmlReaderExtensionsTest
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

		#region Attributes

		[Test]
		public static void HasAttributeTest()
		{
			IcdXmlReader reader = new IcdXmlReader(EXAMPLE_XML);

			Assert.IsFalse(reader.HasAttribute("attr1"));

			reader.SkipToNextElement();

			Assert.IsTrue(reader.HasAttribute("attr1"));
			Assert.IsTrue(reader.HasAttribute("attr2"));
			Assert.IsFalse(reader.HasAttribute("attr3"));
			Assert.IsFalse(reader.HasAttribute("Attr1"));
		}

	    [Test]
		public static void GetAttributesTest()
		{
			IcdXmlReader reader = new IcdXmlReader(EXAMPLE_XML);

			Assert.IsEmpty(reader.GetAttributes());

			reader.SkipToNextElement();

			IcdXmlAttribute[] attributes = reader.GetAttributes().ToArray();
			Assert.AreEqual(2, attributes.Length);
			Assert.AreEqual("attr1", attributes[0].Name);
			Assert.AreEqual("attr2", attributes[1].Name);
		}

	    [Test]
		public static void GetAttributeAsStringTest()
		{
			IcdXmlReader reader = new IcdXmlReader(EXAMPLE_XML);

			Assert.Throws<FormatException>(() => reader.GetAttributeAsString("attr1"));

			reader.SkipToNextElement();

			Assert.AreEqual("1", reader.GetAttributeAsString("attr1"));
			Assert.AreEqual("2", reader.GetAttributeAsString("attr2"));
			Assert.Throws<FormatException>(() => reader.GetAttributeAsString("attr3"));
			Assert.Throws<FormatException>(() => reader.GetAttributeAsString("Attr1"));
		}

	    [Test]
		public static void GetAttributeAsIntTest()
		{
			IcdXmlReader reader = new IcdXmlReader(EXAMPLE_XML);

			Assert.Throws<FormatException>(() => reader.GetAttributeAsInt("attr1"));

			reader.SkipToNextElement();

			Assert.AreEqual(1, reader.GetAttributeAsInt("attr1"));
			Assert.AreEqual(2, reader.GetAttributeAsInt("attr2"));
			Assert.Throws<FormatException>(() => reader.GetAttributeAsInt("attr3"));
			Assert.Throws<FormatException>(() => reader.GetAttributeAsInt("Attr1"));
		}

	    [Test]
		public static void GetAttributeAsBoolTest()
		{
			Assert.Inconclusive();
		}

		#endregion

		#region Recurse

	    [Test]
		public static void RecurseTest()
		{
			Assert.Inconclusive();
		}

		#endregion

		#region Skip

	    [Test]
		public static void SkipInsignificantWhitespaceTest()
		{
			Assert.Inconclusive();
		}

	    [Test]
		public static void SkipToNextElementTest()
		{
			Assert.Inconclusive();
		}

		#endregion

		#region Get Child Element

	    [Test]
		public static void HasChildElementsTest()
		{
			Assert.Inconclusive();
		}

	    [Test]
		public static void GetChildElementsTest()
		{
			Assert.Inconclusive();
		}

	    [Test]
		public static void GetChildElementsAsStringTest()
		{
			Assert.Inconclusive();
		}

		#endregion

		#region Read Element Content

	    [Test]
		public static void ReadElementContentAsByteTest()
		{
			Assert.Inconclusive();
		}

	    [Test]
		public static void ReadElementContentAsUintTest()
		{
			Assert.Inconclusive();
		}

	    [Test]
		public static void ReadElementContentAsIntTest()
		{
			Assert.Inconclusive();
		}

	    [Test]
		public static void ReadElementContentAsUShortTest()
		{
			Assert.Inconclusive();
		}

	    [Test]
		public static void ReadElementContentAsEnumTest()
		{
			Assert.Inconclusive();
		}

		#endregion
	}
}
