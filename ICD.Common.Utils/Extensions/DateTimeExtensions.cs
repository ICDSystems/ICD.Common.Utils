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
	}
}
