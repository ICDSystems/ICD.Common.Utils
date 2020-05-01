#if !SIMPLSHARP
using System;
using ICD.Common.Properties;

namespace ICD.Common.Utils
{
	public static partial class ProcessorUtils
	{
		#region Properties

		/// <summary>
		/// Gets the model name of the processor.
		/// </summary>
		[PublicAPI]
		public static string ModelName { get { return Environment.MachineName; } }

		/// <summary>
		/// Gets the processor firmware version.
		/// </summary>
		[PublicAPI]
		public static Version ModelVersion
		{
			get
			{
				// TODO
				return new Version("1.0.0.0");
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
				// TODO
				return DateTime.MinValue;
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
				// TODO
				return null;
			}
		}

		/// <summary>
		/// Gets the ram usage in the range 0 - 1.
		/// </summary>
		public static float RamUsagePercent
		{
			get
			{
				// TODO
				return 0.0f;
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
			throw new NotSupportedException();
		}

		/// <summary>
		/// Reboots the processor.
		/// </summary>
		[PublicAPI]
		public static void Reboot()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets the uptime for the system
		/// </summary>
		/// <returns></returns>
		[PublicAPI]
		public static TimeSpan GetSystemUptime()
		{
			return TimeSpan.FromMilliseconds(Environment.TickCount);
		}

		/// <summary>
		/// Gets the uptime 
		/// </summary>
		/// <returns></returns>
		[PublicAPI]
		public static TimeSpan GetProgramUptime()
		{
			var current = System.Diagnostics.Process.GetCurrentProcess();
			return IcdEnvironment.GetLocalTime() - current.StartTime;
		}

		#endregion
	}
}
#endif
