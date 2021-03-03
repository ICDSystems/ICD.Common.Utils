#if !SIMPLSHARP
using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using ICD.Common.Properties;
using ICD.Common.Utils.IO;
using Microsoft.Win32;

namespace ICD.Common.Utils
{
	public static partial class ProcessorUtils
	{
		private static DateTime? s_SystemStartTime;

		#region Properties

		/// <summary>
		/// Gets the model name of the processor.
		/// </summary>
		[PublicAPI]
		public static string ModelName
		{
			get
			{
				string productName = RegistryLocalMachineGetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
				if (productName == string.Empty)
					return string.Empty;

				string csdVersion = RegistryLocalMachineGetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion");

				return (productName.StartsWith("Microsoft") ? string.Empty : "Microsoft ") +
				       productName +
				       (csdVersion == string.Empty ? string.Empty : " " + csdVersion);
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
				return RegistryLocalMachineGetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild");
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
				long time = RegistryLocalMachineGetLong(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "InstallTime");
				return time == 0 ? DateTime.MinValue : DateTime.FromFileTime(132448642489109028).ToUniversalTime();
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
				ManagementObject os = new ManagementObject("Win32_OperatingSystem=@");
				return (string)os["SerialNumber"];
			}
		}

		/// <summary>
		/// Gets the ram usage in the range 0 - 1.
		/// </summary>
		public static float RamUsagePercent
		{
			get
			{
				ManagementObjectSearcher wmiObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");

				var memoryValues =
					wmiObject.Get()
					         .Cast<ManagementObject>()
					         .Select(mo => new
					         {
						         FreePhysicalMemory = double.Parse(mo["FreePhysicalMemory"].ToString()),
						         TotalVisibleMemorySize = double.Parse(mo["TotalVisibleMemorySize"].ToString())
					         })
					         .FirstOrDefault();

				return memoryValues == null
					? 0
					: (float)((memoryValues.TotalVisibleMemorySize - memoryValues.FreePhysicalMemory) /
					          memoryValues.TotalVisibleMemorySize);
			}
		}

		/// <summary>
		/// Gets the total number of bytes of physical memory.
		/// </summary>
		public static ulong RamTotalBytes
		{
			get
			{
				// TODO
				return 0;
			}
		}

		/// <summary>
		/// Gets the total number of bytes of physical memory being used by the control system.
		/// </summary>
		public static ulong RamUsedBytes
		{
			get
			{
				// TODO
				return 0;
			}
		}

		/// <summary>
		/// Gets the total number of bytes of physical memory not being used by the control system.
		/// </summary>
		public static ulong RamBytesFree
		{
			get
			{
				// TODO
				return 0;
			}
		}

		/// <summary>
		/// Gets the total number of bytes that can be reclaimed.
		/// </summary>
		public static ulong RamBytesReclaimable
		{
			get
			{
				// TODO
				return 0;
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
			string filename = Process.GetCurrentProcess().MainModule?.FileName;
			if (string.IsNullOrEmpty(filename) || !IcdFile.Exists(filename))
				throw new InvalidOperationException("Failed to find program filename");

			Process.Start(filename);
			Environment.Exit(0);
		}

		/// <summary>
		/// Reboots the processor.
		/// </summary>
		[PublicAPI]
		public static void Reboot()
		{
			// TODO - Linux
			ProcessStartInfo psi =
				new ProcessStartInfo("shutdown", "/r /t 0")
				{
					CreateNoWindow = true,
					UseShellExecute = false
				};

			Process.Start(psi);
		}

		/// <summary>
		/// Gets the time the system was started
		/// DateTime that uptime starts
		/// </summary>
		/// <returns></returns>
		[PublicAPI]
		public static DateTime? GetSystemStartTime()
		{
			return s_SystemStartTime ?? (s_SystemStartTime = Process.GetCurrentProcess().StartTime.ToUniversalTime());
		}

		/// <summary>
		/// Gets the time the program was started
		/// Datetime the program starts
		/// </summary>
		/// <returns></returns>
		[PublicAPI]
		public static DateTime? GetProgramStartTime()
		{
			return GetSystemStartTime();
		}

		#endregion

		#region Private Methods

		private static string RegistryLocalMachineGetString(string path, string key)
		{
			try
			{
				RegistryKey rk = Registry.LocalMachine.OpenSubKey(path);
				return rk == null ? string.Empty : (string)rk.GetValue(key);
			}
			catch
			{
				return string.Empty;
			}
		}

		private static long RegistryLocalMachineGetLong(string path, string key)
		{
			try
			{
				RegistryKey rk = Registry.LocalMachine.OpenSubKey(path);
				return rk == null ? 0 : (long)rk.GetValue(key);
			}
			catch
			{
				return 0;
			}
		}

		#endregion
	}
}
#endif
