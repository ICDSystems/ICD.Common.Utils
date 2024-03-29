﻿using System;
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

		public bool IsEmptyElement { get { return m_Reader.IsEmptyElement; } }

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
			try
			{
				return m_Reader.MoveToNextAttribute();
			}
			catch (XmlException e)
			{
				throw new IcdXmlException(e);
			}
		}

		public void MoveToElement()
		{
			try
			{
				m_Reader.MoveToElement();
			}
			catch (XmlException e)
			{
				throw new IcdXmlException(e);
			}
		}

		public string GetAttribute(string name)
		{
			try
			{
				return m_Reader.GetAttribute(name);
			}
			catch (XmlException e)
			{
				throw new IcdXmlException(e);
			}
		}

		public string ReadString()
		{
			try
			{
#if SIMPLSHARP
				return m_Reader.ReadString();
#else
				return m_Reader.ReadElementContentAsString();
#endif
			}
			catch (XmlException e)
			{
				throw new IcdXmlException(e);
			}
		}

		public bool Read()
		{
			try
			{
				return m_Reader.Read();
			}
			catch (XmlException e)
			{
				throw new IcdXmlException(e);
			}
		}

		public void Dispose()
		{
			if (m_Reader == null)
				return;

#if SIMPLSHARP
				m_Reader.Dispose(true);
#else
				m_Reader.Dispose();
#endif
		}

		public void Skip()
		{
			try
			{
				m_Reader.Skip();
			}
			catch (XmlException e)
			{
				throw new IcdXmlException(e);
			}
		}

		public string ReadOuterXml()
		{
			try
			{
				return m_Reader.ReadOuterXml();
			}
			catch (XmlException e)
			{
				throw new IcdXmlException(e);
			}
		}

		public string ReadInnerXml()
		{
			try
			{
				return m_Reader.ReadInnerXml();
			}
			catch (XmlException e)
			{
				throw new IcdXmlException(e);
			}
		}

		public string ReadElementContentAsString()
		{
			try
			{
				return m_Reader.ReadElementContentAsString();
			}
			catch (XmlException e)
			{
				string message = string.Format("Unable to read element '{0}' content as string", m_Reader.Name);
				throw new IcdXmlException(message, e, e.LineNumber, e.LinePosition);
			}
		}

		public long ReadElementContentAsLong()
		{
			// ReadElementContentAsLong() logs and throws...
			string value = ReadElementContentAsString();
			return long.Parse(value);
		}

		public float ReadElementContentAsFloat()
		{
			// ReadElementContentAsFloat() logs and throws...
			string value = ReadElementContentAsString();
			return float.Parse(value);
		}

		public double ReadElementContentAsDouble()
		{
			// ReadElementContentAsDouble() logs and throws...
			string value = ReadElementContentAsString();
			return double.Parse(value);
		}

		public bool ReadElementContentAsBoolean()
		{
			// ReadElementContentAsBoolean() is too case sensitive
			string value = ReadElementContentAsString();
			return bool.Parse(value);
		}

		#endregion
	}
}
