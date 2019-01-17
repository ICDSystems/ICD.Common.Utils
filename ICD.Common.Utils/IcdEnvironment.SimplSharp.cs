#if SIMPLSHARP
using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using ICD.Common.Properties;

namespace ICD.Common.Utils
{
	public static partial class IcdEnvironment
	{
		public static string NewLine { get { return CrestronEnvironment.NewLine; } }

		public static eRuntimeEnvironment RuntimeEnvironment
		{
			get
			{
				return CrestronEnvironment.DevicePlatform == eDevicePlatform.Server
					       ? eRuntimeEnvironment.SimplSharpProMono
					       : GetRuntimeEnvironment(CrestronEnvironment.RuntimeEnvironment);
			}
		}

		/// <summary>
		/// Gets the network address(es) of the processor.
		/// </summary>
		[PublicAPI]
		public static IEnumerable<string> NetworkAddresses
		{
			get
			{
				const CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET param =
					CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_CURRENT_IP_ADDRESS;
				const EthernetAdapterType type = EthernetAdapterType.EthernetLANAdapter;
				short id = CrestronEthernetHelper.GetAdapterdIdForSpecifiedAdapterType(type);
				yield return CrestronEthernetHelper.GetEthernetParameter(param, id);
			}
		}

		/// <summary>
		/// Gets the mac address(es) of the processor.
		/// </summary>
		[PublicAPI]
		public static IEnumerable<string> MacAddresses
		{
			get
			{
				const CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET param =
					CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_MAC_ADDRESS;
				const EthernetAdapterType type = EthernetAdapterType.EthernetLANAdapter;
				short id = CrestronEthernetHelper.GetAdapterdIdForSpecifiedAdapterType(type);
				yield return CrestronEthernetHelper.GetEthernetParameter(param, id);
			}
		}

		/// <summary>
		/// Static constructor.
		/// </summary>
		static IcdEnvironment()
		{
			CrestronEnvironment.ProgramStatusEventHandler += CrestronEnvironmentOnProgramStatusEventHandler;
			CrestronEnvironment.EthernetEventHandler += CrestronEnvironmentOnEthernetEventHandler;
		}

		#region Methods

		public static DateTime GetLocalTime()
		{
			return CrestronEnvironment.GetLocalTime();
		}

		public static eEthernetEventType GetEthernetEventType(Crestron.SimplSharp.eEthernetEventType type)
		{
			switch (type)
			{
				case Crestron.SimplSharp.eEthernetEventType.LinkDown:
					return eEthernetEventType.LinkDown;
				case Crestron.SimplSharp.eEthernetEventType.LinkUp:
					return eEthernetEventType.LinkUp;
				default:
					throw new ArgumentOutOfRangeException("type");
			}
		}

		public static eEthernetAdapterType GetEthernetAdapterType(EthernetAdapterType ethernetAdapter)
		{
			return (eEthernetAdapterType)(int)ethernetAdapter;
		}

		public static eProgramStatusEventType GetProgramStatusEventType(Crestron.SimplSharp.eProgramStatusEventType type)
		{
			switch (type)
			{
				case Crestron.SimplSharp.eProgramStatusEventType.Stopping:
					return eProgramStatusEventType.Stopping;
				case Crestron.SimplSharp.eProgramStatusEventType.Paused:
					return eProgramStatusEventType.Paused;
				case Crestron.SimplSharp.eProgramStatusEventType.Resumed:
					return eProgramStatusEventType.Resumed;
				default:
					throw new ArgumentOutOfRangeException("type");
			}
		}

		public static eRuntimeEnvironment GetRuntimeEnvironment(Crestron.SimplSharp.eRuntimeEnvironment runtimeEnvironment)
		{
			switch (runtimeEnvironment)
			{
				case Crestron.SimplSharp.eRuntimeEnvironment.SIMPL:
					return eRuntimeEnvironment.SimplSharp;
				case Crestron.SimplSharp.eRuntimeEnvironment.SimplSharpPro:
					return eRuntimeEnvironment.SimplSharpPro;
				default:
					throw new ArgumentOutOfRangeException("runtimeEnvironment");
			}
		}

		#endregion

		#region Private Methods

		private static void CrestronEnvironmentOnEthernetEventHandler(EthernetEventArgs args)
		{
			EthernetEventCallback handler = OnEthernetEvent;
			if (handler != null)
				handler(GetEthernetAdapterType(args.EthernetAdapter), GetEthernetEventType(args.EthernetEventType));
		}

		private static void CrestronEnvironmentOnProgramStatusEventHandler(Crestron.SimplSharp.eProgramStatusEventType type)
		{
			ProgramStatusCallback handler = OnProgramStatusEvent;
			if (handler != null)
				handler(GetProgramStatusEventType(type));
		}

		#endregion
	}
}

#endif
