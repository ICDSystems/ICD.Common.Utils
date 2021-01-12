using System;
using System.Collections.Generic;
using ICD.Common.Properties;
#if SIMPLSHARP
using Crestron.SimplSharp;
#endif

namespace ICD.Common.Utils
{
	public static class IcdErrorLog
	{
		private const string ERROR = "Error";
		private const string WARN = "Warn";
		private const string NOTICE = "Notice";
		private const string OK = "OK";
		private const string EXCEPTION = "Except";
		private const string INFO = "Info";

		private static readonly Dictionary<string, eConsoleColor> s_SeverityToColor =
			new Dictionary<string, eConsoleColor>
			{
				{ERROR, eConsoleColor.Red},
				{WARN, eConsoleColor.Yellow},
				{NOTICE, eConsoleColor.Blue},
				{OK, eConsoleColor.Green},
				{EXCEPTION, eConsoleColor.Red},
				{INFO, eConsoleColor.Cyan}
			};

		private static readonly Dictionary<string, Action<string, Exception>> s_LogMethods =
			new Dictionary<string, Action<string, Exception>>
			{
#if SIMPLSHARP
				{ERROR, (m, e) => ErrorLog.Error(m)},
				{WARN, (m, e) => ErrorLog.Warn(m)},
				{NOTICE, (m, e) => ErrorLog.Notice(m)},
				{OK, (m, e) => ErrorLog.Ok(m)},
				{EXCEPTION, ErrorLog.Exception},
				{INFO, (m, e) => ErrorLog.Info(m)}
#else
				{ERROR, (m, e) => Console.Error.WriteLine(m)},
				{WARN, (m, e) => Console.Error.WriteLine(m)},
				{NOTICE, (m, e) => Console.Error.WriteLine(m)},
				{OK, (m, e) => Console.Error.WriteLine(m)},
				{EXCEPTION, (m, e) => Console.Error.WriteLine(m)},
				{INFO, (m, e) => Console.Error.WriteLine(m)}
#endif
			};

		private static readonly SafeCriticalSection s_LoggingSection = new SafeCriticalSection();

		#region Methods

		/// <summary>
		/// Logs the error to the standard error output.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		[PublicAPI]
		public static void Error(string message, params object[] args)
		{        
			Log(ERROR, message, null, args);
		}

		/// <summary>
		/// Logs the warning to the standard error output.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		[PublicAPI]
		public static void Warn(string message, params object[] args)
		{
			Log(WARN, message, null, args);
		}

		/// <summary>
		/// Logs the notice to the standard error output.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		[PublicAPI]
		public static void Notice(string message, params object[] args)
		{
			Log(NOTICE, message, null, args);
		}

		/// <summary>
		/// Logs the message to the standard error output.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		[PublicAPI]
		public static void Ok(string message, params object[] args)
		{
			Log(OK, message, null, args);
		}

		/// <summary>
		/// Logs the exception to the standard error output.
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="message"></param>
		/// <param name="args"></param>
		[PublicAPI]
		public static void Exception(Exception exception, string message, params object[] args)
		{
			Log(EXCEPTION, message, exception, args);
		}

		/// <summary>
		/// Logs the info to the standard error output.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		[PublicAPI]
		public static void Info(string message, params object[] args)
		{
			Log(INFO, message, null, args);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Logs the message to the standard error output.
		/// </summary>
		/// <param name="severity"></param>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		/// <param name="args"></param>
		private static void Log(string severity, string message, Exception exception, params object[] args)
		{
			message = Format(severity, message, exception, args);

			s_LoggingSection.Enter();

			try
			{
				Action<string, Exception> method = s_LogMethods[severity];
				method(message, exception);
			}
			finally
			{
				s_LoggingSection.Leave();
			}
		}

		/// <summary>
		/// Formats the message into something human readable.
		/// </summary>
		/// <param name="severity"></param>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		private static string Format(string severity, string message, Exception exception, params object[] args)
		{
			// Insert the parameters
			if (args.Length > 0)
				message = string.Format(message, args);

			if (IcdEnvironment.Framework == IcdEnvironment.eFramework.Standard)
			{
				// Prepend the exception type, append the stack trace
				if (exception != null)
					message = string.Format("{0}: {1}{2}{3}",
					                        exception.GetType().Name,
					                        message,
					                        IcdEnvironment.NewLine,
					                        exception.StackTrace);

				// Prefix severity and time
				string fixedSeverity = severity.Substring(0, Math.Min(6, severity.Length));
				fixedSeverity = string.Format("{0,-6}", fixedSeverity);
				message = string.Format("{0} - {1} - {2}", fixedSeverity, IcdEnvironment.GetLocalTime(), message);
			
			}
			// Add an extra newline for 4-series to help formatting
			else if(IcdEnvironment.CrestronSeries == IcdEnvironment.eCrestronSeries.FourSeries)
				message += IcdEnvironment.NewLine;
			

			// Color formatting
			return s_SeverityToColor[severity].FormatAnsi(message);
		}

		#endregion
	}
}
