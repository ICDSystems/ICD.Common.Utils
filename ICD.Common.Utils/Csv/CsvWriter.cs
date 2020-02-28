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
		private readonly CsvWriterSettings m_Settings;
		private bool m_NewLine;

		#region Properties

		private string Separator { get { return m_Settings.InsertSpaceAfterComma ? ", " : ","; } }

		private string LineTerminator { get { return m_Settings.NewLineSequence; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public CsvWriter([NotNull] IcdTextWriter writer, [NotNull] CsvWriterSettings settings, [NotNull] params string[] header)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			if (settings == null)
				throw new ArgumentNullException("settings");

			if (header == null)
				throw new ArgumentNullException("header");

			m_NewLine = true;
			m_Writer = writer;
			m_Settings = settings;

			if (header.Any())
				AppendRow(header);
		}

		/// <summary>
		/// Deconstructor.
		/// </summary>
		~CsvWriter()
		{
			Dispose();
		}

		/// <summary>
		/// Instantiates a new CsvWriter with the properties given in the CsvWriterSettings.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="settings"></param>
		/// <param name="header"></param>
		/// <returns></returns>
		[PublicAPI]
		public static CsvWriter Create([NotNull] IcdTextWriter writer, [NotNull] CsvWriterSettings settings,
		                               [NotNull] params string[] header)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			if (settings == null)
				throw new ArgumentNullException("settings");

			if (header == null)
				throw new ArgumentNullException("header");

			return new CsvWriter(writer, settings, header);
		}

		#endregion

		public void Dispose()
		{
			m_Writer.Dispose();
		}

		#region Methods

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
