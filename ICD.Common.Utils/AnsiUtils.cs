using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ICD.Common.Utils
{
	public static class AnsiUtils
	{
		public const string COLOR_RED = "\x1b[31;1m";
		public const string COLOR_GREEN = "\x1b[32;1m";
		public const string COLOR_YELLOW = "\x1b[33;1m";
		public const string COLOR_BLUE = "\x1b[34;1m";
		public const string COLOR_MAGENTA = "\x1b[35;1m";
		public const string COLOR_CYAN = "\x1b[36;1m";
		public const string COLOR_WHITE = "\x1b[37;1m";
		public const string COLOR_YELLOW_ON_RED_BACKGROUND = "\x1b[93;41m";

		public const string CODE_RESET = "\x1b[0m";

		/// <summary>
		/// Matches ANSI escape codes, e.g. \x1b[31m and \x1b[30;1m
		/// </summary>
		private const string ANSI_PATTERN = "(?'match'\x01b\\[(?'code'[\\d;]+)m)";

		/// <summary>
		/// Matches ANSI escape codes to HTML styles.
		/// Color values are taken from PuTTY.
		/// </summary>
		private static readonly Dictionary<string, string> s_PuttyColors =
			new Dictionary<string, string>
			{
				{"30", "000000"}, // Black
				{"31", "BB0000"}, // Red
				{"32", "00BB00"}, // Green
				{"33", "BBBB00"}, // Yellow
				{"34", "0000BB"}, // Blue
				{"35", "BB00BB"}, // Magenta
				{"36", "00BBBB"}, // Cyan
				{"37", "BBBBBB"}, // White

				{"30;1", "555555"}, // Bright Black
				{"31;1", "FF5555"}, // Bright Red
				{"32;1", "55FF55"}, // Bright Green
				{"33;1", "FFFF55"}, // Bright Yellow
				{"34;1", "5555FF"}, // Bright Blue
				{"35;1", "FF55FF"}, // Bright Magenta
				{"36;1", "55FFFF"}, // Bright Cyan
				{"37;1", "FFFFFF"}, // Bright White
			};

		/// <summary>
		/// Gets the color map matching PuTTY.
		/// </summary>
		public static IDictionary<string, string> PuttyColors { get { return s_PuttyColors; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		static AnsiUtils()
		{
#if !SIMPLSHARP
			// Enables ANSI color output in windows/linux console
			Pastel.ConsoleExtensions.Enable();
#endif
		}

		/// <summary>
		/// Prefixes the given data with an ANSI control code and suffixes with a reset code.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="code"></param>
		/// <returns></returns>
		public static string Format(string data, string code)
		{
			// % needs to be escaped or weird things happen
			data = string.IsNullOrEmpty(data) ? data : data.Replace("%", "%%");

			return string.Format("{0}{1}{2}", code, data, CODE_RESET);
		}

		/// <summary>
		/// Splits the given ANSI string into spans.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static IEnumerable<AnsiSpan> ToSpans(string data)
		{
			if (string.IsNullOrEmpty(data))
				yield break;

			Regex regex = new Regex(ANSI_PATTERN);
			Match match = regex.Match(data);

			// No matches
			if (!match.Success)
				yield return new AnsiSpan {Text = data};

			// Find the spans
			while (match.Success)
			{
				// Get the code
				string code = match.Groups["code"].Value;

				// Get the text
				Match next = match.NextMatch();
				int startIndex = match.Index + match.Length;
				int endIndex = next.Success ? next.Index : data.Length;
				string text = data.Substring(startIndex, endIndex - startIndex);

				// Build the span
				if (text.Length > 0)
					yield return new AnsiSpan { Code = code, Text = text };

				// Loop
				match = next;
			}
		}

		/// <summary>
		/// Removes the bright suffix from the code if present, otherwise appends a bright suffix.
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public static string InvertBright(string code)
		{
			return code.EndsWith(";1")
				? code.Substring(0, code.Length - 1)
				: code + ";1";
		}
	}

	public sealed class AnsiSpan
	{
		public string Code { get; set; }
		public string Text { get; set; }

		/// <summary>
		/// Gets the color value for the code.
		/// </summary>
		/// <param name="colors"></param>
		/// <param name="invertBright"></param>
		/// <returns></returns>
		public string GetColor(IDictionary<string, string> colors, bool invertBright)
		{
			string code = invertBright ? AnsiUtils.InvertBright(Code) : Code;
			return colors[code];
		}
	}
}