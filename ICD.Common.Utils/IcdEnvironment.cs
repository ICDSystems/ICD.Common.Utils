using System;

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
	}
}
