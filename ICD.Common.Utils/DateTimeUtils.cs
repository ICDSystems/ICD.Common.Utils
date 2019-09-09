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
	}
}
