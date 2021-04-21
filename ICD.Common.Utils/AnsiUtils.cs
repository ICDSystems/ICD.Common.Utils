using System;
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
		public const string ANSI_RESET = "\x1b[0m";

		public const string CODE_BLACK = "30";
		public const string CODE_RED = "31";
		public const string CODE_GREEN = "32";
		public const string CODE_YELLOW = "33";
		public const string CODE_BLUE = "34";
		public const string CODE_MAGENTA = "35";
		public const string CODE_CYAN = "36";
		public const string CODE_WHITE = "37";

		public const string CODE_BRIGHT_BLACK = "30;1";
		public const string CODE_BRIGHT_RED = "31;1";
		public const string CODE_BRIGHT_GREEN = "32;1";
		public const string CODE_BRIGHT_YELLOW = "33;1";
		public const string CODE_BRIGHT_BLUE = "34;1";
		public const string CODE_BRIGHT_MAGENTA = "35;1";
		public const string CODE_BRIGHT_CYAN = "36;1";
		public const string CODE_BRIGHT_WHITE = "37;1";

		/// <summary>
		/// Matches ANSI escape codes, e.g. \x1b[31m and \x1b[30;1m
		/// </summary>
		public const string ANSI_REGEX = "(?'match'\x01b\\[(?'code'[\\d;]+)m)";

		/// <summary>
		/// Matches ANSI escape codes to HTML styles.
		/// Color values are taken from PuTTY.
		/// </summary>
		private static readonly Dictionary<string, string> s_PuttyColors =
			new Dictionary<string, string>
			{
				{CODE_BLACK, "#000000"},
				{CODE_RED, "#BB0000"},
				{CODE_GREEN, "#00BB00"},
				{CODE_YELLOW, "#BBBB00"},
				{CODE_BLUE, "#0000BB"},
				{CODE_MAGENTA, "#BB00BB"},
				{CODE_CYAN, "#00BBBB"},
				{CODE_WHITE, "#BBBBBB"},

				{CODE_BRIGHT_BLACK, "#555555"},
				{CODE_BRIGHT_RED, "#FF5555"},
				{CODE_BRIGHT_GREEN, "#55FF55"},
				{CODE_BRIGHT_YELLOW, "#FFFF55"},
				{CODE_BRIGHT_BLUE, "#5555FF"},
				{CODE_BRIGHT_MAGENTA, "#FF55FF"},
				{CODE_BRIGHT_CYAN, "#55FFFF"},
				{CODE_BRIGHT_WHITE, "#FFFFFF"}
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
		/// Removes ANSI control sequences from the string.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static string StripAnsi(string data)
		{
			return Regex.Replace(data, ANSI_REGEX, string.Empty);
		}

		/// <summary>
		/// Prefixes the given data with an ANSI control sequence and suffixes with a reset sequence.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="ansiSequence"></param>
		/// <returns></returns>
		public static string Format(string data, string ansiSequence)
		{
			// % needs to be escaped or weird things happen
			data = string.IsNullOrEmpty(data) ? data : data.Replace("%", "%%");

			return string.Format("{0}{1}{2}", ansiSequence, data, ANSI_RESET);
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

			Regex regex = new Regex(ANSI_REGEX);
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
				? code.Substring(0, code.Length - 2)
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
			if (colors == null)
				throw new ArgumentNullException("colors");

			return GetColor<string>(colors, invertBright);
		}

		/// <summary>
		/// Gets the color value for the code.
		/// </summary>
		/// <param name="colors"></param>
		/// <param name="invertBright"></param>
		/// <returns></returns>
		public T GetColor<T>(IDictionary<string, T> colors, bool invertBright)
		{
			if (colors == null)
				throw new ArgumentNullException("colors");

			string code = invertBright ? AnsiUtils.InvertBright(Code) : Code;
			return colors[code];
		}
	}
}