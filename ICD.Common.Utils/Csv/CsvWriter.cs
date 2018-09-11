using System;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.IO;

namespace ICD.Common.Utils.Csv
{
	public sealed class CsvWriter : IDisposable
	{
		private const string QUOTATION_MARK = "\"";
		private const string DOUBLE_QUOTE_MARK = "\"\"";

		private readonly IcdTextWriter m_Writer;

		private readonly string m_Seperator;
		private readonly string m_LineTerminator;
		private readonly bool m_AlwaysEscape;

		private bool m_NewLine;

		/// <summary>
		/// Constructor.
		/// </summary>
		public CsvWriter(IcdTextWriter writer, bool spaceAfterComma, bool alwaysEscape, string newline, params string[] header)
		{
			m_NewLine = true;
			m_Writer = writer;
			m_Seperator = spaceAfterComma ? ", " : ",";
			m_AlwaysEscape = alwaysEscape;
			m_LineTerminator = newline;

			if(header.Any())
				AppendRow(header);
		}

		~CsvWriter()
		{
			Dispose();
		}

		/// <summary>
		/// Calls ToString() for each item and adds the row to the builder.
		/// </summary>
		/// <param name="row"></param>
		[PublicAPI]
		public void AppendRow(params object[] row)
		{
			foreach (object value in row)
				AppendValue(value);
			AppendNewline();
		}

		/// <summary>
		/// Adds the row to the builder.
		/// </summary>
		/// <param name="row"></param>
		[PublicAPI]
		public void AppendRow(params string[] row)
		{
			foreach (string value in row)
				AppendValue(value);
			AppendNewline();
		}

		/// <summary>
		/// Calls ToString() on the item and adds it to the builder.
		/// </summary>
		/// <param name="value"></param>
		[PublicAPI]
		public void AppendValue(object value)
		{
			AppendValue(string.Format("{0}", value));
		}

		/// <summary>
		/// Adds a value to the builder.
		/// </summary>
		/// <param name="value"></param>
		[PublicAPI]
		public void AppendValue(string value)
		{
			if (!m_NewLine)
				m_Writer.WrappedTextWriter.Write(m_Seperator);

			if (m_AlwaysEscape || value.Contains(","))
			{
				value = value.Replace(QUOTATION_MARK, DOUBLE_QUOTE_MARK);

				// Append the value, surrounded by quotes
				m_Writer.WrappedTextWriter.Write(QUOTATION_MARK);
				m_Writer.WrappedTextWriter.Write(value);
				m_Writer.WrappedTextWriter.Write(QUOTATION_MARK);
			}
			else
			{
				m_Writer.WrappedTextWriter.Write(value);
			}

			m_NewLine = false;
		}

		/// <summary>
		/// Adds a New Line To the Builder
		/// </summary>
		[PublicAPI]
		public void AppendNewline()
		{
			m_Writer.WrappedTextWriter.Write(m_LineTerminator);

			m_NewLine = true;
		}

		public void Dispose()
		{
			m_Writer.Dispose();
		}

		/// <summary>
		/// Instantiates a new CsvWriter with the properties given in the CsvWriterSettings.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="settings"></param>
		/// <param name="header"></param>
		/// <returns></returns>
		[PublicAPI]
		public static CsvWriter Create(IcdTextWriter writer, CsvWriterSettings settings, params string[] header)
		{
			return new CsvWriter(writer,
			                     settings.InsertSpaceAfterComma,
			                     settings.AlwaysEscapeEveryValue,
			                     settings.NewLineSequence,
			                     header);
		}
	}
}