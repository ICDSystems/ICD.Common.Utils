using System;

namespace ICD.Common.Utils.TimeZoneInfo
{
	public sealed class IcdTimeZoneInfoAdjustmentRule
	{
		public DateTime DateStart { get; set; }

		public DateTime DateEnd { get; set; }

		public TimeSpan DaylightDelta { get; set; }

		public IcdTimeZoneInfoTransitionTime DaylightTransitionStart { get; set; }

		public IcdTimeZoneInfoTransitionTime DaylightTransitionEnd { get; set; }

		// ----- SECTION: internal utility methods ----------------*

		//
		// When Windows sets the daylight transition start Jan 1st at 12:00 AM, it means the year starts with the daylight saving on.
		// We have to special case this value and not adjust it when checking if any date is in the daylight saving period.
		//
		public bool IsStartDateMarkerForBeginningOfYear()
		{
			return DaylightTransitionStart.Month == 1 && DaylightTransitionStart.Day == 1 && DaylightTransitionStart.TimeOfDay.Hour == 0 &&
				   DaylightTransitionStart.TimeOfDay.Minute == 0 && DaylightTransitionStart.TimeOfDay.Second == 0 &&
				   DateStart.Year == DateEnd.Year;
		}

		//
		// When Windows sets the daylight transition end Jan 1st at 12:00 AM, it means the year ends with the daylight saving on.
		// We have to special case this value and not adjust it when checking if any date is in the daylight saving period.
		//
		public bool IsEndDateMarkerForEndOfYear()
		{
			return DaylightTransitionEnd.Month == 1 && DaylightTransitionEnd.Day == 1 && DaylightTransitionEnd.TimeOfDay.Hour == 0 &&
				   DaylightTransitionEnd.TimeOfDay.Minute == 0 && DaylightTransitionEnd.TimeOfDay.Second == 0 &&
				   DateStart.Year == DateEnd.Year;
		}
	}
}
