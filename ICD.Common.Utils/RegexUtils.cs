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
	}
}
