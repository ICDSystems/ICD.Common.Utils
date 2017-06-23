#if SIMPLSHARP
using System.Text;
using Crestron.SimplSharp.CrestronXml;
using ICD.Common.Utils.IO;

namespace ICD.Common.Utils.Xml
{
	public sealed partial class IcdXmlTextWriter
	{
		private readonly XmlTextWriter m_Writer;

		public XmlWriter WrappedWriter { get { return m_Writer; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="encoding"></param>
		public IcdXmlTextWriter(IcdStream stream, Encoding encoding)
			: this(new XmlTextWriter(stream.WrappedStream, encoding))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="textWriter"></param>
		public IcdXmlTextWriter(IcdTextWriter textWriter)
			: this(new XmlTextWriter(textWriter.WrappedTextWriter))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="writer"></param>
		public IcdXmlTextWriter(XmlTextWriter writer)
		{
			m_Writer = writer;
			m_Writer.Formatting = Crestron.SimplSharp.CrestronXml.Formatting.Indented;
		}

		#region Methods

		public void WriteStartElement(string elementName)
		{
			m_Writer.WriteStartElement(elementName);
		}

		public void WriteElementString(string elementName, string value)
		{
			m_Writer.WriteElementString(elementName, value);
		}

		public void WriteEndElement()
		{
			m_Writer.WriteEndElement();
		}

		public void WriteComment(string comment)
		{
			m_Writer.WriteComment(comment);
		}

		public void Dispose()
		{
			m_Writer.Dispose(true);
		}

		public void WriteAttributeString(string attributeName, string value)
		{
			m_Writer.WriteAttributeString(attributeName, value);
		}

		public void Flush()
		{
			m_Writer.Flush();
		}

		public void Close()
		{
			m_Writer.Close();
		}

		public void WriteRaw(string xml)
		{
			m_Writer.WriteRaw(xml);
		}

		#endregion
	}
}

#endif
