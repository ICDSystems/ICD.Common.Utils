using System;
#if SIMPLSHARP
using Crestron.SimplSharp;
#else
using System.Xml;
#endif

namespace ICD.Common.Utils.Xml
{
	public sealed class IcdXmlException : Exception
	{
		private readonly int m_LineNumber;
		private readonly int m_LinePosition;

		public int LineNumber { get { return m_LineNumber; } }

		public int LinePosition { get { return m_LinePosition; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		/// <param name="lineNumber"></param>
		/// <param name="linePosition"></param>
		public IcdXmlException(string message, Exception inner, int lineNumber, int linePosition)
			: base(message, inner)
		{
			m_LineNumber = lineNumber;
			m_LinePosition = linePosition;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="inner"></param>
		public IcdXmlException(XmlException inner)
			: this(inner.Message, inner, inner.LineNumber, inner.LinePosition)
		{
		}
	}
}
