using System;

namespace ICD.Common.Utils.Extensions
{
	public static class DayOfWeekExtensions
	{
		public static bool IsWeekday(this DayOfWeek day)
		{
			return !IsWeekend(day);
		}

		public static bool IsWeekend(this DayOfWeek day)
		{
			return day == DayOfWeek.Saturday || day == DayOfWeek.Sunday;
		}
	}
}