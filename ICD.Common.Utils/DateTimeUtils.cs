using System;
using System.Globalization;

namespace ICD.Common.Utils
{
	public static class DateTimeUtils
	{
		/// <summary>
		/// Converts the hour in 24 hour format to 12 hour format (1 through 12).
		/// </summary>
		/// <param name="hour"></param>
		/// <returns></returns>
		public static int To12Hour(int hour)
		{
			return MathUtils.Modulus(hour + 11, 12) + 1;
		}

		/// <summary>
		/// Creates a DateTime from the given number of milliseconds since the epoch (1970-01-01T00:00:00Z)
		/// </summary>
		/// <param name="milliseconds">milliseconds since the epoch</param>
		/// <returns></returns>
		public static DateTime FromEpochMilliseconds(long milliseconds)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milliseconds);
		}

		/// <summary>
		/// Creates a DateTime from the given number of seconds since the epoch (1970-01-01T00:00:00Z)
		/// </summary>
		/// <param name="seconds">seconds since the epoch</param>
		/// <returns></returns>
		public static DateTime FromEpochSeconds(long seconds)
		{
			return FromEpochMilliseconds(seconds * 1000);
		}

		/// <summary>
		/// Returns a DateTime for the given ISO-8601 string.
		/// </summary>
		/// <param name="iso"></param>
		/// <returns></returns>
		public static DateTime FromIso8601(string iso)
		{
			return DateTime.Parse(iso, null, DateTimeStyles.RoundtripKind);
		}
	}
}
