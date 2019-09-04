using System.Globalization;

namespace ICD.Common.Utils.Extensions
{
	public static class CultureInfoExtensions
	{
		/// <summary>
		/// Returns true if the given culture uses a 24 hour time format.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static bool Uses24HourFormat(this CultureInfo extends)
		{
			return extends.DateTimeFormat.ShortTimePattern.Contains("H");
		}
	}
}
