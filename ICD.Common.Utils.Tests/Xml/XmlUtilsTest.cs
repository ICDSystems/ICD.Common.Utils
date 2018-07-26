using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using NUnit.Framework;
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

		[Flags]
		public enum eTestEnum
		{
			A = 1,
			B = 2,
			C = 4
		}

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
				reader.ReadToNextElement();

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
				reader.ReadToNextElement();

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
				reader.ReadToNextElement();
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
				reader.ReadToNextElement();
				Assert.AreEqual("Level1", reader.Name);

				reader.ReadToNextElement();
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
				reader.ReadToNextElement();

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

		#region Read Child Element

		[TestCase("<Test><Child>Test</Child></Test>", "Child", "Test")]
		public void ReadChildElementContentAsStringTest(string xml, string childElement, string expected)
		{
			Assert.AreEqual(expected, XmlUtils.ReadChildElementContentAsString(xml, childElement));
		}

		[TestCase("<Test><Child>1</Child></Test>", "Child", 1)]
		public void ReadChildElementContentAsIntTest(string xml, string childElement, int expected)
		{
			Assert.AreEqual(expected, XmlUtils.ReadChildElementContentAsInt(xml, childElement));
		}

		[TestCase("<Test><Child>1</Child></Test>", "Child", (uint)1)]
		public void ReadChildElementContentAsUintTest(string xml, string childElement, uint expected)
		{
			Assert.AreEqual(expected, XmlUtils.ReadChildElementContentAsUint(xml, childElement));
		}

		[TestCase("<Test><Child>1</Child></Test>", "Child", (long)1)]
		public void ReadChildElementContentAsLongTest(string xml, string childElement, long expected)
		{
			Assert.AreEqual(expected, XmlUtils.ReadChildElementContentAsLong(xml, childElement));
		}

		[TestCase("<Test><Child>1</Child></Test>", "Child", (ushort)1)]
		public void ReadChildElementContentAsUShortTest(string xml, string childElement, ushort expected)
		{
			Assert.AreEqual(expected, XmlUtils.ReadChildElementContentAsUShort(xml, childElement));
		}

		[TestCase("<Test><Child>1</Child></Test>", "Child", 1.0f)]
		public void ReadChildElementContentAsFloatTest(string xml, string childElement, float expected)
		{
			Assert.AreEqual(expected, XmlUtils.ReadChildElementContentAsFloat(xml, childElement), 0.001f);
		}

		[TestCase("<Test><Child>True</Child></Test>", "Child", true)]
		[TestCase("<Test><Child>true</Child></Test>", "Child", true)]
		[TestCase("<Test><Child>False</Child></Test>", "Child", false)]
		[TestCase("<Test><Child>false</Child></Test>", "Child", false)]
		public void ReadChildElementContentAsBooleanTest(string xml, string childElement, bool expected)
		{
			Assert.AreEqual(expected, XmlUtils.ReadChildElementContentAsBoolean(xml, childElement));
		}

		[TestCase("<Test><Child>1</Child></Test>", "Child", (byte)1)]
		[TestCase("<Test><Child>0x01</Child></Test>", "Child", (byte)1)]
		[TestCase("<Test><Child>0X01</Child></Test>", "Child", (byte)1)]
		public void ReadChildElementContentAsByteTest(string xml, string childElement, byte expected)
		{
			Assert.AreEqual(expected, XmlUtils.ReadChildElementContentAsByte(xml, childElement));
		}

		[TestCase("<Test><Child>A</Child></Test>", "Child", true, eTestEnum.A)]
		[TestCase("<Test><Child>A, B</Child></Test>", "Child", true, eTestEnum.A | eTestEnum.B)]
		public void ReadChildElementContentAsEnumTest<T>(string xml, string childElement, bool ignoreCase, T expected)
			where T : struct, IConvertible
		{
			Assert.AreEqual(expected, XmlUtils.ReadChildElementContentAsEnum<T>(xml, childElement, ignoreCase));
		}

		#endregion

		#region Try Read Child Element

		[TestCase("<Test><Child>Test</Child></Test>", "Child", "Test")]
		[TestCase("<Test></Test>", "Child", null)]
		public void TryReadChildElementContentAsStringTest(string xml, string childElement, string expected)
		{
			Assert.AreEqual(expected, XmlUtils.TryReadChildElementContentAsString(xml, childElement));
		}

		[TestCase("<Test><Child>1</Child></Test>", "Child", 1)]
		[TestCase("<Test><Child>fgfdgfd</Child></Test>", "Child", null)]
		[TestCase("<Test></Test>", "Child", null)]
		public void TryReadChildElementContentAsIntTest(string xml, string childElement, int? expected)
		{
			Assert.AreEqual(expected, XmlUtils.TryReadChildElementContentAsInt(xml, childElement));
		}

		[TestCase("<Test><Child>1</Child></Test>", "Child", (long)1)]
		[TestCase("<Test><Child>fgfdgfd</Child></Test>", "Child", null)]
		[TestCase("<Test></Test>", "Child", null)]
		public void TryReadChildElementContentAsLongTest(string xml, string childElement, long? expected)
		{
			Assert.AreEqual(expected, XmlUtils.TryReadChildElementContentAsLong(xml, childElement));
		}

		[TestCase("<Test><Child>1</Child></Test>", "Child", (ushort)1)]
		[TestCase("<Test><Child>fgfdgfd</Child></Test>", "Child", null)]
		[TestCase("<Test></Test>", "Child", null)]
		public void TryReadChildElementContentAsUShortTest(string xml, string childElement, ushort? expected)
		{
			Assert.AreEqual(expected, XmlUtils.TryReadChildElementContentAsUShort(xml, childElement));
		}

		[TestCase("<Test><Child>1</Child></Test>", "Child", 1.0f)]
		[TestCase("<Test><Child>fgfdgfd</Child></Test>", "Child", null)]
		[TestCase("<Test></Test>", "Child", null)]
		public void TryReadChildElementContentAsFloatTest(string xml, string childElement, float? expected)
		{
			float? result = XmlUtils.TryReadChildElementContentAsFloat(xml, childElement);

			if (result == null)
				Assert.AreEqual(expected, null);
			else if (expected == null)
				Assert.Fail();
			else
				Assert.AreEqual((float)expected, (float)result, 0.001f);
		}

		[TestCase("<Test><Child>True</Child></Test>", "Child", true)]
		[TestCase("<Test><Child>true</Child></Test>", "Child", true)]
		[TestCase("<Test><Child>False</Child></Test>", "Child", false)]
		[TestCase("<Test><Child>false</Child></Test>", "Child", false)]
		[TestCase("<Test><Child>fgfdgfd</Child></Test>", "Child", null)]
		[TestCase("<Test></Test>", "Child", null)]
		public void TryReadChildElementContentAsBooleanTest(string xml, string childElement, bool? expected)
		{
			Assert.AreEqual(expected, XmlUtils.TryReadChildElementContentAsBoolean(xml, childElement));
		}

		[TestCase("<Test><Child>1</Child></Test>", "Child", (byte)1)]
		[TestCase("<Test><Child>0x01</Child></Test>", "Child", (byte)1)]
		[TestCase("<Test><Child>0X01</Child></Test>", "Child", (byte)1)]
		[TestCase("<Test><Child>fgfdgfd</Child></Test>", "Child", null)]
		[TestCase("<Test></Test>", "Child", null)]
		public void TryReadChildElementContentAsByteTest(string xml, string childElement, byte? expected)
		{
			Assert.AreEqual(expected, XmlUtils.TryReadChildElementContentAsByte(xml, childElement));
		}

		[TestCase("<Test><Child>A</Child></Test>", "Child", true, eTestEnum.A)]
		[TestCase("<Test><Child>A, B</Child></Test>", "Child", true, eTestEnum.A | eTestEnum.B)]
		[TestCase("<Test><Child>fgfdgfd</Child></Test>", "Child", true, null)]
		[TestCase("<Test></Test>", "Child", true, null)]
		public void TryReadChildElementContentAsEnumTest(string xml, string childElement, bool ignoreCase, eTestEnum? expected)
		{
			Assert.AreEqual(expected, XmlUtils.TryReadChildElementContentAsEnum<eTestEnum>(xml, childElement, ignoreCase));
		}

		[TestCase("<Test><Child>A</Child></Test>", "Child", true, true)]
		[TestCase("<Test><Child>A, B</Child></Test>", "Child", true, true)]
		[TestCase("<Test><Child>fgfdgfd</Child></Test>", "Child", true, false)]
		[TestCase("<Test></Test>", "Child", true, false)]
		public void TryReadChildElementContentAsEnumTest(string xml, string childElement, bool ignoreCase, bool expected)
		{
			eTestEnum output;
			Assert.AreEqual(expected, XmlUtils.TryReadChildElementContentAsEnum(xml, childElement, ignoreCase, out output));
		}

		#endregion
	}
}