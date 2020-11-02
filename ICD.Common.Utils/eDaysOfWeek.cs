using System;

namespace ICD.Common.Utils
{
	[Flags]
	public enum eDaysOfWeek
	{
		None = 0,
		Sunday = 1,
		Monday = 2,
		Tuesday = 4,
		Wednesday = 8,
		Thursday = 16,
		Friday = 32,
		Saturday = 64,
		WeekDays = Monday | Tuesday | Wednesday | Thursday | Friday,
		WeekEnds = Saturday | Sunday
	}

	public static class DaysOfWeekExtensions
	{
		public static eDaysOfWeek ToDaysOfWeek(this DayOfWeek extends)
		{
			switch (extends)
			{
				case DayOfWeek.Sunday:
					return eDaysOfWeek.Sunday;
				case DayOfWeek.Monday:
					return eDaysOfWeek.Monday;
				case DayOfWeek.Tuesday:
					return eDaysOfWeek.Tuesday;
				case DayOfWeek.Wednesday:
					return eDaysOfWeek.Wednesday;
				case DayOfWeek.Thursday:
					return eDaysOfWeek.Thursday;
				case DayOfWeek.Friday:
					return eDaysOfWeek.Friday;
				case DayOfWeek.Saturday:
					return eDaysOfWeek.Saturday;
				default:
					throw new ArgumentOutOfRangeException("extends");
			}
		}
	}
}
