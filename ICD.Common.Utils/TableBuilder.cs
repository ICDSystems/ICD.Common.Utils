using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils
{
	/// <summary>
	/// TableBuilder provides a way to format a collection of strings to a table.
	/// </summary>
	public sealed class TableBuilder
	{
#if SIMPLSHARP
		private const char HORIZONTAL = '-';
		private const char VERTICAL = '|';
		private const char INTERSECT = '+';
#else
		private const char INTERSECT = '\u253C';

		private const char HORIZONTAL = '\u2500';
		private const char VERTICAL = '\u2502';

		private const char HORIZONTAL_DOWN = '\u252C';
		private const char HORIZONTAL_UP = '\u2534';

		private const char VERTICAL_RIGHT = '\u251C';
		private const char VERTICAL_LEFT = '\u2524';

		private const char DOWN_RIGHT = '\u250C';
		private const char DOWN_LEFT = '\u2510';

		private const char UP_RIGHT = '\u2514';
		private const char UP_LEFT = '\u2518';
#endif

		private readonly List<string[]> m_Rows;
		private readonly string[] m_Columns;

		#region Properties

		/// <summary>
		/// Gets the columns.
		/// </summary>
		[PublicAPI]
		public string[] Columns { get { return m_Columns.ToArray(); } }

		/// <summary>
		/// Gets the number of columns.
		/// </summary>
		[PublicAPI]
		public int ColumnsCount { get { return m_Columns.Length; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="columns"></param>
		public TableBuilder(params string[] columns)
		{
			m_Rows = new List<string[]>();
			m_Columns = columns;
		}

		#region Methods

		/// <summary>
		/// Clears all of the rows.
		/// </summary>
		[PublicAPI]
		public TableBuilder ClearRows()
		{
			m_Rows.Clear();
			return this;
		}

		/// <summary>
		/// Calls ToString() for each item and adds the row to the builder.
		/// </summary>
		/// <param name="row"></param>
		[PublicAPI]
		public TableBuilder AddRow(params object[] row)
		{
			string[] stringRow = row.Select(o => string.Format("{0}", o))
			                        .ToArray();
			return AddRow(stringRow);
		}

		/// <summary>
		/// Adds the row to the builder.
		/// </summary>
		/// <param name="row"></param>
		[PublicAPI]
		public TableBuilder AddRow(params string[] row)
		{
			// Special empty row case
			if (row == null)
			{
				m_Rows.Add(null);
				return this;
			}

			if (row.Length != m_Columns.Length)
				throw new ArgumentException("Row must match columns length.");

			// Split new-lines into multiple rows
			List<string[]> rows = new List<string[]>();

			for (int column = 0; column < row.Length; column++)
			{
				string cell = row[column];
				string[] lines = Regex.Split(cell, "\r\n|\r|\n");

				for (int line = 0; line < lines.Length; line++)
				{
					// Add a new row for this line
					if (line >= rows.Count)
						rows.Add(new string[row.Length]);

					// Insert the line into the row
					rows[line][column] = lines[line];
				}
			}

			// Add all of the rows into the table
			m_Rows.AddRange(rows);

			return this;
		}

		/// <summary>
		/// Adds an empty row to the builder.
		/// </summary>
		[PublicAPI]
		public TableBuilder AddEmptyRow()
		{
			return AddRow(new string[m_Columns.Length]);
		}

		/// <summary>
		/// Adds a horizontal line to the builder.
		/// </summary>
		/// <returns></returns>
		[PublicAPI]
		public TableBuilder AddSeparator()
		{
			return AddRow(null);
		}

		/// <summary>
		/// Adds a new header to the builder.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		[PublicAPI]
		public TableBuilder AddHeader(params string[] row)
		{
			if (row.Length != m_Columns.Length)
				throw new ArgumentException("Row must match columns length.");

			AddSeparator();
			AddRow(row);
			AddSeparator();

			return this;
		}

		/// <summary>
		/// Gets the output string.
		/// </summary>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			{
				int[] columnWidths = GetColumnWidths();

				AppendTopSeparator(sb, columnWidths);

				AppendRow(sb, m_Columns, columnWidths);
				AppendSeparator(sb, columnWidths);

				foreach (string[] row in m_Rows)
				{
					if (row == null)
						AppendSeparator(sb, columnWidths);
					else
						AppendRow(sb, row, columnWidths);
				}

				AppendBottomSeparator(sb, columnWidths);
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
			int titleLength = m_Columns[index].GetPrintableLength() + 1;
			if (m_Rows.Count == 0)
				return titleLength;

			int maxColumnWidth = m_Rows.Except((string[])null)
			                           .Max(x => x[index] != null ? x[index].GetPrintableLength() : 0) + 1;

			return titleLength > maxColumnWidth ? titleLength : maxColumnWidth;
		}

		private void AppendTopSeparator(StringBuilder builder, IList<int> columnWidths)
		{
#if SIMPLSHARP
			// Can't do fancy tables so don't bother drawing the top row
			return;
#else
			builder.Append(DOWN_RIGHT).Append(HORIZONTAL);

			for (int index = 0; index < columnWidths.Count; index++)
			{
				int length = columnWidths[index];

				// Subsequent columns have padding
				if (index > 0)
					length++;

				builder.Append(new string(HORIZONTAL, length));
				if (index < columnWidths.Count - 1)
					builder.Append(HORIZONTAL_DOWN);
			}

			builder.Append(DOWN_LEFT);

			builder.AppendLine();
#endif
		}

		private void AppendBottomSeparator(StringBuilder builder, IList<int> columnWidths)
		{
#if SIMPLSHARP
			AppendSeparator(builder, columnWidths);
			return;
#else
			builder.Append(UP_RIGHT).Append(HORIZONTAL);

			for (int index = 0; index < columnWidths.Count; index++)
			{
				int length = columnWidths[index];

				// Subsequent columns have padding
				if (index > 0)
					length++;

				builder.Append(new string(HORIZONTAL, length));
				if (index < columnWidths.Count - 1)
					builder.Append(HORIZONTAL_UP);
			}

			builder.Append(UP_LEFT);

			builder.AppendLine();
#endif
		}

		private static void AppendRow(StringBuilder builder, IList<string> row, IList<int> columnWidths)
		{
#if !SIMPLSHARP
			builder.Append(VERTICAL).Append(' ');
#endif

			for (int index = 0; index < row.Count; index++)
			{
				if (index > 0)
					builder.Append(' ');

				string value = row[index] ?? string.Empty;
				builder.Append(value.PadRightPrintable(columnWidths[index]));

				if (index < row.Count - 1)
					builder.Append(VERTICAL);
			}

#if !SIMPLSHARP
			builder.Append(VERTICAL);
#endif

			builder.AppendLine();
		}

		private static void AppendSeparator(StringBuilder builder, IList<int> columnWidths)
		{
#if !SIMPLSHARP
			builder.Append(VERTICAL_RIGHT).Append(HORIZONTAL);
#endif

			for (int index = 0; index < columnWidths.Count; index++)
			{
				int length = columnWidths[index];

				// Subsequent columns have padding
				if (index > 0)
					length++;

				builder.Append(new string(HORIZONTAL, length));
				if (index < columnWidths.Count - 1)
					builder.Append(INTERSECT);
			}

#if !SIMPLSHARP
			builder.Append(VERTICAL_LEFT);
#endif

			builder.AppendLine();
		}

#endregion
	}
}
