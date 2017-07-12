using System;
using ICD.Common.Properties;
#if SIMPLSHARP
using Crestron.SimplSharp;
#endif

namespace ICD.Common.Utils
{
	public static class IcdErrorLog
	{
		private const string CONSOLE_RED = "\x1B[31;1m";
		private const string CONSOLE_GREEN = "\x1B[32;1m";
		private const string CONSOLE_YELLOW = "\x1B[33;1m";
		private const string CONSOLE_BLUE = "\x1B[34;1m";
		//private const string CONSOLE_MAGENTA = "\x1B[35;1m";
		private const string CONSOLE_CYAN = "\x1B[36;1m";
		//private const string CONSOLE_WHITE = "\x1B[37;1m";
		private const string CONSOLE_YELLOW_ON_RED_BACKGROUND = "\x1B[93;41m";
		private const string CONSOLE_RESET = "\x1B[0m";

		private static readonly SafeCriticalSection s_LoggingSection;

		/// <summary>
		/// Static constructor.
		/// </summary>
		static IcdErrorLog()
		{
			s_LoggingSection = new SafeCriticalSection();
		}

		[PublicAPI]
		public static void Error(string message)
		{
			s_LoggingSection.Enter();

			try
			{
#if SIMPLSHARP
				message = FormatConsoleColor(message, CONSOLE_RED);
				ErrorLog.Error(message);
#else
			    System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.Error.WriteLine(message);
                System.Console.ResetColor();
#endif
			}
			finally
			{
				s_LoggingSection.Leave();
			}
		}

		[PublicAPI]
		public static void Error(string message, params object[] args)
		{
			Error(string.Format(message, args));
		}

		[PublicAPI]
		public static void Warn(string message)
		{
			s_LoggingSection.Enter();

			try
			{
#if SIMPLSHARP
				message = FormatConsoleColor(message, CONSOLE_YELLOW);
				ErrorLog.Warn(message);
#else
			    System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.Error.WriteLine(message);
			    System.Console.ResetColor();
#endif
			}
			finally
			{
				s_LoggingSection.Leave();
			}
		}

		[PublicAPI]
		public static void Warn(string message, params object[] args)
		{
			Warn(string.Format(message, args));
		}

		[PublicAPI]
		public static void Notice(string message)
		{
			s_LoggingSection.Enter();

			try
			{
#if SIMPLSHARP
				message = FormatConsoleColor(message, CONSOLE_BLUE);
				ErrorLog.Notice(message);
#else
			    System.Console.ForegroundColor = ConsoleColor.Blue;
                System.Console.Error.WriteLine(message);
			    System.Console.ResetColor();
#endif
			}
			finally
			{
				s_LoggingSection.Leave();
			}
		}

		[PublicAPI]
		public static void Notice(string message, params object[] args)
		{
			Notice(string.Format(message, args));
		}

		[PublicAPI]
		public static void Ok(string message)
		{
			s_LoggingSection.Enter();

			try
			{
#if SIMPLSHARP
				message = FormatConsoleColor(message, CONSOLE_GREEN);
				ErrorLog.Ok(message);
#else
			    System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.Error.WriteLine(message);
			    System.Console.ResetColor();
#endif
			}
			finally
			{
				s_LoggingSection.Leave();
			}
		}

		[PublicAPI]
		public static void Ok(string message, params object[] args)
		{
			Ok(string.Format(message, args));
		}

		[PublicAPI]
		public static void Exception(Exception ex, string message)
		{
			s_LoggingSection.Enter();

			try
			{
#if SIMPLSHARP
				message = FormatConsoleColor(message, CONSOLE_YELLOW_ON_RED_BACKGROUND);
				ErrorLog.Exception(message, ex);
#else
			    System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.BackgroundColor = ConsoleColor.Red;
               System.Console.Error.WriteLine("{0}: {1}", ex.GetType().Name, message);
			    System.Console.ResetColor();
                System.Console.Error.WriteLine(ex.StackTrace);
#endif
			}
			finally
			{
				s_LoggingSection.Leave();
			}
		}

		[PublicAPI]
		public static void Exception(Exception ex, string message, params object[] args)
		{
			message = string.Format(message, args);
			Exception(ex, message);
		}

		[PublicAPI]
		public static void Info(string message)
		{
			s_LoggingSection.Enter();

			try
			{
#if SIMPLSHARP
				message = FormatConsoleColor(message, CONSOLE_CYAN);
				ErrorLog.Info(message);
#else
			    System.Console.ForegroundColor = ConsoleColor.Cyan;
                System.Console.Error.WriteLine(message);
			    System.Console.ResetColor();
#endif
			}
			finally
			{
				s_LoggingSection.Leave();
			}
		}

		[PublicAPI]
		public static void Info(string message, params object[] args)
		{
			Info(string.Format(message, args));
		}

		/// <summary>
		/// Formats the text with the given console color.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="color"></param>
		/// <returns></returns>
		private static string FormatConsoleColor(string text, string color)
		{
			return string.Format("{0}{1}{2}", color, text, CONSOLE_RESET);
		}
	}
}
