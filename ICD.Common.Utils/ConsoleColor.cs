using System;

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
		public const string CONSOLE_RED = "\x1B[31;1m";
		public const string CONSOLE_GREEN = "\x1B[32;1m";
		public const string CONSOLE_YELLOW = "\x1B[33;1m";
		public const string CONSOLE_BLUE = "\x1B[34;1m";
		public const string CONSOLE_MAGENTA = "\x1B[35;1m";
		public const string CONSOLE_CYAN = "\x1B[36;1m";
		public const string CONSOLE_WHITE = "\x1B[37;1m";
		public const string CONSOLE_YELLOW_ON_RED_BACKGROUND = "\x1B[93;41m";
		public const string CONSOLE_RESET = "\x1B[0m";

		public static string FormatAnsi(this eConsoleColor extends, string data)
		{
			return string.Format("{0}{1}{2}", extends.ToAnsiPrefix(), data, CONSOLE_RESET);
		}

		public static string ToAnsiPrefix(this eConsoleColor extends)
		{
			switch (extends)
			{
				case eConsoleColor.Red:
					return CONSOLE_RED;
				case eConsoleColor.Green:
					return CONSOLE_GREEN;
				case eConsoleColor.Yellow:
					return CONSOLE_YELLOW;
				case eConsoleColor.Blue:
					return CONSOLE_BLUE;
				case eConsoleColor.Magenta:
					return CONSOLE_MAGENTA;
				case eConsoleColor.Cyan:
					return CONSOLE_CYAN;
				case eConsoleColor.White:
					return CONSOLE_WHITE;
				case eConsoleColor.YellowOnRed:
					return CONSOLE_YELLOW_ON_RED_BACKGROUND;
				default:
					throw new ArgumentOutOfRangeException("extends");
			}
		}

#if STANDARD

		public static ConsoleColor ToForegroundConsoleColor(this eConsoleColor extends)
		{
			switch (extends)
			{
				case eConsoleColor.Red:
					return ConsoleColor.Red;
				case eConsoleColor.Green:
					return ConsoleColor.Green;
				case eConsoleColor.Yellow:
				case eConsoleColor.YellowOnRed:
					return ConsoleColor.Yellow;
				case eConsoleColor.Blue:
					return ConsoleColor.Blue;
				case eConsoleColor.Magenta:
					return ConsoleColor.Magenta;
				case eConsoleColor.Cyan:
					return ConsoleColor.Cyan;
				case eConsoleColor.White:
					return ConsoleColor.White;
				default:
					throw new ArgumentOutOfRangeException("extends");
			}
		}

		public static ConsoleColor ToBackgroundConsoleColor(this eConsoleColor extends)
		{
			switch (extends)
			{
				case eConsoleColor.Red:
				case eConsoleColor.Green:
				case eConsoleColor.Yellow:
				case eConsoleColor.Blue:
				case eConsoleColor.Magenta:
				case eConsoleColor.Cyan:
				case eConsoleColor.White:
					return ConsoleColor.Black;
				
				case eConsoleColor.YellowOnRed:
					return ConsoleColor.Red;
				
				default:
					throw new ArgumentOutOfRangeException("extends");
			}
		}

#endif
	}
}
