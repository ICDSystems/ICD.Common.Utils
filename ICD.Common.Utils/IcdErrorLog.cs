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
				message = eConsoleColor.Red.FormatAnsi(message);
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
				message = eConsoleColor.Yellow.FormatAnsi(message);
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
				message = eConsoleColor.Blue.FormatAnsi(message);
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
				message = eConsoleColor.Green.FormatAnsi(message);
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
				message = eConsoleColor.YellowOnRed.FormatAnsi(message);
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
				message = eConsoleColor.Cyan.FormatAnsi(message);
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
	}
}
