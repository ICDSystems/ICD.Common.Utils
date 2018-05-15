using System;
using System.Text;
using ICD.Common.Properties;

namespace ICD.Common.Utils.Services.Logging
{
	/// <summary>
	/// Log Entry Item
	/// </summary>
	public struct LogItem
	{
		private readonly string m_Message;
		private readonly eSeverity m_Severity;
		private readonly DateTime m_Timestamp;

		#region Properties

		/// <summary>
		/// Gets the log time in UTC.
		/// </summary>
		[PublicAPI]
		public DateTime Timestamp { get { return m_Timestamp; } }

		/// <summary>
		/// Gets the log time in local time.
		/// </summary>
		public DateTime LocalTimestamp { get { return Timestamp.ToLocalTime(); } }

		/// <summary>
		/// Get/Set for severity level.
		/// </summary>
		public eSeverity Severity { get { return m_Severity; } }

		/// <summary>
		/// Get/Set for message string.
		/// </summary>
		public string Message { get { return m_Message; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new LogItem object with the specified values.
		/// </summary>
		/// <param name="severity">Severity Level, between 0 and 7</param>
		/// <param name="message">Error message text</param>
		public LogItem(eSeverity severity, string message)
		{
			m_Severity = severity;
			m_Message = message;
			m_Timestamp = IcdEnvironment.GetLocalTime().ToUniversalTime();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Return the text format to send to Fusion
		/// </summary>
		/// <returns>text format for fusion, including timestamp, severity, and message</returns>
		[PublicAPI]
		public string GetFusionLogText()
		{
			StringBuilder s = new StringBuilder();

			s.Append(Timestamp.ToString("yyyyMMddHHmmss"));
			s.Append("||");
			s.Append((int)Severity);
			s.Append("||");
			s.Append(Message);

			return s.ToString();
		}

		/// <summary>
		/// Implementing default equality.
		/// </summary>
		/// <param name="a1"></param>
		/// <param name="a2"></param>
		/// <returns></returns>
		public static bool operator ==(LogItem a1, LogItem a2)
		{
			return a1.Equals(a2);
		}

		/// <summary>
		/// Implementing default inequality.
		/// </summary>
		/// <param name="a1"></param>
		/// <param name="a2"></param>
		/// <returns></returns>
		public static bool operator !=(LogItem a1, LogItem a2)
		{
			return !(a1 == a2);
		}

		/// <summary>
		/// Returns true if this instance is equal to the given object.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public override bool Equals(object other)
		{
			if (other == null || GetType() != other.GetType())
				return false;

			return GetHashCode() == ((LogItem)other).GetHashCode();
		}

		/// <summary>
		/// Gets the hashcode for this instance.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 23 + (m_Message == null ? 0 : m_Message.GetHashCode());
				hash = hash * 23 + (int)m_Timestamp.Ticks;
				hash = hash * 23 + (int)m_Severity;
				return hash;
			}
		}

		#endregion
	}
}
