using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ICD.Common.Utils.IO;
using ICD.Common.Utils.Sqlite;

namespace ICD.Common.Utils.Globalization
{
	/// <summary>
	/// Adds missing cultures to the limited selection provided by Crestron.
	/// </summary>
	/// <remarks>
	/// This class is a slight modification of CultureEx provided by Neil Colvin in his SSharpCultureLibrary.
	/// 
	/// ICD Changes include:
	///		- Using ICD wrapper classes (IO, SQLite, Reflection) to support Net Standard for testing
	///		- Removing Neil's custom threading mechanisms (do we *really* want to do threading on Crestron?)
	///		- Moving the database location out of NVRAM and into the program directory.
	/// 
	/// 
	/// Neil Colvin - Actually 1.500.0013 - CultureInfo
	/// http://www.crestronlabs.com/showthread.php?11036
	/// 
	///		It has been very frustrating to live with the limitations of the S#/CF when it comes to internationalization.
	///		Crestron has chosen to include no international CultureInfo's in their build of Windows CE.
	///		This makes writing generic modules that work in a wide variety of international environments very difficult.
	///		To provide a solution to this problem, I have implemented a System.Globolization.CultureInfoEx class that provides
	///		many of the missing pieces.
	/// 
	///		CultureInfoEx derives from CultureInfo, so it has the identical properties and methods, both static and instance,
	///		It also has the one method missing in the CF build, GetCultures (CultureTypes type) so that all available cultures
	///		can be obtained.
	/// 
	///		The important piece behind this is a SQLite database which includes the data for the entire set of CultureInfo's
	///		that are available on Windows 10 (809 of them). This database is slightly more the 1 MB in size.
	/// 
	///		This allows CultureInfoEx to be relatively small (23 KB), yet support every culture.	Since it is derived from
	///		CultureInfo, it can be passed to any method taking a CultureInfo, or an IFormatProvider, as an argument (like
	///		String.Format, for instance).
	/// 
	///		When a specific culture is referenced in the CultureInfoEx constructor or in the GetCulture method, it is created
	///		on the fly from the SQLite database (and cached in the case of GetCulture so that it does not have to be recreated).
	/// 
	///		The SQLite database is read-only, so there is no difficulty with it being located in the control system internal
	///		flash file system (it is currently defined to be located in the \NVRAM\DBs folder). It is totally thread safe as well.
	/// 
	///		If a resident CultureInfo is requested (like "en-US") the built in CultureInfo is used, not the one from the database.
	/// 
	///		...
	/// 
	///		I have posted this library on my download site as SSharpCultureLibrary. The database is in the Databases Folder as a
	///		zip file. It should either be in the same folder on the control system as the library, or if it is to be shared among
	///		multiple slots, it should be in the \NVRAM\DBs folder.
	/// 
	///		It should be noted that this does not 100% replicate System.Globalization.CultureInfo, since sealed classes and built-in
	///		functionality make that impossible.
	/// 
	///		For instance CompareInfo is a real problem because it is a sealed class. Although CultureInfoEx defaults a CompareInfo
	///		in most cases, it is probably not the correct one. Also, any attempt to generate a CompareInfo for any but the "en-US"
	///		culture will fail.
	/// 
	///		Also, many .NET functions implicitly use CultureInfo.CurrentCulture as their culture (like String.Format). To use one
	///		the CultureInfoEx generated CultureInfo's, they must be used explicitly (all .NET functions which use
	///		CultureInfo.CurrentCulture implicitely have a version which takes the CultureInfo explicitely as well).
	/// 
	///		However, this class does provide all of the needed culture specific formatting information needed to display information
	///		correctly for that culture (both numbers and data/time).
	/// 
	/// 
	/// Neil's SSharp/Mono libraries are licensed under MIT:
	/// 
	///		The MIT License (MIT)
	/// 
	///		Copyright (c) 2019 Nivloc Enterprises Ltd
	/// 
	///		Permission is hereby granted, free of charge, to any person obtaining a copy
	///		of this software and associated documentation files (the "Software"), to deal
	///		in the Software without restriction, including without limitation the rights
	///		to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	///		copies of the Software, and to permit persons to whom the Software is
	///		furnished to do so, subject to the following conditions:
	/// 
	///		The above copyright notice and this permission notice shall be included in
	///		all copies or substantial portions of the Software.
	/// 
	///		THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	///		IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	///		FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	///		AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	///		LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	///		OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	///		THE SOFTWARE.
	/// </remarks>
	public sealed class IcdCultureInfo : CultureInfo
	{
		private const string SQL_LOCAL_DATABASE_FILE = "CultureInfo.sqlite";
		private const string SQL_CONNECTION_STRING_FORMAT =
#if SIMPLSHARP
			"Data Source = {0}; Version = 3; ReadOnly = True";
#else
			"Data Source = {0}";
#endif
		private const string SQL_CMD_SELECT_BY_NAME = "select * from cultureinfo where name = @name collate nocase";
		private const string SQL_CMD_SELECT_BY_LCID = "select * from cultureinfo where lcid = @lcid";
		private const string SQL_CMD_SELECT_BY_ID = "select * from cultureinfo where id = @id";
		private const string SQL_CMD_SELECT_NUMBER_FORMAT_BY_ID = "select * from numberformatinfo where id = @id";
		private const string SQL_CMD_SELECT_DATE_TIME_FORMAT_BY_ID = "select * from datetimeformatinfo where id = @id";

		private static readonly Dictionary<string, CultureTypes> s_DictAvailableCulturesByName;
		private static readonly Dictionary<int, CultureTypes> s_DictAvailableCulturesByLcid;
		private static readonly Dictionary<string, string> s_DictSpecificCulture;
		private static readonly string[] s_AvailableCultureNames;
		private static readonly Dictionary<string, CultureInfo> s_DictCacheByName;
		private static readonly SafeCriticalSection s_LockCacheByName;
		private static readonly Dictionary<int, CultureInfo> s_DictCacheByLcid;
		private static readonly SafeCriticalSection s_LockCacheByLcid;
		private static readonly Dictionary<int, NumberFormatInfo> s_DictNumberFormatInfos;
		private static readonly Dictionary<int, DateTimeFormatInfo> s_DictDatetimeFormatInfos;
		private static readonly string s_SqlConnectionString;
		private static readonly bool s_IsDatabasePresent;

		private static CultureInfo s_CurrentCulture;
		private static CultureInfo s_CurrentUiCulture;

		private Calendar m_Calendar;
		private Type m_CalendarType;
		private GregorianCalendarTypes? m_GregorianCalendarType;
		private CompareInfo m_CompareInfo;
		private string m_EnglishName;
		private bool m_IsNeutralCulture;
		private int m_Lcid;
		private string m_Name;
		private string m_NativeName;
		private Calendar[] m_OptionalCalendars;
		private Type[] m_OptionalCalendarTypes;
		private GregorianCalendarTypes?[] m_OptionalGregorianCalendarTypes;
		private CultureInfo m_Parent;
		private int m_ParentId;
		private TextInfo m_TextInfo;
		private string m_ThreeLetterIsoLanguageName;
		private string m_ThreeLetterWindowsLanguageName;
		private string m_TwoLetterIsoLanguageName;
		private NumberFormatInfo m_NumberFormat;
		private int m_NumberFormatId;
		private DateTimeFormatInfo m_DatetimeFormat;
		private int m_DatetimeFormatId;
		private bool m_IsResident;

		#region Properties

		public new static CultureInfo InvariantCulture { get { return CultureInfo.InvariantCulture; } }

		public new static CultureInfo CurrentCulture
		{
			get { return s_CurrentCulture ?? CultureInfo.CurrentCulture; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				s_CurrentCulture = value;
			}
		}

// ReSharper disable InconsistentNaming
		public new static CultureInfo CurrentUICulture
// ReSharper restore InconsistentNaming
		{
			get { return s_CurrentUiCulture ?? CultureInfo.CurrentUICulture; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");
				s_CurrentUiCulture = value;
			}
		}

		public override Calendar Calendar
		{
			get
			{
				if (m_IsResident || m_CalendarType == null)
					return base.Calendar;
				Calendar calendar;
				if ((calendar = m_Calendar) == null)
				{
					calendar =
						m_Calendar =
							m_CalendarType == typeof(GregorianCalendar) && m_GregorianCalendarType.HasValue
								? ReflectionUtils.CreateInstance<Calendar>(m_CalendarType, new object[]
								{
									m_GregorianCalendarType
								})
								: ReflectionUtils.CreateInstance<Calendar>(m_CalendarType);
				}
				return calendar;
			}
		}

		public override CompareInfo CompareInfo { get { return m_CompareInfo ?? base.CompareInfo; } }

		public override DateTimeFormatInfo DateTimeFormat
		{
			get
			{
				if (m_DatetimeFormat == null)
				{
					if (m_IsResident)
						return base.DateTimeFormat;

					ThrowIfNeutralCulture(this);

					DateTimeFormatInfo dateTimeFormatInfo = GetDateTimeFormat(m_DatetimeFormatId);
					if (IsReadOnly)
						dateTimeFormatInfo = DateTimeFormatInfo.ReadOnly(dateTimeFormatInfo);

					m_DatetimeFormat = dateTimeFormatInfo;
				}
				return m_DatetimeFormat;
			}
			set
			{
				if (m_IsResident)
					base.DateTimeFormat = value;

				ThrowIfReadOnly();

				if (value == null)
					throw new ArgumentException("value");

				m_DatetimeFormat = value;
			}
		}

		public override string EnglishName { get { return m_EnglishName; } }

		public override bool IsNeutralCulture { get { return m_IsNeutralCulture; } }

		public override int LCID { get { return m_Lcid; } }

		public override string Name { get { return m_Name; } }

		public override string NativeName { get { return m_NativeName; } }

		public override NumberFormatInfo NumberFormat
		{
			get
			{
				if (m_NumberFormat == null)
				{
					if (m_IsResident)
						return base.NumberFormat;

					ThrowIfNeutralCulture(this);

					NumberFormatInfo numberFormatInfo = GetNumberFormat(m_NumberFormatId);
					if (IsReadOnly)
						numberFormatInfo = NumberFormatInfo.ReadOnly(numberFormatInfo);

					m_NumberFormat = numberFormatInfo;
				}
				return m_NumberFormat;
			}
			set
			{
				if (m_IsResident)
					base.NumberFormat = value;

				ThrowIfReadOnly();

				if (value == null)
					throw new ArgumentException("value");

				m_NumberFormat = value;
			}
		}

		public override Calendar[] OptionalCalendars
		{
			get
			{
				Calendar[] calendars;
				if ((calendars = m_OptionalCalendars) == null)
				{
					calendars =
						(m_OptionalCalendars =
						 m_OptionalCalendarTypes.Select((t, ix) =>
						                              new KeyValuePair
							                              <Type, GregorianCalendarTypes?>(t,
							                                                              m_OptionalGregorianCalendarTypes
								                                                              [ix])).Where(kvp => kvp.Key != null).Select(delegate(KeyValuePair<Type, GregorianCalendarTypes?> kvp)
						                                               {
							                                               if (kvp.Key != typeof(GregorianCalendar) || !kvp.Value.HasValue)
								                                               return ReflectionUtils.CreateInstance<Calendar>(kvp.Key);
							                                               return ReflectionUtils.CreateInstance<Calendar>(kvp.Key, new object[]
							                                               {
								                                               kvp.Value
							                                               });
						                                               }).ToArray());
				}
				return calendars;
			}
		}

		public override CultureInfo Parent
		{
			get
			{
				CultureInfo cultureInfo;
				if ((cultureInfo = m_Parent) == null)
					cultureInfo = m_Parent = new IcdCultureInfo(m_ParentId, 0);
				return cultureInfo;
			}
		}

		public override TextInfo TextInfo { get { return m_TextInfo ?? base.TextInfo; } }

		public override string ThreeLetterISOLanguageName { get { return m_ThreeLetterIsoLanguageName; } }

		public override string ThreeLetterWindowsLanguageName { get { return m_ThreeLetterWindowsLanguageName; } }

		public override string TwoLetterISOLanguageName { get { return m_TwoLetterIsoLanguageName; } }

		#endregion

		#region Constructors

		static IcdCultureInfo()
		{
			s_DictAvailableCulturesByName = new Dictionary<string, CultureTypes>(StringComparer.InvariantCultureIgnoreCase);
			s_DictAvailableCulturesByLcid = new Dictionary<int, CultureTypes>();
			s_DictSpecificCulture = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			s_DictCacheByName = new Dictionary<string, CultureInfo>(StringComparer.InvariantCultureIgnoreCase);
			s_LockCacheByName = new SafeCriticalSection();
			s_DictCacheByLcid = new Dictionary<int, CultureInfo>();
			s_LockCacheByLcid = new SafeCriticalSection();
			s_DictNumberFormatInfos = new Dictionary<int, NumberFormatInfo>();
			s_DictDatetimeFormatInfos = new Dictionary<int, DateTimeFormatInfo>();

			string[] builtinCultures =
#if SIMPLSHARP
			{
				"",
				"af",
				"ar",
				"az",
				"be",
				"bg",
				"ca",
				"cs",
				"da",
				"de",
				"dv",
				"el",
				"en",
				"en-US",
				"es",
				"et",
				"eu",
				"fa",
				"fi",
				"fo",
				"fr",
				"gl",
				"gu",
				"he",
				"hi",
				"hr",
				"hu",
				"hy",
				"id",
				"is",
				"it",
				"ja",
				"ka",
				"kk",
				"kn",
				"ko",
				"kok",
				"ky",
				"lt",
				"lv",
				"mk",
				"mn",
				"mr",
				"ms",
				"nl",
				"no",
				"pa",
				"pl",
				"pt",
				"ro",
				"ru",
				"sa",
				"sk",
				"sl",
				"sq",
				"sr",
				"sv",
				"sw",
				"syr",
				"ta",
				"te",
				"th",
				"tr",
				"tt",
				"uk",
				"ur",
				"uz",
				"vi",
				"zh-Hans",
				"zh-Hant",
				"zh-CHS",
				"zh-CHT"
			};
#else
				CultureInfo.GetCultures(CultureTypes.AllCultures)
				           .Select(c => c.Name)
				           .ToArray();
#endif

			string databasePath = IcdPath.Combine(PathUtils.ProgramPath, SQL_LOCAL_DATABASE_FILE);
			if (!IcdFile.Exists(databasePath))
			{
				s_IsDatabasePresent = false;
				return;
			}

			s_SqlConnectionString = string.Format(SQL_CONNECTION_STRING_FORMAT, databasePath);
			using (IcdSqliteConnection sQLiteConnection = new IcdSqliteConnection(s_SqlConnectionString))
			{
				try
				{
					sQLiteConnection.Open();
				}
				catch (Exception)
				{
					s_IsDatabasePresent = false;
					return;
				}
				s_IsDatabasePresent = true;
				IcdSqliteCommand sQLiteCommand = new IcdSqliteCommand("select count(*) from cultureinfo", sQLiteConnection);
				int num = Convert.ToInt32(sQLiteCommand.ExecuteScalar());
				s_AvailableCultureNames = new string[num + 1];
				IcdSqliteCommand sQLiteCommand2 = new IcdSqliteCommand("select id, name, lcid, isneutralculture from cultureinfo",
				                                                       sQLiteConnection);
				using (IcdSqliteDataReader sQLiteDataReader = sQLiteCommand2.ExecuteReader())
				{
					while (sQLiteDataReader.Read())
					{
						int @int = sQLiteDataReader.GetInt32(0);
						bool boolean = sQLiteDataReader.GetBoolean(3);
						string @string = sQLiteDataReader.GetString(1);
						s_DictAvailableCulturesByName[@string] = boolean ? CultureTypes.NeutralCultures : CultureTypes.SpecificCultures;
						int int2 = sQLiteDataReader.GetInt32(2);
						s_DictAvailableCulturesByLcid[int2] = boolean ? CultureTypes.NeutralCultures : CultureTypes.SpecificCultures;
						s_AvailableCultureNames[@int] = @string;
					}
				}

				sQLiteCommand2 = new IcdSqliteCommand("select id, specificculture from specificcultureinfo", sQLiteConnection);
				using (IcdSqliteDataReader sQLiteDataReader2 = sQLiteCommand2.ExecuteReader())
				{
					while (sQLiteDataReader2.Read())
					{
						int int3 = sQLiteDataReader2.GetInt32(0);
						string string2 = sQLiteDataReader2.GetString(1);
						s_DictSpecificCulture[s_AvailableCultureNames[int3]] = string2;
					}
				}
			}

			string[] array = builtinCultures;
			for (int i = 0; i < array.Length; i++)
			{
				string name = array[i];
				try
				{
					CultureInfo cultureInfo = CultureInfo.GetCultureInfo(name);

					Dictionary<string, CultureTypes> dictAvailableCulturesByName;
					string name2;
					(dictAvailableCulturesByName = s_DictAvailableCulturesByName)[name2 = cultureInfo.Name] =
						dictAvailableCulturesByName[name2] | CultureTypes.InstalledWin32Cultures;

					Dictionary<int, CultureTypes> dictAvailableCulturesByLcid;
					int lCid;
					(dictAvailableCulturesByLcid = s_DictAvailableCulturesByLcid)[lCid = cultureInfo.LCID] =
						dictAvailableCulturesByLcid[lCid] | CultureTypes.InstalledWin32Cultures;
				}
				catch (Exception)
				{
				}
			}
		}

		public IcdCultureInfo(int culture)
			: this(culture, true)
		{
		}

		public IcdCultureInfo(int culture, bool useUserOverride)
			: base(GetResidentNeutralLcid(culture), useUserOverride)
		{
			if (culture < 0)
				throw new ArgumentOutOfRangeException("culture", "must be >= 0");

			CultureTypes cultureTypes;
			if (!s_DictAvailableCulturesByLcid.TryGetValue(culture, out cultureTypes))
				throw new ArgumentException("not supported");

			if (!s_IsDatabasePresent || (cultureTypes & CultureTypes.InstalledWin32Cultures) != 0)
			{
				BuildCultureInfoEx(CultureInfo.GetCultureInfo(culture));
				return;
			}

			CultureInfo ci;
			bool flag;
			s_LockCacheByLcid.Enter();
			{
				flag = s_DictCacheByLcid.TryGetValue(culture, out ci);
			}
			s_LockCacheByLcid.Leave();

			if (flag)
			{
				BuildCultureInfoEx(ci);
				return;
			}

			using (IcdSqliteConnection sQLiteConnection = new IcdSqliteConnection(s_SqlConnectionString))
			{
				sQLiteConnection.Open();
				IcdSqliteCommand sQLiteCommand = new IcdSqliteCommand(SQL_CMD_SELECT_BY_LCID, sQLiteConnection);
				sQLiteCommand.Parameters.AddWithValue("@lcid", culture);
				using (IcdSqliteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
					ProcessReader(sQLiteDataReader, sQLiteConnection);
			}
		}

		public IcdCultureInfo(string name)
			: this(name, true)
		{
		}

		public IcdCultureInfo(string name, bool useUserOverrides)
			: base(GetResidentNeutralName(name), useUserOverrides)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			CultureTypes cultureTypes;
			if (!s_DictAvailableCulturesByName.TryGetValue(name, out cultureTypes))
				throw new ArgumentException("not supported");

			if (!s_IsDatabasePresent || (cultureTypes & CultureTypes.InstalledWin32Cultures) != 0)
			{
				BuildCultureInfoEx(CultureInfo.GetCultureInfo(name));
				return;
			}

			CultureInfo ci;
			bool flag;
			s_LockCacheByName.Enter();
			{
				flag = s_DictCacheByName.TryGetValue(name, out ci);
			}
			s_LockCacheByName.Leave();
			if (flag)
			{
				BuildCultureInfoEx(ci);
				return;
			}
			using (IcdSqliteConnection sQLiteConnection = new IcdSqliteConnection(s_SqlConnectionString))
			{
				sQLiteConnection.Open();
				IcdSqliteCommand sQLiteCommand = new IcdSqliteCommand(SQL_CMD_SELECT_BY_NAME, sQLiteConnection);
				sQLiteCommand.Parameters.AddWithValue("@name", name);
				using (IcdSqliteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
					ProcessReader(sQLiteDataReader, sQLiteConnection);
			}
		}

		private IcdCultureInfo(int id, int clone)
			: base(GetResidentNeutralName(s_AvailableCultureNames[id]))
		{
			if (!s_IsDatabasePresent)
				throw new PlatformNotSupportedException("No database");
			if (IsResident(s_AvailableCultureNames[id]))
			{
				BuildCultureInfoEx(CultureInfo.GetCultureInfo(s_AvailableCultureNames[id]));
				return;
			}
			using (IcdSqliteConnection sQLiteConnection = new IcdSqliteConnection(s_SqlConnectionString))
			{
				sQLiteConnection.Open();
				IcdSqliteCommand sQLiteCommand = new IcdSqliteCommand(SQL_CMD_SELECT_BY_ID, sQLiteConnection);
				sQLiteCommand.Parameters.AddWithValue("@id", id);
				using (IcdSqliteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
					ProcessReader(sQLiteDataReader, sQLiteConnection);
			}
		}

		private IcdCultureInfo(CultureInfo ci)
			: base(GetResidentNeutralName(ci.Name))
		{
			BuildCultureInfoEx(ci);
		}

		public static IcdCultureInfo FromCultureInfo(CultureInfo ci)
		{
			return new IcdCultureInfo(ci);
		}

		#endregion

		private void ProcessReader(IcdSqliteDataReader rdr, IcdSqliteConnection conn)
		{
			if (!rdr.Read())
				throw new InvalidOperationException("failure reading database");

			int ordinal = rdr.GetOrdinal("calendar");
			string text = rdr.GetString(ordinal);
			if (text.EndsWith(")"))
			{
				int num = text.IndexOf('(');
				string value = text.Substring(num + 1, text.Length - num - 2);
				text = text.Substring(0, num);
				m_GregorianCalendarType = (GregorianCalendarTypes)Enum.Parse(typeof(GregorianCalendarTypes), value, true);
			}

			m_CalendarType = Type.GetType("System.Globalization." + text);
			ordinal = rdr.GetOrdinal("englishname");
			m_EnglishName = rdr.GetString(ordinal);
			ordinal = rdr.GetOrdinal("isneutralculture");
			m_IsNeutralCulture = rdr.GetBoolean(ordinal);
			ordinal = rdr.GetOrdinal("lcid");
			m_Lcid = rdr.GetInt32(ordinal);
			ordinal = rdr.GetOrdinal("name");
			m_Name = rdr.GetString(ordinal);
			ordinal = rdr.GetOrdinal("nativename");
			m_NativeName = rdr.GetString(ordinal);
			ordinal = rdr.GetOrdinal("optionalcalendars");
			string[] array = rdr.GetString(ordinal).Split('|');
			m_OptionalGregorianCalendarTypes = new GregorianCalendarTypes?[array.Length];
			m_OptionalCalendarTypes = new Type[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i];
				if (text2.EndsWith(")"))
				{
					int num2 = text2.IndexOf('(');
					string value2 = text2.Substring(num2 + 1, text2.Length - num2 - 2);
					text2 = text2.Substring(0, num2);
					m_OptionalGregorianCalendarTypes[i] =
						(GregorianCalendarTypes)Enum.Parse(typeof(GregorianCalendarTypes), value2, true);
				}
				m_OptionalCalendarTypes[i] = Type.GetType("System.Globalization." + text2);
			}
			ordinal = rdr.GetOrdinal("parent");
			m_ParentId = rdr.GetInt32(ordinal);
			ordinal = rdr.GetOrdinal("threeletterisolanguagename");
			m_ThreeLetterIsoLanguageName = rdr.GetString(ordinal);
			ordinal = rdr.GetOrdinal("threeletterwindowslanguagename");
			m_ThreeLetterWindowsLanguageName = rdr.GetString(ordinal);
			ordinal = rdr.GetOrdinal("twoletterisolanguagename");
			m_TwoLetterIsoLanguageName = rdr.GetString(ordinal);
			ordinal = rdr.GetOrdinal("datetimeformat");
			m_DatetimeFormatId = rdr.GetInt32(ordinal);
			ordinal = rdr.GetOrdinal("numberformat");
			m_NumberFormatId = rdr.GetInt32(ordinal);
			rdr.Close();
		}

		private static NumberFormatInfo GetNumberFormat(int id)
		{
			NumberFormatInfo numberFormat;
			using (IcdSqliteConnection sQLiteConnection = new IcdSqliteConnection(s_SqlConnectionString))
			{
				sQLiteConnection.Open();
				numberFormat = GetNumberFormat(id, sQLiteConnection);
			}
			return numberFormat;
		}

		private static NumberFormatInfo GetNumberFormat(int id, IcdSqliteConnection conn)
		{
			NumberFormatInfo numberFormatInfo;
			if (s_DictNumberFormatInfos.TryGetValue(id, out numberFormatInfo))
				return (NumberFormatInfo)numberFormatInfo.Clone();

			IcdSqliteCommand sQLiteCommand = new IcdSqliteCommand(SQL_CMD_SELECT_NUMBER_FORMAT_BY_ID, conn);
			sQLiteCommand.Parameters.AddWithValue("@id", id);

			using (IcdSqliteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
			{
				if (!sQLiteDataReader.Read())
					throw new InvalidOperationException("invalid number format database");

				int ordinal = sQLiteDataReader.GetOrdinal("CurrencyDecimalDigits");
				int ordinal2 = sQLiteDataReader.GetOrdinal("CurrencyDecimalSeparator");
				int ordinal3 = sQLiteDataReader.GetOrdinal("CurrencyGroupSizes");
				int ordinal4 = sQLiteDataReader.GetOrdinal("NumberGroupSizes");
				int ordinal5 = sQLiteDataReader.GetOrdinal("PercentGroupSizes");
				int ordinal6 = sQLiteDataReader.GetOrdinal("CurrencyGroupSeparator");
				int ordinal7 = sQLiteDataReader.GetOrdinal("CurrencySymbol");
				int ordinal8 = sQLiteDataReader.GetOrdinal("NaNSymbol");
				int ordinal9 = sQLiteDataReader.GetOrdinal("CurrencyNegativePattern");
				int ordinal10 = sQLiteDataReader.GetOrdinal("NumberNegativePattern");
				int ordinal11 = sQLiteDataReader.GetOrdinal("PercentPositivePattern");
				int ordinal12 = sQLiteDataReader.GetOrdinal("PercentNegativePattern");
				int ordinal13 = sQLiteDataReader.GetOrdinal("NegativeInfinitySymbol");
				int ordinal14 = sQLiteDataReader.GetOrdinal("NegativeSign");
				int ordinal15 = sQLiteDataReader.GetOrdinal("NumberDecimalDigits");
				int ordinal16 = sQLiteDataReader.GetOrdinal("NumberDecimalSeparator");
				int ordinal17 = sQLiteDataReader.GetOrdinal("NumberGroupSeparator");
				int ordinal18 = sQLiteDataReader.GetOrdinal("CurrencyPositivePattern");
				int ordinal19 = sQLiteDataReader.GetOrdinal("PositiveInfinitySymbol");
				int ordinal20 = sQLiteDataReader.GetOrdinal("PositiveSign");
				int ordinal21 = sQLiteDataReader.GetOrdinal("PercentDecimalDigits");
				int ordinal22 = sQLiteDataReader.GetOrdinal("PercentDecimalSeparator");
				int ordinal23 = sQLiteDataReader.GetOrdinal("PercentGroupSeparator");
				int ordinal24 = sQLiteDataReader.GetOrdinal("PercentSymbol");
				int ordinal25 = sQLiteDataReader.GetOrdinal("PerMilleSymbol");

				NumberFormatInfo numberFormatInfo2 = new NumberFormatInfo
				{
					CurrencyDecimalDigits = sQLiteDataReader.GetInt32(ordinal),
					CurrencyDecimalSeparator = sQLiteDataReader.GetString(ordinal2),
					CurrencyGroupSizes = sQLiteDataReader.GetString(ordinal3)
					                                     .Split(',')
					                                     .Select(s => int.Parse(s))
					                                     .ToArray(),
					NumberGroupSizes = sQLiteDataReader.GetString(ordinal4)
					                                   .Split(',')
					                                   .Select(s => int.Parse(s))
					                                   .ToArray(),
					PercentGroupSizes = sQLiteDataReader.GetString(ordinal5)
					                                    .Split(',')
					                                    .Select(s => int.Parse(s))
					                                    .ToArray(),
					CurrencyGroupSeparator = sQLiteDataReader.GetString(ordinal6),
					CurrencySymbol = sQLiteDataReader.GetString(ordinal7),
					NaNSymbol = sQLiteDataReader.GetString(ordinal8),
					CurrencyNegativePattern = sQLiteDataReader.GetInt32(ordinal9),
					NumberNegativePattern = sQLiteDataReader.GetInt32(ordinal10),
					PercentPositivePattern = sQLiteDataReader.GetInt32(ordinal11),
					PercentNegativePattern = sQLiteDataReader.GetInt32(ordinal12),
					NegativeInfinitySymbol = sQLiteDataReader.GetString(ordinal13),
					NegativeSign = sQLiteDataReader.GetString(ordinal14),
					NumberDecimalDigits = sQLiteDataReader.GetInt32(ordinal15),
					NumberDecimalSeparator = sQLiteDataReader.GetString(ordinal16),
					NumberGroupSeparator = sQLiteDataReader.GetString(ordinal17),
					CurrencyPositivePattern = sQLiteDataReader.GetInt32(ordinal18),
					PositiveInfinitySymbol = sQLiteDataReader.GetString(ordinal19),
					PositiveSign = sQLiteDataReader.GetString(ordinal20),
					PercentDecimalDigits = sQLiteDataReader.GetInt32(ordinal21),
					PercentDecimalSeparator = sQLiteDataReader.GetString(ordinal22),
					PercentGroupSeparator = sQLiteDataReader.GetString(ordinal23),
					PercentSymbol = sQLiteDataReader.GetString(ordinal24),
					PerMilleSymbol = sQLiteDataReader.GetString(ordinal25)
				};
				numberFormatInfo = numberFormatInfo2;
			}
			s_DictNumberFormatInfos[id] = numberFormatInfo;
			return (NumberFormatInfo)numberFormatInfo.Clone();
		}

		private static DateTimeFormatInfo GetDateTimeFormat(int id)
		{
			DateTimeFormatInfo dateTimeFormat;
			using (IcdSqliteConnection sQLiteConnection = new IcdSqliteConnection(s_SqlConnectionString))
			{
				sQLiteConnection.Open();
				dateTimeFormat = GetDateTimeFormat(id, sQLiteConnection);
			}
			return dateTimeFormat;
		}

		private static DateTimeFormatInfo GetDateTimeFormat(int id, IcdSqliteConnection conn)
		{
			DateTimeFormatInfo dateTimeFormatInfo;
			if (s_DictDatetimeFormatInfos.TryGetValue(id, out dateTimeFormatInfo))
				return (DateTimeFormatInfo)dateTimeFormatInfo.Clone();
			IcdSqliteCommand sQLiteCommand = new IcdSqliteCommand(SQL_CMD_SELECT_DATE_TIME_FORMAT_BY_ID, conn);
			sQLiteCommand.Parameters.AddWithValue("@id", id);
			using (IcdSqliteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader())
			{
				if (!sQLiteDataReader.Read())
					throw new InvalidOperationException("invalid datetime format database");
				int ordinal = sQLiteDataReader.GetOrdinal("AMDesignator");
				sQLiteDataReader.GetOrdinal("Calendar");
				int ordinal2 = sQLiteDataReader.GetOrdinal("DateSeparator");
				int ordinal3 = sQLiteDataReader.GetOrdinal("FirstDayOfWeek");
				int ordinal4 = sQLiteDataReader.GetOrdinal("CalendarWeekRule");
				int ordinal5 = sQLiteDataReader.GetOrdinal("FullDateTimePattern");
				int ordinal6 = sQLiteDataReader.GetOrdinal("LongDatePattern");
				int ordinal7 = sQLiteDataReader.GetOrdinal("LongTimePattern");
				int ordinal8 = sQLiteDataReader.GetOrdinal("MonthDayPattern");
				int ordinal9 = sQLiteDataReader.GetOrdinal("PMDesignator");
				int ordinal10 = sQLiteDataReader.GetOrdinal("ShortDatePattern");
				int ordinal11 = sQLiteDataReader.GetOrdinal("ShortTimePattern");
				int ordinal12 = sQLiteDataReader.GetOrdinal("TimeSeparator");
				int ordinal13 = sQLiteDataReader.GetOrdinal("YearMonthPattern");
				int ordinal14 = sQLiteDataReader.GetOrdinal("AbbreviatedDayNames");
				int ordinal15 = sQLiteDataReader.GetOrdinal("ShortestDayNames");
				int ordinal16 = sQLiteDataReader.GetOrdinal("DayNames");
				int ordinal17 = sQLiteDataReader.GetOrdinal("AbbreviatedMonthNames");
				int ordinal18 = sQLiteDataReader.GetOrdinal("MonthNames");
				int ordinal19 = sQLiteDataReader.GetOrdinal("AbbreviatedMonthGenitiveNames");
				int ordinal20 = sQLiteDataReader.GetOrdinal("MonthGenitiveNames");
				dateTimeFormatInfo = new DateTimeFormatInfo
				{
					AMDesignator = sQLiteDataReader.GetString(ordinal),
					DateSeparator = sQLiteDataReader.GetString(ordinal2),
					FirstDayOfWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), sQLiteDataReader.GetString(ordinal3), true),
					CalendarWeekRule =
						(CalendarWeekRule)Enum.Parse(typeof(CalendarWeekRule), sQLiteDataReader.GetString(ordinal4), true),
					FullDateTimePattern = sQLiteDataReader.GetString(ordinal5),
					LongDatePattern = sQLiteDataReader.GetString(ordinal6),
					LongTimePattern = sQLiteDataReader.GetString(ordinal7),
					MonthDayPattern = sQLiteDataReader.GetString(ordinal8),
					PMDesignator = sQLiteDataReader.GetString(ordinal9),
					ShortDatePattern = sQLiteDataReader.GetString(ordinal10),
					ShortTimePattern = sQLiteDataReader.GetString(ordinal11),
					TimeSeparator = sQLiteDataReader.GetString(ordinal12),
					YearMonthPattern = sQLiteDataReader.GetString(ordinal13),
					AbbreviatedDayNames = sQLiteDataReader.GetString(ordinal14).Split('|'),
					ShortestDayNames = sQLiteDataReader.GetString(ordinal15).Split('|'),
					DayNames = sQLiteDataReader.GetString(ordinal16).Split('|'),
					AbbreviatedMonthNames = sQLiteDataReader.GetString(ordinal17).Split('|'),
					MonthNames = sQLiteDataReader.GetString(ordinal18).Split('|'),
					AbbreviatedMonthGenitiveNames = sQLiteDataReader.GetString(ordinal19).Split('|'),
					MonthGenitiveNames = sQLiteDataReader.GetString(ordinal20).Split('|')
				};
			}
			s_DictDatetimeFormatInfos[id] = dateTimeFormatInfo;
			return (DateTimeFormatInfo)dateTimeFormatInfo.Clone();
		}

		private void BuildCultureInfoEx(CultureInfo ci)
		{
			IcdCultureInfo icdCultureInfo = ci as IcdCultureInfo;
			if (icdCultureInfo == null)
			{
				m_IsResident = true;
				m_Calendar = ci.Calendar;
				m_OptionalCalendars = ci.OptionalCalendars;
				m_Parent = ci.Parent;
				m_TextInfo = ci.TextInfo;
				try
				{
					m_CompareInfo = ci.CompareInfo;
				}
				catch (PlatformNotSupportedException)
				{
					m_CompareInfo = CultureInfo.InvariantCulture.CompareInfo;
				}
			}
			else
			{
				m_Calendar = icdCultureInfo.m_Calendar;
				m_CalendarType = icdCultureInfo.m_CalendarType;
				m_GregorianCalendarType = icdCultureInfo.m_GregorianCalendarType;
				m_OptionalCalendars = icdCultureInfo.m_OptionalCalendars;
				m_OptionalCalendarTypes = icdCultureInfo.m_OptionalCalendarTypes;
				m_OptionalGregorianCalendarTypes = icdCultureInfo.m_OptionalGregorianCalendarTypes;
				m_Parent = icdCultureInfo.m_Parent;
				m_TextInfo = icdCultureInfo.m_TextInfo;
				m_CompareInfo = icdCultureInfo.m_CompareInfo;
			}
			
			m_EnglishName = ci.EnglishName;
			m_IsNeutralCulture = ci.IsNeutralCulture;
			m_Lcid = ci.LCID;
			m_Name = ci.Name;
			m_NativeName = ci.NativeName;
			m_ThreeLetterIsoLanguageName = ci.ThreeLetterISOLanguageName;
			m_ThreeLetterWindowsLanguageName = ci.ThreeLetterWindowsLanguageName;
			m_TwoLetterIsoLanguageName = ci.TwoLetterISOLanguageName;

			if (m_IsNeutralCulture)
				return;

			if (icdCultureInfo == null)
			{
				m_DatetimeFormat = ci.DateTimeFormat.Clone() as DateTimeFormatInfo;
				m_NumberFormat = ci.NumberFormat.Clone() as NumberFormatInfo;
				return;
			}

			m_DatetimeFormatId = icdCultureInfo.m_DatetimeFormatId;
			m_DatetimeFormat = icdCultureInfo.m_DatetimeFormat.Clone() as DateTimeFormatInfo;
			m_NumberFormatId = icdCultureInfo.m_NumberFormatId;
			m_NumberFormat = icdCultureInfo.m_NumberFormat.Clone() as NumberFormatInfo;
		}

		#region Methods

		public new static CultureInfo GetCultureInfo(string name)
		{
			CultureInfo cultureInfo;
			s_LockCacheByName.Enter();
			{
				if (s_DictCacheByName.TryGetValue(name, out cultureInfo))
					return cultureInfo;
			}
			s_LockCacheByName.Leave();
			cultureInfo = new IcdCultureInfo(name);
			if (!cultureInfo.IsNeutralCulture)
				cultureInfo = ReadOnly(cultureInfo);
			s_LockCacheByName.Enter();
			{
				s_DictCacheByName[cultureInfo.Name] = cultureInfo;
				s_DictCacheByLcid[cultureInfo.LCID] = cultureInfo;
			}
			s_LockCacheByName.Leave();
			return cultureInfo;
		}

		public new static CultureInfo GetCultureInfo(int culture)
		{
			CultureInfo cultureInfo;
			s_LockCacheByLcid.Enter();
			{
				if (s_DictCacheByLcid.TryGetValue(culture, out cultureInfo))
					return cultureInfo;
			}
			s_LockCacheByLcid.Leave();
			cultureInfo = new IcdCultureInfo(culture);
			if (!cultureInfo.IsNeutralCulture)
				cultureInfo = ReadOnly(cultureInfo);
			s_LockCacheByLcid.Enter();
			{
				s_DictCacheByName[cultureInfo.Name] = cultureInfo;
				s_DictCacheByLcid[cultureInfo.LCID] = cultureInfo;
			}
			s_LockCacheByLcid.Leave();
			return cultureInfo;
		}

		public static CultureInfo[] GetCultures(CultureTypes types)
		{
			return s_DictAvailableCulturesByName.Where(de => (de.Value & types) != 0)
			                                    .Select(de => new IcdCultureInfo(de.Key))
			                                    .Cast<CultureInfo>()
			                                    .ToArray();
		}

		public new static CultureInfo CreateSpecificCulture(string name)
		{
			IcdCultureInfo icdCultureInfo = new IcdCultureInfo(name);
			if (!icdCultureInfo.IsNeutralCulture)
				return icdCultureInfo;
			if ((icdCultureInfo.LCID & 1023) == 4)
				throw new ArgumentException();
			return new IcdCultureInfo(s_DictSpecificCulture[name]);
		}

		public override object Clone()
		{
			return new IcdCultureInfo(this);
		}

		public override bool Equals(object value)
		{
			CultureInfo cultureInfo = value as CultureInfo;
			return cultureInfo != null && Name.Equals(cultureInfo.Name) && CompareInfo.Equals(cultureInfo.CompareInfo);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode() + CompareInfo.GetHashCode();
		}

		public override string ToString()
		{
			return m_Name;
		}

		public new void ClearCachedData()
		{
			s_LockCacheByName.Enter();
			s_LockCacheByLcid.Enter();
			{
				s_DictCacheByName.Clear();
				s_DictCacheByLcid.Clear();
			}
			s_LockCacheByLcid.Enter();
			s_LockCacheByName.Enter();

			base.ClearCachedData();
		}

		#endregion

		private void ThrowIfReadOnly()
		{
			if (IsReadOnly)
				throw new InvalidOperationException();
		}

		private static void ThrowIfNeutralCulture(CultureInfo culture)
		{
			if (culture.IsNeutralCulture)
				throw new NotSupportedException("A neutral culture does not provide enough information to display the correct numeric format");
		}

		private static bool IsResident(string name)
		{
			CultureTypes cultureTypes;
			return s_DictAvailableCulturesByName.TryGetValue(name, out cultureTypes) &&
			       (cultureTypes & CultureTypes.InstalledWin32Cultures) != 0;
		}

		private static bool IsResidentAndNeutral(string name)
		{
			CultureTypes cultureTypes;
			return s_DictAvailableCulturesByName.TryGetValue(name, out cultureTypes) &&
			       (cultureTypes & (CultureTypes.InstalledWin32Cultures | CultureTypes.NeutralCultures)) ==
			       (CultureTypes.InstalledWin32Cultures | CultureTypes.NeutralCultures);
		}

		private static string GetResidentNeutralName(string name)
		{
			if (IsResident(name))
				return name;
			if (name.Length <= 2 || !IsResidentAndNeutral(name.Substring(0, 2)))
				return string.Empty;
			return name.Substring(0, 2);
		}

		private static bool IsResident(int culture)
		{
			CultureTypes cultureTypes;
			return s_DictAvailableCulturesByLcid.TryGetValue(culture, out cultureTypes) &&
			       (cultureTypes & CultureTypes.InstalledWin32Cultures) != 0;
		}

		private static int GetResidentNeutralLcid(int culture)
		{
			return IsResident(culture) ? culture : 127;
		}
	}
}
