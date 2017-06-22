using System;
#if SIMPLSHARP
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronXml;
#else
using System.Xml;
using System.IO;
#endif

namespace ICD.Common.Utils.Xml
{
	public sealed class IcdXmlReader : IDisposable
	{
		private readonly XmlReader m_Reader;

		#region Properties

		public bool HasAttributes { get { return m_Reader.HasAttributes; } }

		public string Name { get { return m_Reader.Name; } }

		public string Value { get { return m_Reader.Value; } }

		public XmlNodeType NodeType { get { return m_Reader.NodeType; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="xml"></param>
		public IcdXmlReader(string xml)
		{
			if (xml == null)
				throw new ArgumentNullException("xml");

#if SIMPLSHARP
			m_Reader = new XmlReader(xml);
#else
            m_Reader = XmlReader.Create(new StringReader(xml));
#endif
		}

		~IcdXmlReader()
		{
			Dispose();
		}

		#region Methods

		public bool MoveToNextAttribute()
		{
			return m_Reader.MoveToNextAttribute();
		}

		public void MoveToElement()
		{
			m_Reader.MoveToElement();
		}

		public string GetAttribute(string name)
		{
			return m_Reader.GetAttribute(name);
		}

		public string ReadString()
		{
#if SIMPLSHARP
			return m_Reader.ReadString();
#else
            return m_Reader.ReadElementContentAsString();
#endif
		}

		public bool Read()
		{
			try
			{
				return m_Reader.Read();
			}
			catch (XmlException e)
			{
				throw new IcdXmlException(e.Message, e, e.LineNumber, e.LinePosition);
			}
		}

		public void Dispose()
		{
#if SIMPLSHARP
			m_Reader.Dispose(true);
#else
            m_Reader.Dispose();
#endif
		}

		public void Skip()
		{
			m_Reader.Skip();
		}

		public string ReadElementContentAsString()
		{
			return m_Reader.ReadElementContentAsString();
		}

		public string ReadOuterXml()
		{
			return m_Reader.ReadOuterXml();
		}

		public string ReadInnerXml()
		{
			return m_Reader.ReadInnerXml();
		}

		public long ReadElementContentAsLong()
		{
			return m_Reader.ReadElementContentAsLong();
		}

		public float ReadElementContentAsFloat()
		{
			return m_Reader.ReadElementContentAsFloat();
		}

		#endregion
	}
}
