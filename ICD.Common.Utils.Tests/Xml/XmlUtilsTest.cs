using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;
#if SIMPLSHARP
using Crestron.Xml;
#else
using System.Xml;
#endif

namespace ICD.Common.Utils.Tests.Xml
{
	[TestFixture]
	public sealed class XmlUtilsTest
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
		public void HasAttributeTest()
		{
			Assert.IsTrue(XmlUtils.HasAttribute(EXAMPLE_XML, "attr1"));
			Assert.IsTrue(XmlUtils.HasAttribute(EXAMPLE_XML, "attr2"));
			Assert.IsFalse(XmlUtils.HasAttribute(EXAMPLE_XML, "attr3"));
			Assert.IsFalse(XmlUtils.HasAttribute(EXAMPLE_XML_2, "attr1"));
		}

		[Test, UsedImplicitly]
		public void GetAttributesTest()
		{
			using (IcdXmlReader reader = new IcdXmlReader(EXAMPLE_XML))
			{
				reader.SkipToNextElement();

				IcdXmlAttribute[] attributes = reader.GetAttributes().ToArray();

				Assert.AreEqual(2, attributes.Length);
				Assert.AreEqual("attr1", attributes[0].Name);
				Assert.AreEqual("1", attributes[0].Value);
				Assert.AreEqual("attr2", attributes[1].Name);
				Assert.AreEqual("2", attributes[1].Value);
			}
		}

		[Test, UsedImplicitly]
		public void GetAttributeAsIntTest()
		{
			using (IcdXmlReader reader = new IcdXmlReader(EXAMPLE_XML))
			{
				reader.SkipToNextElement();

				int value = reader.GetAttributeAsInt("attr1");
				Assert.AreEqual(1, value);
			}
		}

		#endregion

		[Test, UsedImplicitly]
		public void RecursionTest()
		{
			List<string[]> paths = new List<string[]>();
			List<string> nodes = new List<string>();

			XmlUtils.Recurse(EXAMPLE_XML, args =>
			                              {
				                              paths.Add(args.Path);
				                              nodes.Add(args.Outer);
			                              }
				);

			Assert.AreEqual(4, paths.Count);

			Assert.AreEqual("Level1", paths[0][0]);
			Assert.AreEqual("Level2", paths[1][1]);
			Assert.AreEqual("Level2", paths[2][1]);
			Assert.AreEqual("Level3", paths[3][2]);

			using (IcdXmlReader reader = new IcdXmlReader(nodes[3]))
			{
				reader.SkipToNextElement();
				Assert.AreEqual("Some text", reader.ReadInnerXml());
			}
		}

		[Test, UsedImplicitly]
		public void SkipInsignificantWhitespaceTest()
		{
			using (IcdXmlReader reader = new IcdXmlReader(EXAMPLE_XML))
			{
				reader.Read();
				Assert.AreEqual(reader.NodeType, XmlNodeType.Whitespace);

				reader.SkipInsignificantWhitespace();
				Assert.AreNotEqual(reader.NodeType, XmlNodeType.Whitespace);
			}
		}

		[Test, UsedImplicitly]
		public void SkipToNextElementTest()
		{
			using (IcdXmlReader reader = new IcdXmlReader(EXAMPLE_XML))
			{
				reader.SkipToNextElement();
				Assert.AreEqual("Level1", reader.Name);

				reader.SkipToNextElement();
				Assert.AreEqual("Level2", reader.Name);
			}
		}

		[Test, UsedImplicitly]
		public void GetChildElementsFromXmlTest()
		{
			IEnumerable<IcdXmlReader> results = XmlUtils.GetChildElements(EXAMPLE_XML);
			IcdXmlReader[] children = results.ToArray();

			Assert.AreEqual(2, children.Length);
			Assert.AreEqual("Level2", children[0].Name);
			Assert.AreEqual("Level2", children[1].Name);

			foreach (IcdXmlReader child in children)
				child.Dispose();
		}

		[Test, UsedImplicitly]
		public void GetChildElementsAsStringEmptyElementTest()
		{
			string[] results = XmlUtils.GetChildElementsAsString(EXAMPLE_XML_2).ToArray();

			Assert.AreEqual(2, results.Length);
			Assert.AreEqual("<Level2 />", results[0]);
			Assert.AreEqual("<Level2 />", results[1]);
		}

		[Test, UsedImplicitly]
		public void GetChildElementsTest()
		{
			using (IcdXmlReader reader = new IcdXmlReader(EXAMPLE_XML))
			{
				reader.SkipToNextElement();

				IEnumerable<IcdXmlReader> results = reader.GetChildElements();
				IcdXmlReader[] children = results.ToArray();

				Assert.AreEqual(2, children.Length);
				Assert.AreEqual("Level2", children[0].Name);
				Assert.AreEqual("Level2", children[1].Name);

				foreach (IcdXmlReader child in children)
					child.Dispose();
			}
		}

		[Test, UsedImplicitly]
		public void IsValidXmlTest()
		{
			Assert.IsFalse(XmlUtils.IsValidXml(@"<Foo></Bar>"));
			Assert.IsTrue(XmlUtils.IsValidXml(EXAMPLE_XML));
		}

		[Test]
		public void FormatTest()
		{
			const string xml = "<Test1><Test2></Test2><Test2></Test2></Test1>";
			const string expected = @"<Test1>
  <Test2></Test2>
  <Test2></Test2>
</Test1>";

			Assert.AreEqual(expected, XmlUtils.Format(xml));
		}
	}
}