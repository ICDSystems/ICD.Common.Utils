using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ICD.Common.Properties;

namespace ICD.Common.Utils.Extensions
{
	/// <summary>
	/// Extension methods for DateTime.
	/// </summary>
	public static class DateTimeExtensions
	{
		/// <summary>
		/// Replacement for missing DateTime.ToShortTimeString() absent from NetStandard.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static string ToShortTimeString(this DateTime extends)
		{
			return extends.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern);
		}

		/// <summary>
		/// Gets a string representation of the DateTime with millisecond precision.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static string ToLongTimeStringWithMilliseconds(this DateTime extends)
		{
			// Todo - Better handle different cultures
			return extends.ToString("HH:mm:ss:fff");
		}

		/// <summary>
		/// Returns the closest DateTime to the target time that is greater than the target time
		/// </summary>
		/// <param name="target"></param>
		/// <param name="inclusive">Whether or not to include times equal to the target time</param>
		/// <param name="times"></param>
		/// <returns></returns>
		public static DateTime? NextEarliestTime(this DateTime target, bool inclusive, [NotNull] params DateTime[] times)
		{
			if (times == null)
				throw new ArgumentNullException("times");

			return target.NextEarliestTime(inclusive, (IEnumerable<DateTime>)times);
		}

		/// <summary>
		/// Returns the closest DateTime to the target time that is greater than the target time
		/// </summary>
		/// <param name="target"></param>
		/// <param name="inclusive">Whether or not to include times equal to the target time</param>
		/// <param name="times"></param>
		/// <returns></returns>
		public static DateTime? NextEarliestTime(this DateTime target, bool inclusive, [NotNull] IEnumerable<DateTime> times)
		{
			if (times == null)
				throw new ArgumentNullException("times");

			DateTime earliestTime;
			bool success = times.OrderBy(dt => dt).TryFirst(dt => inclusive ? target <= dt : target < dt, out earliestTime);
			return success ? earliestTime : (DateTime?)null;
		}

		/// <summary>
		/// Returns the closest DateTime to the target time that is less than the target time
		/// </summary>
		/// <param name="target"></param>
		/// <param name="inclusive">Whether or not to include times equal to the target time</param>
		/// <param name="times"></param>
		/// <returns></returns>
		public static DateTime? PreviousLatestTime(this DateTime target, bool inclusive, [NotNull] params DateTime[] times)
		{
			if (times == null)
				throw new ArgumentNullException("times");

			return target.PreviousLatestTime(inclusive, (IEnumerable<DateTime>)times);
		}

		/// <summary>
		/// Returns the closest DateTime to the target time that is less than the target time
		/// </summary>
		/// <param name="target"></param>
		/// <param name="inclusive">Whether or not to include times equal to the target time</param>
		/// <param name="times"></param>
		/// <returns></returns>
		public static DateTime? PreviousLatestTime(this DateTime target, bool inclusive, [NotNull] IEnumerable<DateTime> times)
		{
			if (times == null)
				throw new ArgumentNullException("times");

			DateTime latestTime;
			bool success = times.OrderByDescending(dt => dt).TryFirst(dt => inclusive ? target >= dt : target > dt, out latestTime);
			return success ? latestTime : (DateTime?)null;
		}

		/// <summary>
		/// Returns the DateTime representing the very start of the current day.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static DateTime StartOfDay(this DateTime extends)
		{
			return new DateTime(extends.Year, extends.Month, extends.Day, 0, 0, 0);
		}

		/// <summary>
		/// Returns the DateTime representing the very end of the current day.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static DateTime EndOfDay(this DateTime extends)
		{
			return extends.StartOfDay() + new TimeSpan(24, 0, 0);
		}

		/// <summary>
		/// Returns the given date in unix timestamp format.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static double ToUnixTimestamp(this DateTime extends)
		{
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			TimeSpan diff = extends.ToUniversalTime() - origin;
			return Math.Floor(diff.TotalSeconds);
		}

		/// <summary>
		/// Adds the given number of hours to the time, wrapping every 24 hours without modifying the day.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="hours"></param>
		/// <returns></returns>
		public static DateTime AddYearsAndWrap(this DateTime extends, int years)
		{
			years += extends.Year;

			// need to check days in month due to leap year
			int daysInMonth = DateTime.DaysInMonth(years, extends.Month);
			int day = daysInMonth < extends.Day ? daysInMonth : extends.Day;
			return new DateTime(years, extends.Month, day, extends.Hour, extends.Minute, extends.Second, extends.Millisecond);
		}

		/// <summary>
		/// Adds the given number of hours to the time, wrapping every 24 hours without modifying the day.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="months"></param>
		/// <returns></returns>
		public static DateTime AddMonthsAndWrap(this DateTime extends, int months)
		{
			months = MathUtils.Modulus(months + extends.Month - 1, 12) + 1;
			int daysInMonth = DateTime.DaysInMonth(extends.Year, months);
			int day = daysInMonth < extends.Day ? daysInMonth : extends.Day;

			return new DateTime(extends.Year, months, day, extends.Hour, extends.Minute, extends.Second, extends.Millisecond);
		}

		/// <summary>
		/// Adds the given number of hours to the time, wrapping every 24 hours without modifying the day.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="days"></param>
		/// <returns></returns>
		public static DateTime AddDaysAndWrap(this DateTime extends, int days)
		{
			days = MathUtils.Modulus(days + extends.Day - 1, DateTime.DaysInMonth(extends.Year, extends.Month)) + 1;
			return new DateTime(extends.Year, extends.Month, days, extends.Hour, extends.Minute, extends.Second, extends.Millisecond);
		}

		/// <summary>
		/// Adds the given number of hours to the time, wrapping every 24 hours without modifying the day.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="hours"></param>
		/// <returns></returns>
		public static DateTime AddHoursAndWrap(this DateTime extends, int hours)
		{
			hours = MathUtils.Modulus(hours + extends.Hour, 24);
			return new DateTime(extends.Year, extends.Month, extends.Day, hours, extends.Minute, extends.Second, extends.Millisecond);
		}

		/// <summary>
		/// Adds the given number of hours to the time, wrapping within the current 12 hour span, without modifying the day.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="hours"></param>
		/// <returns></returns>
		public static DateTime AddHoursAndWrap12Hour(this DateTime extends, int hours)
		{
			int currentHour = extends.Hour;
			bool am = extends.Hour < 12;

			int current12Hour = MathUtils.Modulus(currentHour, 12);
			int new12Hour = MathUtils.Modulus(current12Hour + hours, 12);

			return am
				? new DateTime(extends.Year, extends.Month, extends.Day, new12Hour, extends.Minute, extends.Second, extends.Millisecond)
				: new DateTime(extends.Year, extends.Month, extends.Day, new12Hour + 12, extends.Minute, extends.Second, extends.Millisecond);
		}

		/// <summary>
		/// Adds the given number of minutes to the time, wrapping every 60 minutes without modifying the hour.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="minutes"></param>
		/// <returns></returns>
		public static DateTime AddMinutesAndWrap(this DateTime extends, int minutes)
		{
			minutes = MathUtils.Modulus(minutes + extends.Minute, 60);
			return new DateTime(extends.Year, extends.Month, extends.Day, extends.Hour, minutes, extends.Second, extends.Millisecond);
		}
	
	}
}
