using System.Xml;
using ICD.Common.Utils.Xml;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Xml
{
	[TestFixture]
	public sealed class GenericXmlConverterTest
	{
		[XmlConverter(typeof(TestClassConverter))]
		private sealed class TestClass
		{
			public string A { get; set; }
			public int B { get; set; }
		}
		
		private sealed class TestClassConverter : AbstractGenericXmlConverter<TestClass>
		{
			private const string ELEMENT_A = "A";
			private const string ATTRIBUTE_B = "b";

			/// <summary>
			/// Override to handle the current attribute.
			/// </summary>
			/// <param name="reader"></param>
			/// <param name="instance"></param>
			protected override void ReadAttribute(IcdXmlReader reader, TestClass instance)
			{
				switch (reader.Name)
				{
					case ATTRIBUTE_B:
						instance.B = int.Parse(reader.Value);
						break;

					default:
						base.ReadAttribute(reader, instance);
						break;
				}
			}

			/// <summary>
			/// Override to handle the current element.
			/// </summary>
			/// <param name="reader"></param>
			/// <param name="instance"></param>
			protected override void ReadElement(IcdXmlReader reader, TestClass instance)
			{
				switch (reader.Name)
				{
					case ELEMENT_A:
						instance.A = reader.ReadElementContentAsString();
						break;

					default:
						base.ReadElement(reader, instance);
						break;
				}
			}

			/// <summary>
			/// Override to write attributes to the root element.
			/// </summary>
			/// <param name="writer"></param>
			/// <param name="value"></param>
			protected override void WriteAttributes(IcdXmlTextWriter writer, TestClass value)
			{
				base.WriteAttributes(writer, value);

				writer.WriteAttributeString(ATTRIBUTE_B, value.B.ToString());
			}

			/// <summary>
			/// Override to write elements to the writer.
			/// </summary>
			/// <param name="writer"></param>
			/// <param name="value"></param>
			protected override void WriteElements(IcdXmlTextWriter writer, TestClass value)
			{
				base.WriteElements(writer, value);

				writer.WriteElementString(ELEMENT_A, value.A);
			}
		}

		[Test]
		public void TestElement()
		{
			const string xml = @"<Instances>
  <Instance b=""1"">
    <A>Test</A>
  </Instance>
</Instances>";

			using (IcdXmlReader reader = new IcdXmlReader(xml))
			{
				// Read into the Instances node
				reader.Read();
				Assert.AreEqual(XmlNodeType.Element, reader.NodeType);
				Assert.AreEqual("Instances", reader.Name);

				// Read into the Instance node
				reader.Read();
				reader.SkipInsignificantWhitespace();
				Assert.AreEqual(XmlNodeType.Element, reader.NodeType);
				Assert.AreEqual("Instance", reader.Name);

				// Deserialize
				TestClass instance = IcdXmlConvert.DeserializeObject<TestClass>(reader);

				Assert.IsNotNull(instance);
				Assert.AreEqual("Test", instance.A);
				Assert.AreEqual(1, instance.B);

				// Deserialization should land on the following node
				reader.SkipInsignificantWhitespace();
				Assert.AreEqual(XmlNodeType.EndElement, reader.NodeType);
				Assert.AreEqual("Instances", reader.Name);
			}
		}

		[Test]
		public void TestEmptyElement()
		{
			const string xml = @"<Instances>
  <Instance b=""1"" />
</Instances>";

			using (IcdXmlReader reader = new IcdXmlReader(xml))
			{
				// Read into the Instances node
				reader.Read();
				Assert.AreEqual(XmlNodeType.Element, reader.NodeType);
				Assert.AreEqual("Instances", reader.Name);

				// Read into the Instance node
				reader.Read();
				reader.SkipInsignificantWhitespace();
				Assert.AreEqual(XmlNodeType.Element, reader.NodeType);
				Assert.AreEqual("Instance", reader.Name);

				// Deserialize
				TestClass instance = IcdXmlConvert.DeserializeObject<TestClass>(reader);

				Assert.IsNotNull(instance);
				Assert.IsNull(instance.A);
				Assert.AreEqual(1, instance.B);

				// Deserialization should land on the following node
				reader.SkipInsignificantWhitespace();
				Assert.AreEqual(XmlNodeType.EndElement, reader.NodeType);
				Assert.AreEqual("Instances", reader.Name);
			}
		}
	}
}
