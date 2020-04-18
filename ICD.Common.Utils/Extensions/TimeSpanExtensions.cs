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
			StringBuilder builder = new StringBuilder();

			if (extends.Days == 1)
				builder.AppendFormat("{0} day, ", extends.Days);
			else if (extends.Days > 1)
				builder.AppendFormat("{0} days, ", extends.Days);

			if (extends.Hours == 1)
				builder.AppendFormat("{0} hour, ", extends.Hours);
			else if (extends.Hours > 1)
				builder.AppendFormat("{0} hours, ", extends.Hours);

			if (extends.Minutes == 1)
				builder.AppendFormat("{0} minute, ", extends.Minutes);
			else if (extends.Minutes > 1)
				builder.AppendFormat("{0} minutes, ", extends.Minutes);

			if (extends.Seconds == 1)
				builder.AppendFormat("{0} second, ", extends.Seconds);
			else if (extends.Seconds > 1)
				builder.AppendFormat("{0} seconds, ", extends.Seconds);

			if (extends.Milliseconds > 0)
				builder.AppendFormat("{0} ms", extends.Milliseconds);

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
	}
}
