using System;
using System.Collections.Generic;
#if STANDARD
using System.Text;
#endif
using ICD.Common.Properties;

namespace ICD.Common.Utils.Services.Logging
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
		/// <summary>
		/// Raised when an item is logged against the logger service.
		/// </summary>
		[PublicAPI]
		event EventHandler<LogItemEventArgs> OnEntryAdded;

		/// <summary>
		/// Raised when the severity level changes.
		/// </summary>
		[PublicAPI]
		event EventHandler<SeverityEventArgs> OnSeverityLevelChanged;

		/// <summary>
		/// Gets and sets the minimum severity threshold for log items to be logged.
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
		[NotNull]
		IEnumerable<KeyValuePair<int, LogItem>> GetHistory();

		/// <summary>
		/// Writes all enqueued logs.
		/// </summary>
		void Flush();
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
			if (extends == null)
				throw new ArgumentNullException("extends");

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
			if (extends == null)
				throw new ArgumentNullException("extends");

			extends.AddEntry(severity, string.Format(message, args));
		}

		[PublicAPI]
		public static void AddEntry(this ILoggerService extends, eSeverity severity, Exception e, string message)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (e == null)
				throw new ArgumentNullException("e");

#if STANDARD
			if (e is AggregateException)
			{
				extends.AddEntry(severity, e as AggregateException, message);
				return;
			}
#endif
			extends.AddEntry(severity, string.Format("{0}: {1}{2}{3}{2}{4}", e.GetType().Name, message,
			                                         IcdEnvironment.NewLine, e.Message, e.StackTrace));
		}

#if STANDARD
		/// <summary>
		/// Logs an aggregate exception as a formatted list of inner exceptions.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="severity"></param>
		/// <param name="e"></param>
		/// <param name="message"></param>
		private static void AddEntry(this ILoggerService extends, eSeverity severity, AggregateException e, string message)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (e == null)
				throw new ArgumentNullException("e");

			StringBuilder builder = new StringBuilder();

			builder.AppendFormat("{0}: {1}", e.GetType().Name, message);

			builder.Append(IcdEnvironment.NewLine);
			builder.Append('[');

			foreach (Exception inner in e.Flatten().InnerExceptions)
				builder.AppendFormat("{0}\t{1}: {2}", IcdEnvironment.NewLine, inner.GetType().Name, inner.Message);

			builder.Append(IcdEnvironment.NewLine);
			builder.Append(']');

			builder.Append(IcdEnvironment.NewLine);
			builder.Append(e.StackTrace);

			extends.AddEntry(severity, builder.ToString());
		}
#endif

		[PublicAPI]
		public static void AddEntry(this ILoggerService extends, eSeverity severity, Exception e, string message,
		                            params object[] args)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (e == null)
				throw new ArgumentNullException("e");

			extends.AddEntry(severity, e, string.Format(message, args));
		}
	}
}
