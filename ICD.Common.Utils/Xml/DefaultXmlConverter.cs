using System;
using System.Collections.Generic;

namespace ICD.Common.Utils.Xml
{
	public sealed class DefaultXmlConverter : AbstractXmlConverter
	{
		private static readonly Dictionary<Type, DefaultXmlConverter> s_Instances;

		private readonly Type m_SerializeType;

		/// <summary>
		/// Static constructor.
		/// </summary>
		static DefaultXmlConverter()
		{
			s_Instances = new Dictionary<Type, DefaultXmlConverter>();
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="serializeType"></param>
		private DefaultXmlConverter(Type serializeType)
		{
			m_SerializeType = serializeType;
		}

		/// <summary>
		/// Writes the XML representation of the object.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="elementName"></param>
		/// <param name="value">The value.</param>
		public override void WriteXml(IcdXmlTextWriter writer, string elementName, object value)
		{
			string elementString = IcdXmlConvert.ToString(value);

			writer.WriteElementString(elementName, elementString);
		}

		/// <summary>
		/// Reads the XML representation of the object.
		/// </summary>
		/// <param name="reader">The XmlReader to read from.</param>
		/// <returns>
		/// The object value.
		/// </returns>
		public override object ReadXml(IcdXmlReader reader)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets the converter instance for the given serialization type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IXmlConverter Instance(Type type)
		{
			DefaultXmlConverter converter;

			if (!s_Instances.TryGetValue(type, out converter))
			{
				converter = new DefaultXmlConverter(type);
				s_Instances[type] = converter;
			}

			return converter;
		}
	}
}
