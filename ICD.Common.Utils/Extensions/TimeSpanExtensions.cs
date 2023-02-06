using System;
using System.Text;

namespace ICD.Common.Utils.Extensions
{
	public static class TimeSpanExtensions
	{
		/// <summary>
		/// Writes the TimeSpan to a string in the format "A day/s, B hour/s, C minute/s, D second/s, E ms"
		/// Omits any items that are 0.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static string ToReadableString(this TimeSpan extends)
		{
		    return extends.ToReadableString(false);
		}

	    /// <summary>
	    /// Writes the TimeSpan to a string in the format "A day/s, B hour/s, C minute/s, D second/s, E ms"
	    /// Omits any items that are 0.
	    /// Optionally hides the miliseconds
	    /// </summary>
	    /// <param name="extends"></param>
	    /// <param name="hideMilliseconds"></param>
	    /// <returns></returns>
	    public static string ToReadableString(this TimeSpan extends, bool hideMilliseconds)
	    {
	        int zeroComparison = extends.CompareTo(TimeSpan.Zero);

	        if (zeroComparison == 0)
	            return "Zero Time";

            StringBuilder builder = new StringBuilder();

            // Handle negative time spans
	        if (zeroComparison < 0)
	            builder.Append("-");

            // Get absolute value so negatives can be ignored
            TimeSpan timeSpan = extends.Duration();

            if (timeSpan.Days == 1)
                builder.AppendFormat("{0} day, ", timeSpan.Days);
            else if (timeSpan.Days > 1)
                builder.AppendFormat("{0} days, ", timeSpan.Days);

            if (timeSpan.Hours == 1)
                builder.AppendFormat("{0} hour, ", timeSpan.Hours);
            else if (timeSpan.Hours > 1)
                builder.AppendFormat("{0} hours, ", timeSpan.Hours);

            if (timeSpan.Minutes == 1)
                builder.AppendFormat("{0} minute, ", timeSpan.Minutes);
            else if (timeSpan.Minutes > 1)
                builder.AppendFormat("{0} minutes, ", timeSpan.Minutes);

            if (timeSpan.Seconds == 1)
                builder.AppendFormat("{0} second, ", timeSpan.Seconds);
            else if (timeSpan.Seconds > 1)
                builder.AppendFormat("{0} seconds, ", timeSpan.Seconds);
            else if (hideMilliseconds && (long)timeSpan.TotalSeconds == 0)
                builder.AppendFormat("0 seconds");

            if (!hideMilliseconds && timeSpan.Milliseconds > 0)
                builder.AppendFormat("{0} ms", timeSpan.Milliseconds);

            return builder.ToString().TrimEnd(',', ' ');
        }

		/// <summary>
		/// Adds the given number of hours to the time, wrapping every 24 hours without modifying the day.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="hours"></param>
		/// <returns></returns>
		public static TimeSpan AddHoursAndWrap(this TimeSpan extends, int hours)
		{
			hours = MathUtils.Modulus(hours + extends.Hours, 24);
			return new TimeSpan(extends.Days, hours, extends.Minutes, extends.Seconds, extends.Milliseconds);
		}

		/// <summary>
		/// Adds the given number of hours to the time, wrapping within the current 12 hour span, without modifying the day.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="hours"></param>
		/// <returns></returns>
		public static TimeSpan AddHoursAndWrap12Hour(this TimeSpan extends, int hours)
		{
			int currentHour = extends.Hours;
			bool am = extends.Hours < 12;

			int current12Hour = MathUtils.Modulus(currentHour, 12);
			int new12Hour = MathUtils.Modulus(current12Hour + hours, 12);

			return am
				? new TimeSpan(extends.Days, new12Hour, extends.Minutes, extends.Seconds, extends.Milliseconds)
				: new TimeSpan(extends.Days, new12Hour + 12, extends.Minutes, extends.Seconds, extends.Milliseconds);
		}

		/// <summary>
		/// Adds the given number of minutes to the time, wrapping every 60 minutes without modifying the hour.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="minutes"></param>
		/// <returns></returns>
		public static TimeSpan AddMinutesAndWrap(this TimeSpan extends, int minutes)
		{
			minutes = MathUtils.Modulus(minutes + extends.Minutes, 60);
			return new TimeSpan(extends.Days, extends.Hours, minutes, extends.Seconds, extends.Milliseconds);
		}

		/// <summary>
		/// Adjusts the given timespan by the UTC offset.
		/// e.g. EST is UTC-5, a TimeSpan of 9:00:00 would result in a universal TimeSpan of 14:00:00
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static TimeSpan ToUniversalTime(this TimeSpan extends)
		{
			TimeSpan offset = TimeZone.CurrentTimeZone.GetUtcOffset(IcdEnvironment.GetLocalTime());
			return extends - offset;
		}

		/// <summary>
		/// Adjusts the given timespan from the UTC offset.
		/// e.g. EST is UTC-5, a TimeSpan of 9:00:00 would result in a local TimeSpan of 4:00:00
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static TimeSpan ToLocalTime(this TimeSpan extends)
		{
			TimeSpan offset = TimeZone.CurrentTimeZone.GetUtcOffset(IcdEnvironment.GetLocalTime());
			return extends + offset;
		}
	}
}
