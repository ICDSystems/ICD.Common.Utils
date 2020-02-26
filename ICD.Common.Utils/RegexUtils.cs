using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ICD.Common.Utils
{
	public static class RegexUtils
	{
		/// <summary>
		/// Shim to perform Match and Matches in one call.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="pattern"></param>
		/// <param name="match"></param>
		/// <returns></returns>
		public static bool Matches(string input, string pattern, out Match match)
		{
			match = Regex.Match(input, pattern);
			return match.Success;
		}

		/// <summary>
		/// Shim to perform Match and Matches in one call.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="pattern"></param>
		/// <param name="options"></param>
		/// <param name="match"></param>
		/// <returns></returns>
		public static bool Matches(string input, string pattern, RegexOptions options, out Match match)
		{
			match = Regex.Match(input, pattern, options);
			return match.Success;
		}

		/// <summary>
		/// Uses the pattern to replace the specified group with the provided replacement string.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="pattern"></param>
		/// <param name="groupName"></param>
		/// <param name="replacement"></param>
		/// <returns></returns>
		public static string ReplaceGroup(string input, string pattern, string groupName, string replacement)
		{
			return ReplaceGroup(input, pattern, groupName, replacement, RegexOptions.None);
		}

		/// <summary>
		/// Uses the pattern to replace the specified group with the provided replacement string.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="pattern"></param>
		/// <param name="groupName"></param>
		/// <param name="replacement"></param>
		/// <returns></returns>
		public static string ReplaceGroup(string input, string pattern, string groupName, Func<Match, string> replacement)
		{
			return ReplaceGroup(input, pattern, groupName, replacement, RegexOptions.None);
		}

		/// <summary>
		/// Uses the pattern to replace the specified group with the provided replacement string.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="pattern"></param>
		/// <param name="groupName"></param>
		/// <param name="replacement"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static string ReplaceGroup(string input, string pattern, string groupName, string replacement, RegexOptions options)
		{
			return ReplaceGroup(input, pattern, groupName, match => replacement, options);
		}

		/// <summary>
		/// Uses the pattern to replace the specified group with the provided replacement string.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="pattern"></param>
		/// <param name="groupName"></param>
		/// <param name="replacement"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static string ReplaceGroup(string input, string pattern, string groupName, Func<Match, string> replacement, RegexOptions options)
		{
			MatchEvaluator evaluator =
				m =>
				{
					string replacementString = replacement(m);

					Group group = m.Groups[groupName];
					StringBuilder sb = new StringBuilder();

					int previousCaptureEnd = 0;
					foreach (Capture capture in group.Captures.Cast<Capture>())
					{
						int currentCaptureEnd = capture.Index + capture.Length - m.Index;
						int currentCaptureLength = capture.Index - m.Index - previousCaptureEnd;

						sb.Append(m.Value.Substring(previousCaptureEnd, currentCaptureLength));
						sb.Append(replacementString);

						previousCaptureEnd = currentCaptureEnd;
					}
					sb.Append(m.Value.Substring(previousCaptureEnd));

					return sb.ToString();
				};

			return Regex.Replace(input, pattern, evaluator, options);
		}
	}
}
