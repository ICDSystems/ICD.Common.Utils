using ICD.Common.Properties;

namespace ICD.Common.Utils.Xml
{
    public abstract class AbstractXmlConverter : IXmlConverter
	{
		/// <summary>
		/// Writes the XML representation of the object.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="elementName"></param>
		/// <param name="value">The value.</param>
		public abstract void WriteXml(IcdXmlTextWriter writer, string elementName, object value);

		/// <summary>
		/// Reads the XML representation of the object.
		/// </summary>
		/// <param name="reader">The XmlReader to read from.</param>
		/// <returns>
		/// The object value.
		/// </returns>
		[PublicAPI]
		public abstract object ReadXml(IcdXmlReader reader);
	}
}
