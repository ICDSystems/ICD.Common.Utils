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
				message = eConsoleColor.Red.FormatAnsi(message);

#if SIMPLSHARP
				ErrorLog.Error(message);
#else
				Console.Error.WriteLine("Error  - {0} - {1}", IcdEnvironment.GetLocalTime(), message);
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
				message = eConsoleColor.Yellow.FormatAnsi(message);

#if SIMPLSHARP
				
				ErrorLog.Warn(message);
#else
				Console.Error.WriteLine("Warn   - {0} - {1}", IcdEnvironment.GetLocalTime(), message);
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
				message = eConsoleColor.Blue.FormatAnsi(message);

#if SIMPLSHARP
				ErrorLog.Notice(message);
#else
				Console.Error.WriteLine("Notice - {0} - {1}", IcdEnvironment.GetLocalTime(), message);
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
				message = eConsoleColor.Green.FormatAnsi(message);

#if SIMPLSHARP
				ErrorLog.Ok(message);
#else
				Console.Error.WriteLine("OK     - {0} - {1}", IcdEnvironment.GetLocalTime(), message);
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
#if !SIMPLSHARP
				message = string.Format("{0}: {1}", ex.GetType().Name, message);
#endif

				message = eConsoleColor.YellowOnRed.FormatAnsi(message);

#if SIMPLSHARP
				ErrorLog.Exception(message, ex);
#else
				Console.Error.WriteLine("Except - {0} - {1}", IcdEnvironment.GetLocalTime(), message);
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
				message = eConsoleColor.Cyan.FormatAnsi(message);

#if SIMPLSHARP
				ErrorLog.Info(message);
#else
				Console.Error.WriteLine("Info   - {0} - {1}", IcdEnvironment.GetLocalTime(), message);
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
