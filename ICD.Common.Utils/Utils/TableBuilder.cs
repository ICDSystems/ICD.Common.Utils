using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils
{
	/// <summary>
	/// TableBuilder provides a way to format a collection of strings to a table.
	/// </summary>
	public sealed class TableBuilder
	{
		private const char HORIZONTAL = '-';
		private const char VERTICAL = '|';

		private readonly List<string[]> m_Rows;
		private readonly SafeCriticalSection m_RowsSection;
		private readonly string[] m_Columns;

		/// <summary>
		/// Gets the columns.
		/// </summary>
		[PublicAPI]
		public string[] Columns { get { return m_Columns; } }

		/// <summary>
		/// Gets the number of columns.
		/// </summary>
		[PublicAPI]
		public int ColumnsCount { get { return m_Columns.Length; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="columns"></param>
		public TableBuilder(params string[] columns)
		{
			m_Rows = new List<string[]>();
			m_RowsSection = new SafeCriticalSection();
			m_Columns = columns;
		}

		#region Methods

		/// <summary>
		/// Clears all of the rows.
		/// </summary>
		[PublicAPI]
		public void ClearRows()
		{
			m_RowsSection.Execute(() => m_Rows.Clear());
		}

		/// <summary>
		/// Calls ToString() for each item and adds the row to the builder.
		/// </summary>
		/// <param name="row"></param>
		[PublicAPI]
		public void AddRow(params object[] row)
		{
			string[] stringRow = row.Select(o => string.Format("{0}", o))
			                        .ToArray();
			AddRow(stringRow);
		}

		/// <summary>
		/// Adds the row to the builder.
		/// </summary>
		/// <param name="row"></param>
		[PublicAPI]
		public void AddRow(params string[] row)
		{
			if (row != null && row.Length != m_Columns.Length)
				throw new ArgumentException("Row must match columns length.");

			m_RowsSection.Execute(() => m_Rows.Add(row));
		}

		/// <summary>
		/// Adds an empty row to the builder.
		/// </summary>
		[PublicAPI]
		public void AddEmptyRow()
		{
			AddRow(new string[m_Columns.Length]);
		}

		[PublicAPI]
		public void AddSeparator()
		{
			AddRow(null);
		}

		[PublicAPI]
		public void AddHeader(params string[] row)
		{
			if (row.Length != m_Columns.Length)
				throw new ArgumentException("Row must match columns length.");

			AddSeparator();
			AddRow(row);
			AddSeparator();
		}

		/// <summary>
		/// Gets the output string.
		/// </summary>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			m_RowsSection.Enter();

			try
			{
				int[] columnWidths = GetColumnWidths();

				AppendRow(sb, m_Columns, columnWidths);
				AppendSeparator(sb, columnWidths);

				foreach (string[] row in m_Rows)
				{
					if (row == null)
						AppendSeparator(sb, columnWidths);
					else
						AppendRow(sb, row, columnWidths);
				}

				AppendSeparator(sb, columnWidths);
			}
			finally
			{
				m_RowsSection.Leave();
			}

			return sb.ToString();
		}

		#endregion

		#region Private Methods

		private int[] GetColumnWidths()
		{
			int[] columnWidths = new int[m_Columns.Length];
			for (int index = 0; index < m_Columns.Length; index++)
				columnWidths[index] = GetColumnWidth(index);

			return columnWidths;
		}

		private int GetColumnWidth(int index)
		{
			int titleLength = m_Columns[index].Length + 1;
			if (m_Rows.Count == 0)
				return titleLength;

			int maxColumnWidth = m_Rows.Except((string[])null)
			                           .Max(x => x[index] != null ? x[index].Length : 0) + 1;

			return (titleLength > maxColumnWidth) ? titleLength : maxColumnWidth;
		}

		private static void AppendRow(StringBuilder builder, IList<string> row, IList<int> columnWidths)
		{
			for (int index = 0; index < row.Count; index++)
			{
				if (index > 0)
					builder.Append(' ');

				string value = row[index] ?? string.Empty;
				builder.Append(value.PadRight(columnWidths[index]));

				if (index < row.Count - 1)
					builder.Append(VERTICAL);
			}

			builder.AppendLine();
		}

		private static void AppendSeparator(StringBuilder sb, ICollection<int> columnWidths)
		{
			int length = columnWidths.Sum() + (columnWidths.Count - 1) * 2;
			string line = new string(HORIZONTAL, length);

			sb.AppendLine(line);
		}

		#endregion
	}
}
