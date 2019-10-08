using System;
using System.Globalization;
using ICD.Common.Properties;

namespace ICD.Common.Utils.Extensions
{
	public static class CultureInfoExtensions
	{
		/// <summary>
		/// Returns true if the given culture uses a 24 hour time format.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static bool Uses24HourFormat([NotNull]this CultureInfo extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.DateTimeFormat.ShortTimePattern.Contains("H");
		}

		/// <summary>
		/// Updates the time patterns for the given culture to use 12 hour time.
		/// </summary>
		/// <param name="extends"></param>
		public static void ConvertTo12HourCulture([NotNull]this CultureInfo extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (extends.IsReadOnly)
				throw new InvalidOperationException("Culture is readonly");

			DateTimeFormatInfo dateTimeFormat = extends.DateTimeFormat;

			dateTimeFormat.FullDateTimePattern = To12HourPattern(dateTimeFormat.FullDateTimePattern);
			dateTimeFormat.LongTimePattern = To12HourPattern(dateTimeFormat.LongTimePattern);
			dateTimeFormat.ShortTimePattern = To12HourPattern(dateTimeFormat.ShortTimePattern);
		}

		/// <summary>
		/// Updates the time patterns for the given culture to use 24 hour time.
		/// </summary>
		/// <param name="extends"></param>
		public static void ConvertTo24HourCulture([NotNull]this CultureInfo extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (extends.IsReadOnly)
				throw new InvalidOperationException("Culture is readonly");

			DateTimeFormatInfo dateTimeFormat = extends.DateTimeFormat;

			dateTimeFormat.FullDateTimePattern = To24HourPattern(dateTimeFormat.FullDateTimePattern);
			dateTimeFormat.LongTimePattern = To24HourPattern(dateTimeFormat.LongTimePattern);
			dateTimeFormat.ShortTimePattern = To24HourPattern(dateTimeFormat.ShortTimePattern);
		}

		/// <summary>
		/// Converts the given format pattern to use 24 hour time.
		/// </summary>
		/// <param name="pattern"></param>
		/// <returns></returns>
		private static string To24HourPattern(string pattern)
		{
			if (pattern == null)
				return null;

			pattern = pattern.Replace(" tt", "");

			return pattern.Replace("h", "H");
		}

		/// <summary>
		/// Converts the given format pattern to use 12 hour time.
		/// </summary>
		/// <param name="pattern"></param>
		/// <returns></returns>
		private static string To12HourPattern(string pattern)
		{
			if (pattern == null)
				return null;

			if (!pattern.Contains("t"))
				pattern = pattern + " tt";

			return pattern.Replace("H", "h");
		}
	}
}
