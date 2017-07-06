using System;

namespace ICD.Common.Utils.Extensions
{
	/// <summary>
	/// Extension methods for DateTime.
	/// </summary>
	public static class DateTimeExtensions
	{
		/// <summary>
		/// Gets a string representation of the DateTime with millisecond precision.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static string ToLongTimeStringWithMilliseconds(this DateTime extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			// Todo - Better handle different cultures
			return extends.ToString("HH:mm:ss:fff");
		}
	}
}
