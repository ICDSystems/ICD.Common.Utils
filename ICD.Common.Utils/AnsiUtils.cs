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
	}
}