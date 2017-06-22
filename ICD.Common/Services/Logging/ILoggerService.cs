using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils;

namespace ICD.Common.Services.Logging
{
	public enum eSeverity
	{
		Emergency = 0,
		Alert = 1,
		Critical = 2,
		Error = 3,
		Warning = 4,
		Notice = 5,
		Informational = 6,
		Debug = 7
	}

	public interface ILoggerService
	{
		[PublicAPI]
		event EventHandler<LogItemEventArgs> OnEntryAdded;

		[PublicAPI]
		event EventHandler<SeverityEventArgs> OnSeverityLevelChanged;

		/// <summary>
		/// Gets and sets the severity level.
		/// </summary>
		[PublicAPI]
		eSeverity SeverityLevel { get; set; }

		/// <summary>
		/// Adds the log item.
		/// </summary>
		/// <param name="item">Log entry to add</param>
		[PublicAPI]
		void AddEntry(LogItem item);

		/// <summary>
		/// Gets the log history.
		/// </summary>
		/// <returns></returns>
		[PublicAPI]
		KeyValuePair<int, LogItem>[] GetHistory();
	}

	/// <summary>
	/// Extension methods for ILoggerService.
	/// </summary>
	public static class LoggerServiceExtensions
	{
		/// <summary>
		/// Adds the log item with string formatting.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="severity">Severity Code, 0 - 7</param>
		/// <param name="message">Message Text format string</param>
		[PublicAPI]
		public static void AddEntry(this ILoggerService extends, eSeverity severity, string message)
		{
			LogItem item = new LogItem(severity, message);
			extends.AddEntry(item);
		}

		/// <summary>
		/// Adds the log item with string formatting.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="severity">Severity Code, 0 - 7</param>
		/// <param name="message">Message Text format string</param>
		/// <param name="args">objects to format into the string</param>
		[PublicAPI]
		public static void AddEntry(this ILoggerService extends, eSeverity severity, string message, params object[] args)
		{
			extends.AddEntry(severity, string.Format(message, args));
		}

		[PublicAPI]
		public static void AddEntry(this ILoggerService extends, eSeverity severity, Exception e, string message)
		{
			extends.AddEntry(severity, string.Format("{0}: {1}{2}{3}{2}{4}", e.GetType().Name, message,
			                                         IcdEnvironment.NewLine, e.Message, e.StackTrace));
		}

		[PublicAPI]
		public static void AddEntry(this ILoggerService extends, eSeverity severity, Exception e, string message,
		                            params object[] args)
		{
			extends.AddEntry(severity, e, string.Format(message, args));
		}
	}
}
