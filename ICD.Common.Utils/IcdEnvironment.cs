using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils
{
	public static partial class IcdEnvironment
	{
		/// <summary>
		/// Enumeration to define the various runtime environments a module can run in.
		/// </summary>
		public enum eRuntimeEnvironment
		{
			SimplSharp,
			SimplSharpPro,
			SimplSharpProMono,
			Standard
		}

		/// <summary>
		/// Enum for the Program Event Types
		/// </summary>
		public enum eProgramStatusEventType
		{
			Stopping,
			Paused,
			Resumed,
		}

		/// <summary>
		/// Enum for the Ethernet Event Types
		/// </summary>
		public enum eEthernetEventType
		{
			LinkDown,
			LinkUp,
		}

		/// <summary>
		/// Enums for the Ethernet Adapter Type
		/// </summary>
		[Flags]
		public enum eEthernetAdapterType
		{
			EthernetUnknownAdapter = 0,
			EthernetLanAdapter = 1,
			EthernetCsAdapter = 2,
			EthernetWifiAdapter = 4,
			EthernetLan2Adapter = 8,
		}

		public delegate void ProgramStatusCallback(eProgramStatusEventType type);

		public delegate void EthernetEventCallback(eEthernetAdapterType adapter, eEthernetEventType type);

		public static event ProgramStatusCallback OnProgramStatusEvent;

		public static event EthernetEventCallback OnEthernetEvent;

		/// <summary>
		/// Raised when the program has completed initialization.
		/// </summary>
		public static event EventHandler OnProgramInitializationComplete;

		/// <summary>
		/// Raised when the system date/time has been set.
		/// </summary>
		public static event EventHandler OnSystemDateTimeChanged;

		private static readonly SafeCriticalSection s_ProgramInitializationSection = new SafeCriticalSection();
		private static bool s_ProgramInitializationComplete;

		/// <summary>
		/// Returns true if the program has been flagged as completely initialized.
		/// </summary>
		public static bool ProgramIsInitialized { get { return s_ProgramInitializationSection.Execute(() => s_ProgramInitializationComplete); } }

		/// <summary>
		/// Called by the program entry point to signify that the program initialization is complete.
		/// </summary>
		[PublicAPI]
		public static void SetProgramInitializationComplete()
		{
			s_ProgramInitializationSection.Enter();

			try
			{
				if (s_ProgramInitializationComplete)
					return;

				s_ProgramInitializationComplete = true;
			}
			finally
			{
				s_ProgramInitializationSection.Leave();
			}

			OnProgramInitializationComplete.Raise(null);
		}

		/// <summary>
		/// Gets UTC time
		/// Uses GetLocalTime so Crestron Env will have ms percision
		/// </summary>
		/// <returns></returns>
		public static DateTime GetUtcTime()
		{
			return GetLocalTime().ToUniversalTime();
		}
	}
}
