using System;
using System.Collections.Generic;

namespace ICD.Common.Utils.Converters
{
	public static class AnsiToHtml
	{
		/// <summary>
		/// Matches ANSI escape codes, e.g. \x1b[31m and \x1b[30;1m
		/// </summary>
		private const string ANSI_PATTERN = "(?'match'\x01b\\[(?'code'[\\d;]+)m)";

		private const string CODE_RESET = "0";

		/// <summary>
		/// Matches ANSI escape codes to HTML styles.
		/// Color values are taken from PuTTY.
		/// </summary>
		private static readonly Dictionary<string, string> s_Colors =
			new Dictionary<string, string>
			{
				{"30", "color:#000000"}, // Black
				{"31", "color:#BB0000"}, // Red
				{"32", "color:#00BB00"}, // Green
				{"33", "color:#BBBB00"}, // Yellow
				{"34", "color:#0000BB"}, // Blue
				{"35", "color:#BB00BB"}, // Magenta
				{"36", "color:#00BBBB"}, // Cyan
				{"37", "color:#BBBBBB"}, // White

				{"30;1", "color:#555555"}, // Bright Black
				{"31;1", "color:#FF5555"}, // Bright Red
				{"32;1", "color:#55FF55"}, // Bright Green
				{"33;1", "color:#FFFF55"}, // Bright Yellow
				{"34;1", "color:#5555FF"}, // Bright Blue
				{"35;1", "color:#FF55FF"}, // Bright Magenta
				{"36;1", "color:#55FFFF"}, // Bright Cyan
				{"37;1", "color:#FFFFFF"}, // Bright White
			};

		/// <summary>
		/// Converts the input ansi string into html with color attributes.
		/// </summary>
		/// <param name="ansi"></param>
		/// <returns></returns>
		public static string Convert(string ansi)
		{
			int depth = 0;

			// Hack - Append a reset to close any open spans
			ansi += "\x1b[0m";

			return RegexUtils.ReplaceGroup(ansi, ANSI_PATTERN, "match", match =>
			{
				string code = match.Groups["code"].Value;

				// Reset code - close all of the open spans.
				if (code == CODE_RESET)
				{
					string output = StringUtils.Repeat("</span>", depth);
					depth = 0;
					return output;
				}

				string style;
				if (!s_Colors.TryGetValue(code, out style))
					return match.Value;

				depth++;
				return string.Format("<span style=\"{0}\">", style);
			});
		}
	}
}