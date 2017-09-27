#if SIMPLSHARP
using System;
using System.Text.RegularExpressions;
using Crestron.SimplSharp;
using ICD.Common.Properties;
using ICD.Common.Services;
using ICD.Common.Services.Logging;

namespace ICD.Common.Utils
{
	public static class CrestronUtils
	{
		private const string MODEL_NAME_REGEX = @"^(\S*)";
		private const string MODEL_VERSION_REGEX = @" [[]v(\S*)";

		private const string RAMFREE_COMMAND = "ramfree";
		private const string RAMFREE_DIGITS_REGEX = @"^(\d*)";

		private static string s_VersionResult;

		#region Properties

		/// <summary>
		/// Gets the default mac address of the processor.
		/// </summary>
		[PublicAPI]
		public static string DefaultMacAddress
		{
			get
			{
				const CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET param =
					CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_MAC_ADDRESS;
				const EthernetAdapterType type = EthernetAdapterType.EthernetLANAdapter;
				short id = CrestronEthernetHelper.GetAdapterdIdForSpecifiedAdapterType(type);
				return CrestronEthernetHelper.GetEthernetParameter(param, id);
			}
		}

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
						                         typeof(CrestronUtils).Name, "version");
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
				Regex regex = new Regex(MODEL_NAME_REGEX);
				Match match = regex.Match(VersionResult);

				if (match.Success)
					return match.Groups[1].Value;

				ServiceProvider.TryGetService<ILoggerService>()
				               .AddEntry(eSeverity.Warning, "Unable to get model name from \"{0}\"", VersionResult);
				return string.Empty;
			}
		}

		/// <summary>
		/// Gets the processor firmware version.
		/// </summary>
		[PublicAPI]
		public static Version ModelVersion
		{
			get
			{
				Regex regex = new Regex(MODEL_VERSION_REGEX);
				Match match = regex.Match(VersionResult);

				if (match.Success)
					return new Version(match.Groups[1].Value);

				ServiceProvider.TryGetService<ILoggerService>()
				               .AddEntry(eSeverity.Warning, "Unable to get model version from \"{0}\"", VersionResult);
				return new Version(0, 0);
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
			string consoleResult = string.Empty;
			IcdConsole.SendControlSystemCommand("reboot", ref consoleResult);
		}

		#endregion

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
				                         typeof(CrestronUtils).Name, RAMFREE_COMMAND);
			}
			return ramfree;
		}

		[PublicAPI]
		public static string GetMilliseconds()
		{
			return IcdEnvironment.GetLocalTime().ToString("fff");
		}
	}
}

#endif
