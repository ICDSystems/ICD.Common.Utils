using System;
using ICD.Common.Properties;
using ICD.Common.Utils.IO;

namespace ICD.Common.Utils.Csv
{
	public sealed class CsvWriter : IDisposable
	{
		private const string QUOTATION_MARK = "\"";
		private const string DOUBLE_QUOTE_MARK = "\"\"";

		private readonly IcdTextWriter m_Writer;
		private readonly CsvWriterSettings m_Settings;

		/// <summary>
		/// Are we currently at the beginning of a new line?
		/// </summary>
		private bool m_NewLine;

		#region Properties

		private string Separator { get { return m_Settings.InsertSpaceAfterComma ? ", " : ","; } }

		private string LineTerminator { get { return m_Settings.NewLineSequence; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public CsvWriter([NotNull] IcdTextWriter writer,
		                 [NotNull] CsvWriterSettings settings)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			if (settings == null)
				throw new ArgumentNullException("settings");

			m_NewLine = true;
			m_Writer = writer;
			m_Settings = settings;
		}

		#endregion

		public void Dispose()
		{
			m_Writer.Dispose();
		}

		#region Methods


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
			value = value ?? string.Empty;

			if (!m_NewLine)
				m_Writer.WrappedTextWriter.Write(Separator);

			if (m_Settings.AlwaysEscapeEveryValue || value.Contains(","))
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
			m_Writer.WrappedTextWriter.Write(LineTerminator);

			m_NewLine = true;
		}

		#endregion
	}
}
