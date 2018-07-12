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
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static DateTime? NextEarliestTime(this DateTime target, params DateTime[] times)
		{
			if (times.Length == 0)
				return null;

			DateTime[] orderedTimes = times.OrderBy(dt => dt).ToArray();
			var time = orderedTimes.FirstOrDefault(dt => target < dt);
			return time == default(DateTime) ? (DateTime?) null : time;
		}
	}
}
