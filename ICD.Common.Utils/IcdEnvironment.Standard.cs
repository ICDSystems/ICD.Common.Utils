﻿#if STANDARD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using ICD.Common.Properties;

namespace ICD.Common.Utils
{
	public static partial class IcdEnvironment
	{
		public static string NewLine { get { return Environment.NewLine; } }

		/// <summary>
		/// Gets the network address(es) of the processor.
		/// </summary>
		[PublicAPI]
		public static IEnumerable<string> NetworkAddresses
		{
			get
			{
				return NetworkInterface.GetAllNetworkInterfaces()
				                       .Where(ni => ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
				                                    ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
				                       .SelectMany(ni => ni.GetIPProperties().UnicastAddresses
				                                           .Where(ua => ua.Address.AddressFamily == AddressFamily.InterNetwork)
				                                           .Select(ua => ua.Address.ToString()));
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
				return NetworkInterface.GetAllNetworkInterfaces()
				                       .Where(ni => ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
				                                    ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
				                       .Select(ni => ni.GetPhysicalAddress().ToString())
									   .Select(FormatMacAddress);
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
				try
				{
					return
						NetworkInterface.GetAllNetworkInterfaces()
										.Where(ni => ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
													 ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
										.Select(ni => ni.GetIPProperties().GetIPv4Properties().IsDhcpEnabled)
										.FirstOrDefault();
				}
				catch (PlatformNotSupportedException)
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Exists for parity with SimplSharp processors which have the ability to enable/disable SSL communication.
		/// </summary>
		[PublicAPI]
		public static bool SslEnabled
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the hostname of the processor.
		/// </summary>
		[PublicAPI]
		public static IEnumerable<string> Hostnames { get { yield return Dns.GetHostName(); } }

		static IcdEnvironment()
		{
			s_Framework = eFramework.Standard;
			s_CrestronSeries = eCrestronSeries.Na;
			s_CrestronRuntimeEnvironment = eCrestronRuntimeEnvironment.Na;
		}

		/// <summary>
		/// Gets the name of the local time zone.
		/// </summary>
		/// <returns></returns>
		public static string GetLocalTimeZoneName()
		{
			return System.TimeZoneInfo.Local.IsDaylightSavingTime(GetLocalTime())
				       ? System.TimeZoneInfo.Local.DaylightName
				       : System.TimeZoneInfo.Local.StandardName;
		}

		public static DateTime GetLocalTime()
		{
			return DateTime.Now;
		}

		public static void SetLocalTime(DateTime localTime)
		{
#if DEBUG
			IcdConsole.PrintLine(eConsoleColor.Magenta, "Debug Build - Skipped setting local time to {0}",
			                     localTime.ToString("s"));
#else
			throw new NotSupportedException();
#endif
		}

		/// <summary>
		/// Converts 12 digit address to XX:XX:XX... format
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		private static string FormatMacAddress(string address)
		{
			const string regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
			const string replace = "$1:$2:$3:$4:$5:$6";
			return Regex.Replace(address, regex, replace);
		}

		/// <summary>
		/// Called by the application to raise the program status.
		/// </summary>
		/// <param name="status"></param>
		public static void SetProgramStatus(eProgramStatusEventType status)
		{
			ProgramStatusCallback handler = OnProgramStatusEvent;
			if (handler != null)
				handler(status);
		}

		/// <summary>
		/// Called by the application to raise session change events.
		/// </summary>
		/// <param name="sessionId"></param>
		/// <param name="reasonCode"></param>
		public static void HandleSessionChange(int sessionId, eSessionChangeEventType reasonCode)
		{
			SessionChangeEventCallback handler = OnSessionChangedEvent;
			if (handler != null)
				handler(sessionId, reasonCode);
		}
	}
}
#endif
