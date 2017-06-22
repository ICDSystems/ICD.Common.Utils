using System;

namespace ICD.Common.Utils.Xml
{
	public sealed class IcdXmlException : Exception
	{
		private int m_LineNumber;
		private int m_LinePosition;

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
	}
}
