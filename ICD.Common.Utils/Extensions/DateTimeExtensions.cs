using System;
using System.Globalization;
using System.Linq;

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
		public static DateTime? NextEarliestTime(this DateTime target, bool inclusive, params DateTime[] times)
		{
			if (times == null)
				throw new ArgumentNullException("times");

			DateTime earliestTime;
			bool success = times.OrderBy(dt => dt).TryFirst(dt => inclusive ? target <= dt : target < dt, out earliestTime);
			return success ? earliestTime : (DateTime?) null;
		}

		/// <summary>
		/// Returns the closest DateTime to the target time that is less than the target time
		/// </summary>
		/// <param name="target"></param>
		/// <param name="inclusive">Whether or not to include times equal to the target time</param>
		/// <param name="times"></param>
		/// <returns></returns>
		public static DateTime? PreviousLatestTime(this DateTime target, bool inclusive, params DateTime[] times)
		{
			if (times == null)
				throw new ArgumentNullException("null");

			DateTime latestTime;
			bool success = times.OrderByDescending(dt => dt).TryFirst(dt => inclusive ? target >= dt : target > dt, out latestTime);
			return success ? latestTime : (DateTime?) null;
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
	}
}
