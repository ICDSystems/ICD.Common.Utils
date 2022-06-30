#if !NETSTANDARD
using System.Globalization;
using Crestron.SimplSharp;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using System;
using System.Text.RegularExpressions;
using ICD.Common.Properties;

namespace ICD.Common.Utils
{
	public static partial class ProcessorUtils
	{
		// CP3 - CP3 Cntrl Eng [v1.601.3934.19631 (Oct 10 2019), #00A3BBE7] @E-00107f4a3474
		// CP4 - CP4 Cntrl Eng [v2.4474.00005 (Apr  9 2020), #8EB216B7] @E-00107feb538f
		private const string VER_REGEX =
			@"(?'model'\S+)\s+(?'type'\S+)\s+(?'lang'\S+)\s+\[v(?'version'\d+(\.\d+)+)\s+\((?'date'\S+\s+\d+ \d+)\),\s+#(?'serial'[A-F0-9]+)\]\s+@E-(?'mac'[a-z0-9]+)";

		private const string UPTIME_COMMAND = "uptime";
		private const string PROGUPTIME_COMMAND_ROOT = "proguptime:{0}";
		private const string UPTIME_REGEX = @".*(?'uptime'(?'days'\d+) days (?'hours'\d{2}):(?'minutes'\d{2}):(?'seconds'\d{2})\.(?'milliseconds'\d+))";

		private const string RAMFREE_COMMAND = "ramfree";
		private const string RAMFREE_DIGITS_REGEX = @"^(\d*)";

		private static string s_VersionResult;

		private static DateTime? s_SystemUptimeStartTimeUtc;
		private static DateTime? s_ProgramUptimeStartTimeUtc;

		#region Properties

		/// <summary>
		/// Gets the version text from the console.
		/// </summary>
		private static string VersionResult
		{
			get
			{
				if (string.IsNullOrEmpty(s_VersionResult))
				{
					if (!IcdConsole.SendControlSystemCommand("version", ref s_VersionResult))
					{
						ServiceProvider.TryGetService<ILoggerService>()
						               .AddEntry(eSeverity.Warning, "{0} - Failed to send console command \"{1}\"",
						                         typeof(ProcessorUtils).Name, "version");
					}
				}

				return s_VersionResult;
			}
		}

		/// <summary>
		/// Gets the model name of the processor.
		/// </summary>
		[PublicAPI]
		public static string ModelName
		{
			get
			{
				string versionResult = VersionResult;
				if (!String.IsNullOrEmpty(versionResult))
				{
					Regex regex = new Regex(VER_REGEX);
					Match match = regex.Match(versionResult);

					if (match.Success)
						return match.Groups["model"].Value;
				}

				ServiceProvider.TryGetService<ILoggerService>()
				               .AddEntry(eSeverity.Warning, "Unable to get model name from \"{0}\"", VersionResult);
				return string.Empty;
			}
		}

		/// <summary>
		/// Gets the processor firmware version.
		/// </summary>
		[PublicAPI]
		public static string ModelVersion
		{
			get
			{
				return CrestronEnvironment.OSVersion.Firmware;
			}
		}

		/// <summary>
		/// Gets the date that the firmware was updated.
		/// </summary>
		[PublicAPI]
		public static DateTime ModelVersionDate
		{
			get
			{
				Regex regex = new Regex(VER_REGEX);
				Match match = regex.Match(VersionResult);

				if (!match.Success)
				{
					ServiceProvider.TryGetService<ILoggerService>()
					               .AddEntry(eSeverity.Warning, "Unable to get model version date from \"{0}\"", VersionResult);
					return DateTime.MinValue;
				}

				string date = match.Groups["date"].Value;

				try
				{
					if (IcdEnvironment.CrestronSeries == IcdEnvironment.eCrestronSeries.FourSeries)
					{
						date = StringUtils.RemoveDuplicateWhitespace(date);
						return DateTime.ParseExact(date, "MMM d yyyy", CultureInfo.InvariantCulture).ToUniversalTime();
					}
					
					return DateTime.ParseExact(date, "MMM dd yyyy", CultureInfo.InvariantCulture).ToUniversalTime();
				}
				catch (FormatException)
				{
					ServiceProvider.TryGetService<ILoggerService>()
								   .AddEntry(eSeverity.Warning, "Failed to parse date \"{0}\"", date);
					return DateTime.MinValue;
				}
			}
		}

		/// <summary>
		/// Gets the serial number of the processor
		/// </summary>
		[PublicAPI]
		public static string ProcessorSerialNumber
		{
			get
			{
				return CrestronEnvironment.SystemInfo.SerialNumber;
			}
		}

		/// <summary>
		/// Gets the ram usage in the range 0 - 1.
		/// </summary>
		public static float RamUsagePercent
		{
			get
			{
				string ramFree = GetRamFree();
				string digits = Regex.Matches(ramFree, RAMFREE_DIGITS_REGEX, RegexOptions.Multiline)[0].Groups[1].Value;
				return float.Parse(digits) / 100.0f;
			}
		}

		/// <summary>
		/// Gets the total number of bytes of physical memory.
		/// </summary>
		public static ulong RamTotalBytes
		{
			get
			{
				string ramFree = GetRamFree();
				string digits = Regex.Matches(ramFree, RAMFREE_DIGITS_REGEX, RegexOptions.Multiline)[1].Groups[1].Value;
				return ulong.Parse(digits);
			}
		}

		/// <summary>
		/// Gets the total number of bytes of physical memory being used by the control system.
		/// </summary>
		public static ulong RamUsedBytes
		{
			get
			{
				string ramFree = GetRamFree();
				string digits = Regex.Matches(ramFree, RAMFREE_DIGITS_REGEX, RegexOptions.Multiline)[2].Groups[1].Value;
				return ulong.Parse(digits);
			}
		}

		/// <summary>
		/// Gets the total number of bytes of physical memory not being used by the control system.
		/// </summary>
		public static ulong RamBytesFree
		{
			get
			{
				string ramFree = GetRamFree();
				string digits = Regex.Matches(ramFree, RAMFREE_DIGITS_REGEX, RegexOptions.Multiline)[3].Groups[1].Value;
				return ulong.Parse(digits);
			}
		}

		/// <summary>
		/// Gets the total number of bytes that can be reclaimed.
		/// </summary>
		public static ulong RamBytesReclaimable
		{
			get
			{
				string ramFree = GetRamFree();
				string digits = Regex.Matches(ramFree, RAMFREE_DIGITS_REGEX, RegexOptions.Multiline)[4].Groups[1].Value;
				return ulong.Parse(digits);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Restarts this program.
		/// </summary>
		[PublicAPI]
		public static void RestartProgram()
		{
			ILoggerService logger = ServiceProvider.TryGetService<ILoggerService>();
			if (logger != null)
				logger.AddEntry(eSeverity.Informational, "Intentional Restart of Program");

			string consoleResult = string.Empty;
			string command = string.Format("progreset -p:{0:D2}", ProgramUtils.ProgramNumber);
			IcdConsole.SendControlSystemCommand(command, ref consoleResult);
		}

		/// <summary>
		/// Reboots the processor.
		/// </summary>
		[PublicAPI]
		public static void Reboot()
		{
			ILoggerService logger = ServiceProvider.TryGetService<ILoggerService>();
			if (logger != null)
				logger.AddEntry(eSeverity.Informational, "Intentional Reboot of Processor");

			string consoleResult = string.Empty;
			IcdConsole.SendControlSystemCommand("reboot", ref consoleResult);
		}

		/// <summary>
		/// Gets the time the system was started
		/// DateTime that uptime starts
		/// </summary>
		/// <returns></returns>
		[PublicAPI]
		public static DateTime? GetSystemStartTime()
		{
			if (s_SystemUptimeStartTimeUtc == null)
				UpdateSystemStartTime();

			return s_SystemUptimeStartTimeUtc;
		}

		/// <summary>
		/// Gets the uptime of the current program.
		/// </summary>
		/// <returns></returns>
		[PublicAPI]
		public static DateTime? GetProgramStartTime()
		{
			if (s_ProgramUptimeStartTimeUtc != null)
				return s_ProgramUptimeStartTimeUtc;


			string uptime = GetProgramUptimeFeedback((int)ProgramUtils.ProgramNumber);
			Match match = Regex.Match(uptime, UPTIME_REGEX);
			if (!match.Success)
				return null;

			int days = int.Parse(match.Groups["days"].Value);
			int hours = int.Parse(match.Groups["hours"].Value);
			int minutes = int.Parse(match.Groups["minutes"].Value);
			int seconds = int.Parse(match.Groups["seconds"].Value);
			int milliseconds = int.Parse(match.Groups["milliseconds"].Value);

			TimeSpan span = new TimeSpan(days, hours, minutes, seconds, milliseconds);
			s_ProgramUptimeStartTimeUtc = IcdEnvironment.GetUtcTime() - span;

			return s_ProgramUptimeStartTimeUtc;
		}

		#endregion

		private static void UpdateSystemStartTime()
		{
			string uptime = GetSystemUptimeFeedback();
			Match match = Regex.Match(uptime, UPTIME_REGEX);
			if (!match.Success)
				return;

			int days = int.Parse(match.Groups["days"].Value);
			int hours = int.Parse(match.Groups["hours"].Value);
			int minutes = int.Parse(match.Groups["minutes"].Value);
			int seconds = int.Parse(match.Groups["seconds"].Value);
			int milliseconds = int.Parse(match.Groups["milliseconds"].Value);

			TimeSpan span = new TimeSpan(days, hours, minutes, seconds, milliseconds);
			s_SystemUptimeStartTimeUtc = IcdEnvironment.GetUtcTime() - span;
		}

		/// <summary>
		/// Gets the result from the ramfree console command.
		/// </summary>
		/// <returns></returns>
		private static string GetRamFree()
		{
			string ramfree = null;
			if (!IcdConsole.SendControlSystemCommand(RAMFREE_COMMAND, ref ramfree))
			{
				ServiceProvider.TryGetService<ILoggerService>()
				               .AddEntry(eSeverity.Warning, "{0} - Failed to send console command \"{1}\"",
				                         typeof(ProcessorUtils).Name, RAMFREE_COMMAND);
			}
			return ramfree;
		}

		private static string GetSystemUptimeFeedback()
		{
			string uptime = null;
			if (!IcdConsole.SendControlSystemCommand(UPTIME_COMMAND, ref uptime))
			{
				ServiceProvider.TryGetService<ILoggerService>()
							   .AddEntry(eSeverity.Warning, "{0} - Failed to send console command \"{1}\"",
										 typeof(ProcessorUtils).Name, UPTIME_COMMAND);
			}
			return uptime;
		}

		private static string GetProgramUptimeFeedback(int programSlot)
		{
			string uptime = null;
			if (!IcdConsole.SendControlSystemCommand(string.Format(PROGUPTIME_COMMAND_ROOT, programSlot), ref uptime))
			{
				ServiceProvider.TryGetService<ILoggerService>()
							   .AddEntry(eSeverity.Warning, "{0} - Failed to send console command \"{1}\"",
										 typeof(ProcessorUtils).Name, PROGUPTIME_COMMAND_ROOT);
			}
			return uptime;
		}
	}
}

#endif
