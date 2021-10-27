using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
#else
using System.IO;
#endif
using ICD.Common.Utils.IO;
using ICD.Common.Utils.Sqlite;

namespace ICD.Common.Utils.TimeZoneInfo
{
	public sealed class IcdTimeZoneInfo
	{
		#region Private Members

		private const string SQL_LOCAL_DATABASE_FILE = "TimeZones.sqlite";
		private const string SQL_CONNECTION_STRING_FORMAT =
			"Data Source={0}";

		private static readonly Dictionary<string, IcdTimeZoneInfo> s_Cache;

		private IcdTimeZoneInfoAdjustmentRule[] m_AdjustmentRules;

		#endregion

		#region Properties

		public string Name { get; private set; }

		public string DisplayName { get; private set; }

		public TimeSpan BaseUtcOffset { get; private set; }

		public bool SupportsDaylightSavingTime { get; private set; }

		public IcdTimeZoneInfoAdjustmentRule[] GetAdjustmentRules()
		{
			return m_AdjustmentRules.ToArray();
		}

		#endregion

		#region Constructors

		static IcdTimeZoneInfo()
		{
			s_Cache = new Dictionary<string, IcdTimeZoneInfo>(StringComparer.OrdinalIgnoreCase);

			try
			{
				PopulateCache();
			}
			catch (Exception e)
			{
				IcdErrorLog.Exception(e, "Error populating IcdTimeZoneInfo cache - {0}", e.Message);
			}
		}

		public static IcdTimeZoneInfo FindSystemTimeZoneById(string timeZoneId)
		{
			return s_Cache[timeZoneId];
		}

		public static bool TryFindSystemTimeZoneById(string timeZoneId, out IcdTimeZoneInfo output)
		{
			return s_Cache.TryGetValue(timeZoneId, out output);
		}

		#endregion

		#region Cache Building

		private static void PopulateCache()
		{
			string databasePath = IcdPath.Combine(PathUtils.ProgramPath, SQL_LOCAL_DATABASE_FILE);
			if (!IcdFile.Exists(databasePath))
				throw new FileNotFoundException("Failed to find database at path " + databasePath);

			string sqlConnectionString = string.Format(SQL_CONNECTION_STRING_FORMAT, databasePath);

			using (IcdSqliteConnection sQLiteConnection = new IcdSqliteConnection(sqlConnectionString))
			{
				sQLiteConnection.Open();

				foreach (IcdTimeZoneInfo info in ReadTimeZoneInfos(sQLiteConnection))
					s_Cache.Add(info.Name, info);
			}
		}

		private static IEnumerable<IcdTimeZoneInfo> ReadTimeZoneInfos(IcdSqliteConnection sQLiteConnection)
		{
			const string command = "select id, name, displayName, baseOffsetTicks, hasDstRule from timeZones";

			using (IcdSqliteCommand cmd = new IcdSqliteCommand(command, sQLiteConnection))
			{
				using (IcdSqliteDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						int id = reader.GetInt32(0);
						string name = reader.GetString(1);
						string display = reader.GetString(2);
						long offset = reader.GetInt64(3);
						bool hasRule = reader.GetBoolean(4);

						IcdTimeZoneInfoAdjustmentRule[] adjustmentRules = LoadAdjustmentRules(id, sQLiteConnection).ToArray();

						IcdTimeZoneInfo info = new IcdTimeZoneInfo
						{
							Name = name,
							DisplayName = display,
							BaseUtcOffset = TimeSpan.FromTicks(offset),
							SupportsDaylightSavingTime = hasRule,
							m_AdjustmentRules = adjustmentRules
						};

						yield return info;
					}
				}
			}
		}

		private static IEnumerable<IcdTimeZoneInfoAdjustmentRule> LoadAdjustmentRules(int timeZoneId, IcdSqliteConnection sQLiteConnection)
		{
			string command = string.Format("select id, timeZone, deltaTicks, ruleStart, ruleEnd from rules where timeZone = {0}", timeZoneId);

			using (IcdSqliteCommand cmd = new IcdSqliteCommand(command, sQLiteConnection))
			{
				using (IcdSqliteDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						int id = reader.GetInt32(0);
						long delta = reader.GetInt64(2);
						string ruleStart = reader.GetString(3);
						string ruleEnd = reader.GetString(4);
						IcdTimeZoneInfoTransitionTime transitionTimeStart = LoadTransitionTimeStart(id, sQLiteConnection);
						IcdTimeZoneInfoTransitionTime transitionTimeEnd = LoadTransitionTimeEnd(id, sQLiteConnection);

						IcdTimeZoneInfoAdjustmentRule rule = new IcdTimeZoneInfoAdjustmentRule
						{
							DaylightDelta = TimeSpan.FromTicks(delta),
							DateStart = DateTime.Parse(ruleStart),
							DateEnd = DateTime.Parse(ruleEnd),
							DaylightTransitionStart = transitionTimeStart,
							DaylightTransitionEnd = transitionTimeEnd
						};

						yield return rule;
					}
				}
			}
		}

		private static IcdTimeZoneInfoTransitionTime LoadTransitionTimeStart(int ruleId, IcdSqliteConnection sQLiteConnection)
		{
			string command =
				string.Format("select rule, isTransitionStart, isFixedRule, timeOfDay, dayOfWeek, day, week, month " +
				              "from ruleTransitions " +
				              "where rule = {0} and isTransitionStart = true",
				              ruleId);

			using (IcdSqliteCommand cmd = new IcdSqliteCommand(command, sQLiteConnection))
			{
				using (IcdSqliteDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						bool isFixed = reader.GetBoolean(2);
						string timeOfDay = reader.GetString(3);
						int dayOfWeek = reader.GetInt32(4);
						int day = reader.GetInt32(5);
						int week = reader.GetInt32(6);
						int month = reader.GetInt32(7);

						return new IcdTimeZoneInfoTransitionTime
						{
							IsFixedDateRule = isFixed,
							TimeOfDay = DateTime.Parse(timeOfDay),
							DayOfWeek = (DayOfWeek)dayOfWeek,
							Day = day,
							Week = week,
							Month = month
						};
					}
				}
			}

			throw new ArgumentOutOfRangeException("ruleId", "There was no TransitionTime for the specified rule");
		}

		private static IcdTimeZoneInfoTransitionTime LoadTransitionTimeEnd(int ruleId, IcdSqliteConnection sQLiteConnection)
		{
			string command =
				string.Format("select rule, isTransitionStart, isFixedRule, timeOfDay, dayOfWeek, day, week, month " +
				              "from ruleTransitions " +
				              "where rule = {0} and isTransitionStart = false",
				              ruleId);

			using (IcdSqliteCommand cmd = new IcdSqliteCommand(command, sQLiteConnection))
			{
				using (IcdSqliteDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						bool isFixed = reader.GetBoolean(2);
						string timeOfDay = reader.GetString(3);
						int dayOfWeek = reader.GetInt32(4);
						int day = reader.GetInt32(5);
						int week = reader.GetInt32(6);
						int month = reader.GetInt32(7);

						return new IcdTimeZoneInfoTransitionTime
						{
							IsFixedDateRule = isFixed,
							TimeOfDay = DateTime.Parse(timeOfDay),
							DayOfWeek = (DayOfWeek)dayOfWeek,
							Day = day,
							Week = week,
							Month = month
						};
					}
				}
			}

			throw new ArgumentOutOfRangeException("ruleId", "There was no TransitionTime for the specified rule");
		}

		#endregion

		#region Methods

		public DateTime ConvertToUtc(DateTime time)
		{
			return time.Kind == DateTimeKind.Utc ? time : ConvertToUtc(time, this);
		}

		#endregion

		#region Private Methods

		private static DateTime ConvertToUtc(DateTime dateTime, IcdTimeZoneInfo sourceTimeZone)
		{
			if (sourceTimeZone == null)
				throw new ArgumentNullException("sourceTimeZone");

			IcdTimeZoneInfoAdjustmentRule sourceRule = sourceTimeZone.GetAdjustmentRuleForTime(dateTime);
			TimeSpan sourceOffset = sourceTimeZone.BaseUtcOffset;

			if (sourceRule != null)
			{
				if (sourceTimeZone.SupportsDaylightSavingTime)
				{
					DaylightTime sourceDaylightTime = GetDaylightTime(dateTime.Year, sourceRule);

					bool sourceIsDaylightSavings = GetIsDaylightSavings(dateTime, sourceRule, sourceDaylightTime);

					// adjust the sourceOffset according to the Adjustment Rule / Daylight Saving Rule
					sourceOffset += (sourceIsDaylightSavings ? sourceRule.DaylightDelta : TimeSpan.Zero
					                /*FUTURE: sourceRule.StandardDelta*/);
				}
			}

			const DateTimeKind targetKind = DateTimeKind.Utc;

			long utcTicks = dateTime.Ticks - sourceOffset.Ticks;
			return new DateTime(utcTicks, targetKind);
		}

		// assumes dateTime is in the current time zone's time
		private IcdTimeZoneInfoAdjustmentRule GetAdjustmentRuleForTime(DateTime dateTime)
		{
			if (m_AdjustmentRules == null || m_AdjustmentRules.Length == 0)
			{
				return null;
			}

			// Only check the whole-date portion of the dateTime -
			// This is because the AdjustmentRule DateStart & DateEnd are stored as
			// Date-only values {4/2/2006 - 10/28/2006} but actually represent the
			// time span {4/2/2006@00:00:00.00000 - 10/28/2006@23:59:59.99999}
			DateTime date = dateTime.Date;

			return m_AdjustmentRules.FirstOrDefault(t => t.DateStart <= date && t.DateEnd >= date);
		}

		//
		// GetDaylightTime -
		//
		// Helper function that returns a DaylightTime from a year and AdjustmentRule
		//
		private static DaylightTime GetDaylightTime(int year, IcdTimeZoneInfoAdjustmentRule rule)
		{
			TimeSpan delta = rule.DaylightDelta;
			DateTime startTime = TransitionTimeToDateTime(year, rule.DaylightTransitionStart);
			DateTime endTime = TransitionTimeToDateTime(year, rule.DaylightTransitionEnd);
			return new DaylightTime(startTime, endTime, delta);
		}

		//
		// TransitionTimeToDateTime -
		//
		// Helper function that converts a year and TransitionTime into a DateTime
		//
		private static DateTime TransitionTimeToDateTime(int year, IcdTimeZoneInfoTransitionTime transitionTime)
		{
			DateTime value;
			DateTime timeOfDay = transitionTime.TimeOfDay;

			if (transitionTime.IsFixedDateRule)
			{
				// create a DateTime from the passed in year and the properties on the transitionTime

				// if the day is out of range for the month then use the last day of the month
				int day = DateTime.DaysInMonth(year, transitionTime.Month);

				value = new DateTime(year, transitionTime.Month, (day < transitionTime.Day) ? day : transitionTime.Day,
							timeOfDay.Hour, timeOfDay.Minute, timeOfDay.Second, timeOfDay.Millisecond);
			}
			else
			{
				if (transitionTime.Week <= 4)
				{
					//
					// Get the (transitionTime.Week)th Sunday.
					//
					value = new DateTime(year, transitionTime.Month, 1,
							timeOfDay.Hour, timeOfDay.Minute, timeOfDay.Second, timeOfDay.Millisecond);

					int dayOfWeek = (int)value.DayOfWeek;
					int delta = (int)transitionTime.DayOfWeek - dayOfWeek;
					if (delta < 0)
					{
						delta += 7;
					}
					delta += 7 * (transitionTime.Week - 1);

					if (delta > 0)
					{
						value = value.AddDays(delta);
					}
				}
				else
				{
					//
					// If TransitionWeek is greater than 4, we will get the last week.
					//
					Int32 daysInMonth = DateTime.DaysInMonth(year, transitionTime.Month);
					value = new DateTime(year, transitionTime.Month, daysInMonth,
							timeOfDay.Hour, timeOfDay.Minute, timeOfDay.Second, timeOfDay.Millisecond);

					// This is the day of week for the last day of the month.
					int dayOfWeek = (int)value.DayOfWeek;
					int delta = dayOfWeek - (int)transitionTime.DayOfWeek;
					if (delta < 0)
					{
						delta += 7;
					}

					if (delta > 0)
					{
						value = value.AddDays(-delta);
					}
				}
			}
			return value;
		}

		//
		// GetIsDaylightSavings -
		//
		// Helper function that checks if a given dateTime is in Daylight Saving Time (DST)
		// This function assumes the dateTime and AdjustmentRule are both in the same time zone
		//
		static private bool GetIsDaylightSavings(DateTime time, IcdTimeZoneInfoAdjustmentRule rule, DaylightTime daylightTime)
		{
			if (rule == null)
			{
				return false;
			}

			DateTime startTime;
			DateTime endTime;

			if (time.Kind == DateTimeKind.Local)
			{
				// startTime and endTime represent the period from either the start of DST to the end and ***includes*** the
				// potentially overlapped times
				startTime = rule.IsStartDateMarkerForBeginningOfYear() ? new DateTime(daylightTime.Start.Year, 1, 1, 0, 0, 0) : daylightTime.Start + daylightTime.Delta;
				endTime = rule.IsEndDateMarkerForEndOfYear() ? new DateTime(daylightTime.End.Year + 1, 1, 1, 0, 0, 0).AddTicks(-1) : daylightTime.End;
			}
			else
			{
				// startTime and endTime represent the period from either the start of DST to the end and
				// ***does not include*** the potentially overlapped times
				//
				//         -=-=-=-=-=- Pacific Standard Time -=-=-=-=-=-=-
				//    April 2, 2006                            October 29, 2006
				// 2AM            3AM                        1AM              2AM
				// |      +1 hr     |                        |       -1 hr      |
				// | <invalid time> |                        | <ambiguous time> |
				//                  [========== DST ========>)
				//
				//        -=-=-=-=-=- Some Weird Time Zone -=-=-=-=-=-=-
				//    April 2, 2006                          October 29, 2006
				// 1AM              2AM                    2AM              3AM
				// |      -1 hr       |                      |       +1 hr      |
				// | <ambiguous time> |                      |  <invalid time>  |
				//                    [======== DST ========>)
				//
				bool invalidAtStart = rule.DaylightDelta > TimeSpan.Zero;
				startTime = rule.IsStartDateMarkerForBeginningOfYear() ? new DateTime(daylightTime.Start.Year, 1, 1, 0, 0, 0) : daylightTime.Start + (invalidAtStart ? rule.DaylightDelta : TimeSpan.Zero); /* FUTURE: - rule.StandardDelta; */
				endTime = rule.IsEndDateMarkerForEndOfYear() ? new DateTime(daylightTime.End.Year + 1, 1, 1, 0, 0, 0).AddTicks(-1) : daylightTime.End + (invalidAtStart ? -rule.DaylightDelta : TimeSpan.Zero);
			}

			return CheckIsDst(startTime, time, endTime, false);
		}

		private static bool CheckIsDst(DateTime startTime, DateTime time, DateTime endTime, bool ignoreYearAdjustment)
		{
			bool isDst;

			if (!ignoreYearAdjustment)
			{
				int startTimeYear = startTime.Year;
				int endTimeYear = endTime.Year;

				if (startTimeYear != endTimeYear)
				{
					endTime = endTime.AddYears(startTimeYear - endTimeYear);
				}

				int timeYear = time.Year;

				if (startTimeYear != timeYear)
				{
					time = time.AddYears(startTimeYear - timeYear);
				}
			}

			if (startTime > endTime)
			{
				// In southern hemisphere, the daylight saving time starts later in the year, and ends in the beginning of next year.
				// Note, the summer in the southern hemisphere begins late in the year.
				isDst = (time < endTime || time >= startTime);
			}
			else
			{
				// In northern hemisphere, the daylight saving time starts in the middle of the year.
				isDst = (time >= startTime && time < endTime);
			}
			return isDst;
		}

		#endregion
	}
}
