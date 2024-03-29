﻿#if !NETSTANDARD
using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils
{
	public static partial class IcdEnvironment
	{
		/// <summary>
		/// For some reason crestron returns "Invalid Value" for ethernet parameters sometimes :/
		/// </summary>
		private const string INVALID_VALUE = "Invalid Value";

		#region Properties

		public static string NewLine { get { return CrestronEnvironment.NewLine; } }

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

				foreach (EthernetAdapterType type in EnumUtils.GetValuesExceptNone<EthernetAdapterType>())
				{
					string address;

					try
					{
						short id = CrestronEthernetHelper.GetAdapterdIdForSpecifiedAdapterType(type);
						if (id >= InitialParametersClass.NumberOfEthernetInterfaces)
							continue;

						address = CrestronEthernetHelper.GetEthernetParameter(param, id);
					}
					catch (ArgumentException)
					{
						continue;
					}

					if (!string.IsNullOrEmpty(address) && !address.Equals(INVALID_VALUE))
						yield return address;
				}
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

				foreach (EthernetAdapterType type in EnumUtils.GetValuesExceptNone<EthernetAdapterType>())
				{
					string macAddress;

					try
					{
						short id = CrestronEthernetHelper.GetAdapterdIdForSpecifiedAdapterType(type);
						if (id >= InitialParametersClass.NumberOfEthernetInterfaces)
							continue;

						macAddress = CrestronEthernetHelper.GetEthernetParameter(param, id);
					}
					catch (ArgumentException)
					{
						continue;
					}

					if (!string.IsNullOrEmpty(macAddress) && !macAddress.Equals(INVALID_VALUE))
						yield return macAddress;
				}
			}
		}

		/// <summary>
		/// Gets the dhcp status of the processor.
		/// </summary>
		[PublicAPI]
		public static bool DhcpStatus
		{
			get
			{
				const CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET param =
					CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_CURRENT_DHCP_STATE;
				const EthernetAdapterType type = EthernetAdapterType.EthernetLANAdapter;

				try
				{
					short id = CrestronEthernetHelper.GetAdapterdIdForSpecifiedAdapterType(type);
					if (id >= InitialParametersClass.NumberOfEthernetInterfaces)
						return false;

					string status = CrestronEthernetHelper.GetEthernetParameter(param, id);

					if (!string.IsNullOrEmpty(status) && !status.Equals(INVALID_VALUE))
						return status == "ON";

					return false;
				}
				catch (ArgumentException)
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Gets the SSL state of the processor.
		/// </summary>
		[PublicAPI]
		public static bool SslEnabled
		{
			get
			{
				const CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET param =
					CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_SSL_OFF_STATUS;
				const EthernetAdapterType type = EthernetAdapterType.EthernetLANAdapter;

				try
				{
					short id = CrestronEthernetHelper.GetAdapterdIdForSpecifiedAdapterType(type);
					if (id >= InitialParametersClass.NumberOfEthernetInterfaces)
						return false;

					string status = CrestronEthernetHelper.GetEthernetParameter(param, id);

					if (!string.IsNullOrEmpty(status) && !status.Equals(INVALID_VALUE))
						// GET_SSL_OFF_STATUS return OFF for SSL is Enabled and ON for SSL is disabled
						return status == "OFF";

					return false;
				}
				catch (ArgumentException)
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Gets the hostname of the processor.
		/// </summary>
		[PublicAPI]
		public static IEnumerable<string> Hostnames
		{
			get
			{
				const CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET param =
					CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_HOSTNAME;

				foreach (EthernetAdapterType type in EnumUtils.GetValuesExceptNone<EthernetAdapterType>())
				{
					string hostname;

					try
					{
						short id = CrestronEthernetHelper.GetAdapterdIdForSpecifiedAdapterType(type);
						if (id >= InitialParametersClass.NumberOfEthernetInterfaces)
							continue;

						hostname = CrestronEthernetHelper.GetEthernetParameter(param, id);
					}
					catch (ArgumentException)
					{
						continue;
					}

					if (!string.IsNullOrEmpty(hostname) && !hostname.Equals(INVALID_VALUE))
						yield return hostname;
				}
			}
		}

		#endregion

		/// <summary>
		/// Static constructor.
		/// </summary>
		static IcdEnvironment()
		{
			// Cache the runtime environment
			s_Framework = eFramework.Crestron;

			s_CrestronRuntimeEnvironment = CrestronEnvironment.RuntimeEnvironment == eRuntimeEnvironment.SIMPL
				? eCrestronRuntimeEnvironment.SimplPlus
				: eCrestronRuntimeEnvironment.SimplSharpPro;

			s_CrestronDevicePlatform = CrestronEnvironment.DevicePlatform == eDevicePlatform.Appliance
				? eCrestronDevicePlatform.Appliance
				: eCrestronDevicePlatform.Server;

			// todo: Make this check more robust
			s_CrestronSeries = Type.GetType("Mono.Runtime") != null ? eCrestronSeries.FourSeries : eCrestronSeries.ThreeSeries;

			CrestronEnvironment.ProgramStatusEventHandler += CrestronEnvironmentOnProgramStatusEventHandler;
			CrestronEnvironment.EthernetEventHandler += CrestronEnvironmentOnEthernetEventHandler;
		}

		#region Methods

		/// <summary>
		/// Gets the name of the local time zone.
		/// </summary>
		/// <returns></returns>
		public static string GetLocalTimeZoneName()
		{
			return CrestronEnvironment.GetTimeZone().Name;
		}

		public static DateTime GetLocalTime()
		{
			return CrestronEnvironment.GetLocalTime();
		}

		public static void SetLocalTime(DateTime localTime)
		{
			CrestronEnvironment.SetTimeAndDate((ushort)localTime.Hour,
			                                   (ushort)localTime.Minute,
			                                   (ushort)localTime.Second,
			                                   (ushort)localTime.Month,
			                                   (ushort)localTime.Day,
			                                   (ushort)localTime.Year);

			OnSystemDateTimeChanged.Raise(null);
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
