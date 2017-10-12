#if SIMPLSHARP
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronXml;
#else
using System.Xml;
#endif

namespace ICD.Common.Utils.Xml
{
	public sealed class IcdXmlDocument
	{
		private readonly XmlDocument m_Document;

		/// <summary>
		/// Constructor.
		/// </summary>
		public IcdXmlDocument()
		{
			m_Document = new XmlDocument();
		}

		public void LoadXml(string xml)
		{
			try
			{
				m_Document.LoadXml(xml);
			}
			catch (XmlException e)
			{
				throw new IcdXmlException(e);
			}
		}

		public void WriteContentTo(IcdXmlTextWriter writer)
		{
			m_Document.WriteContentTo(writer.WrappedWriter);
		}
	}
}
