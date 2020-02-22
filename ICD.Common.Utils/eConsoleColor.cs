using System.Collections.Generic;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils
{
	public enum eConsoleColor
	{
		Red,
		Green,
		Yellow,
		Blue,
		Magenta,
		Cyan,
		White,
		YellowOnRed
	}

	public static class ConsoleColorExtensions
	{
		private static readonly Dictionary<eConsoleColor, string> s_ConsoleColorCodes =
			new Dictionary<eConsoleColor, string>
			{
				{eConsoleColor.Red, AnsiUtils.COLOR_RED},
				{eConsoleColor.Green, AnsiUtils.COLOR_GREEN},
				{eConsoleColor.Yellow, AnsiUtils.COLOR_YELLOW},
				{eConsoleColor.Blue, AnsiUtils.COLOR_BLUE},
				{eConsoleColor.Magenta, AnsiUtils.COLOR_MAGENTA},
				{eConsoleColor.Cyan, AnsiUtils.COLOR_CYAN},
				{eConsoleColor.White, AnsiUtils.COLOR_WHITE},
				{eConsoleColor.YellowOnRed, AnsiUtils.COLOR_YELLOW_ON_RED_BACKGROUND}
			};

		public static string FormatAnsi(this eConsoleColor extends, string data)
		{
			string code = s_ConsoleColorCodes.GetDefault(extends, AnsiUtils.CODE_RESET);
			return AnsiUtils.Format(data, code);
		}
	}
}
