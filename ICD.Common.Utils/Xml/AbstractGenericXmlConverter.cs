using System;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronXml;
#else
using System.Xml;
#endif
using ICD.Common.Properties;

namespace ICD.Common.Utils.Xml
{
	public abstract class AbstractGenericXmlConverter<T> : AbstractXmlConverter
	{
		/// <summary>
		/// Creates a new instance of T.
		/// </summary>
		/// <returns></returns>
		protected virtual T Instantiate()
		{
			return ReflectionUtils.CreateInstance<T>();
		}

		/// <summary>
		/// Writes the XML representation of the object.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="elementName"></param>
		/// <param name="value">The value.</param>
		public sealed override void WriteXml(IcdXmlTextWriter writer, string elementName, object value)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			if (value == null)
			{
				writer.WriteElementString(elementName, null);
				return;
			}

			WriteXml(writer, elementName, (T)value);
		}

		/// <summary>
		/// Writes the XML representation of the object.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="elementName"></param>
		/// <param name="value">The value.</param>
		[PublicAPI]
		public void WriteXml(IcdXmlTextWriter writer, string elementName, T value)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			if (value == null)
			{
				writer.WriteElementString(elementName, null);
				return;
			}

			writer.WriteStartElement(elementName);
			{
				WriteAttributes(writer, value);
				WriteElements(writer, value);
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Override to write attributes to the root element.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="value"></param>
		protected virtual void WriteAttributes(IcdXmlTextWriter writer, T value)
		{
		}

		/// <summary>
		/// Override to write elements to the writer.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="value"></param>
		protected virtual void WriteElements(IcdXmlTextWriter writer, T value)
		{
		}

		/// <summary>
		/// Reads the XML representation of the object.
		/// </summary>
		/// <param name="reader">The XmlReader to read from.</param>
		/// <returns>
		/// The object value.
		/// </returns>
		public sealed override object ReadXml(IcdXmlReader reader)
		{
			return ReadXmlTyped(reader);
		}

		/// <summary>
		/// Reads the XML representation of the object.
		/// </summary>
		/// <param name="reader">The XmlReader to read from.</param>
		/// <returns>
		/// The object value.
		/// </returns>
		[PublicAPI]
		public virtual T ReadXmlTyped(IcdXmlReader reader)
		{
			// Read into the first node
			if (!reader.Read())
				throw new FormatException();

			T output = default(T);
			bool instantiated = false;

			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (!instantiated)
				{
					switch (reader.NodeType)
					{
						case XmlNodeType.Element:
							if (reader.IsEmptyElement)
								return default(T);

							output = Instantiate();
							instantiated = true;

							// Read out of the root element
							if (!reader.Read())
								throw new FormatException();
							continue;

						default:
							// Keep reading until we reach the root element.
							if (!reader.Read())
								throw new FormatException();
							continue;
					}
				}

				switch (reader.NodeType)
				{
					case XmlNodeType.Attribute:
						ReadAttribute(reader, output);
						break;

					case XmlNodeType.Element:
						ReadElement(reader, output);
						break;

					case XmlNodeType.EndElement:
						return output;

					default:
						if (!reader.Read())
							return output;
						break;
				}
			}

			return output;
		}

		/// <summary>
		/// Override to handle the current attribute.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="instance"></param>
		protected virtual void ReadAttribute(IcdXmlReader reader, T instance)
		{
			// Skip the attribute
			reader.Read();
		}

		/// <summary>
		/// Override to handle the current element.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="instance"></param>
		protected virtual void ReadElement(IcdXmlReader reader, T instance)
		{
			// Skip the element
			reader.Skip();
		}
	}
}
