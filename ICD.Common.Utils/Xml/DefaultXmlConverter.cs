using System;

namespace ICD.Common.Utils.Xml
{
    public sealed class DefaultXmlConverter : AbstractXmlConverter
	{
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
	}
}
