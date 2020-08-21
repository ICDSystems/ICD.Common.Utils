using System;

namespace ICD.Common.Utils.TimeZoneInfo
{
	public sealed class IcdTimeZoneInfoTransitionTime
	{
		public bool IsFixedDateRule { get; set; }

		public DateTime TimeOfDay { get; set; }

		public DayOfWeek DayOfWeek { get; set; }

		public int Day { get; set; }

		public int Week { get; set; }

		public int Month { get; set; }
	}
}
