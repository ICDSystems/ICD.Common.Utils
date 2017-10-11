#if STANDARD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using ICD.Common.Properties;

namespace ICD.Common.Utils
{
	public static partial class IcdEnvironment
	{
		public static string NewLine { get { return Environment.NewLine; } }

		public static DateTime GetLocalTime()
		{
			return DateTime.Now;
		}

		public static eRuntimeEnvironment RuntimeEnvironment { get { return eRuntimeEnvironment.Standard; } }

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
	}
}
#endif
