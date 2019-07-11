using System;
using ICD.Common.Properties;
#if SIMPLSHARP
using Crestron.SimplSharp;
#endif

namespace ICD.Common.Utils
{
	public static class IcdErrorLog
	{
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
				message = FormatConsoleColor(message, ConsoleColorExtensions.CONSOLE_RED);
				ErrorLog.Error(message);
#else
				Console.Write("Error  - {0} - ", IcdEnvironment.GetLocalTime());
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Error.WriteLine(message);
				Console.ResetColor();
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
				message = FormatConsoleColor(message, ConsoleColorExtensions.CONSOLE_YELLOW);
				ErrorLog.Warn(message);
#else
				Console.Write("Warn   - {0} - ", IcdEnvironment.GetLocalTime());
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.Error.WriteLine(message);
				Console.ResetColor();
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
				message = FormatConsoleColor(message, ConsoleColorExtensions.CONSOLE_BLUE);
				ErrorLog.Notice(message);
#else
				Console.Write("Notice - {0} - ", IcdEnvironment.GetLocalTime());
				Console.ForegroundColor = ConsoleColor.Blue;
				Console.Error.WriteLine(message);
				Console.ResetColor();
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
				message = FormatConsoleColor(message, ConsoleColorExtensions.CONSOLE_GREEN);
				ErrorLog.Ok(message);
#else
				Console.Write("OK     - {0} - ", IcdEnvironment.GetLocalTime());
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Error.WriteLine(message);
				Console.ResetColor();
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
				message = FormatConsoleColor(message, ConsoleColorExtensions.CONSOLE_YELLOW_ON_RED_BACKGROUND);
				ErrorLog.Exception(message, ex);
#else
				Console.Write("Except - {0} - ", IcdEnvironment.GetLocalTime());
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.BackgroundColor = ConsoleColor.Red;
				Console.Error.WriteLine("{0}: {1}", ex.GetType().Name, message);
				Console.ResetColor();
				Console.Error.WriteLine(ex.StackTrace);
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
				message = FormatConsoleColor(message, ConsoleColorExtensions.CONSOLE_CYAN);
				ErrorLog.Info(message);
#else
				Console.Write("Info   - {0} - ", IcdEnvironment.GetLocalTime());
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.Error.WriteLine(message);
				Console.ResetColor();
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
			return string.Format("{0}{1}{2}", color, text, ConsoleColorExtensions.CONSOLE_RESET);
		}
	}
}
