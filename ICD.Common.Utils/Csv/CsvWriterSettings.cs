using ICD.Common.Properties;

namespace ICD.Common.Utils.Csv
{
	public sealed class CsvWriterSettings
	{
		private bool m_InsertSpaceAfterComma = true;
		private bool m_AlwaysEscapeEveryValue = true;
		private string m_NewLineSequence = IcdEnvironment.NewLine;

		/// <summary>
		/// Gets/Sets whether to insert a space between elements, after the comma
		/// Defaults to true.
		/// </summary>
		[PublicAPI]
		public bool InsertSpaceAfterComma
		{
			get { return m_InsertSpaceAfterComma; }
			set { m_InsertSpaceAfterComma = value; }
		}

		/// <summary>
		/// Gets/Sets whether to always escape the values.
		/// If true, values are recorded surrounded by quotes, regardless of if they contain a comma or not. Quotes are escaped. 
		/// If false, values are recorded as the value without quotes, unless escaping is required.
		/// Defaults to true.
		/// </summary>
		[PublicAPI]
		public bool AlwaysEscapeEveryValue
		{
			get { return m_AlwaysEscapeEveryValue; }
			set { m_AlwaysEscapeEveryValue = value; }
		}

		/// <summary>
		/// Gets/Sets the newline character or characters to deliniate records.
		/// Defaults to System.NewLine.
		/// </summary>
		[PublicAPI]
		public string NewLineSequence
		{
			get { return m_NewLineSequence; }
			set { m_NewLineSequence = value; }
		}
	}
}