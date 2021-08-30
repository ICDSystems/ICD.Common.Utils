using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils
{
	public static partial class IcdEnvironment
	{
		/// <summary>
		/// Enumeration to define the various frameworks a module can run in.
		/// </summary>
		public enum eFramework
		{
			Crestron,
			Framework,
			Standard
		}

		/// <summary>
		/// Enumeration to define the Crestron series a module can run it
		/// </summary>
		public enum eCrestronSeries
		{
			Na, //Non-Crestron
			ThreeSeries,
			FourSeries
		}

		/// <summary>
		/// Enumeration to define the various Crestron runtime environments a module can run in
		/// </summary>
		public enum eCrestronRuntimeEnvironment
		{
			Na, //Non-Crestron
			Simpl, // Running in Simpl, Non-Pro
			Appliance, // S#Pro running on a Crestron hardware appliance
			Server // S#Pro running on a server (VC-4)
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

		/// <summary>
		/// Enum for session change events.
		/// </summary>
		public enum eSessionChangeEventType
		{
			None = 0,
			ConsoleConnect = 1,
			ConsoleDisconnect = 2,
			RemoteConnect = 3,
			RemoteDisconnect = 4,
			SessionLogon = 5,
			SessionLogoff = 6,
			SessionLock = 7,
			SessionUnlock = 8,
			SessionRemoteControl = 9
		}

		public delegate void ProgramStatusCallback(eProgramStatusEventType type);

		public delegate void EthernetEventCallback(eEthernetAdapterType adapter, eEthernetEventType type);

		public delegate void SessionChangeEventCallback(int sessionId, eSessionChangeEventType type);

		/// <summary>
		/// Raised when the program status changes.
		/// </summary>
		public static event ProgramStatusCallback OnProgramStatusEvent;

		/// <summary>
		/// Raised when a network adapter connects/disconnects.
		/// </summary>
		public static event EthernetEventCallback OnEthernetEvent;

		/// <summary>
		/// Raised when a session changes, such as user logging in/out.
		/// </summary>
		public static event SessionChangeEventCallback OnSessionChangedEvent;

		/// <summary>
		/// Raised when the program has completed initialization.
		/// </summary>
		public static event EventHandler OnProgramInitializationComplete;

		/// <summary>
		/// Raised when the system date/time has been set.
		/// </summary>
		public static event EventHandler OnSystemDateTimeChanged;

		private static readonly eFramework s_Framework;
		private static readonly eCrestronSeries s_CrestronSeries;
		private static readonly eCrestronRuntimeEnvironment s_CrestronRuntimeEnvironment;

		private static readonly SafeCriticalSection s_ProgramInitializationSection = new SafeCriticalSection();
		private static bool s_ProgramInitializationComplete;

		public static eFramework Framework {get { return s_Framework; }}
		public static eCrestronSeries CrestronSeries {get { return s_CrestronSeries; }}
		public static eCrestronRuntimeEnvironment CrestronRuntimeEnvironment {get { return s_CrestronRuntimeEnvironment; }}

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
		/// Gets UTC time.
		/// </summary>
		/// <returns></returns>
		public static DateTime GetUtcTime()
		{
			// Use GetLocalTime so Crestron Env will have ms precision
			return GetLocalTime().ToUniversalTime();
		}
	}
}
