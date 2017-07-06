#if STANDARD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

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
        
        public static IEnumerable<string> NetworkAddresses
        {
            get
            {
				return NetworkInterface.GetAllNetworkInterfaces()
				  .Where(ni => ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
				  .SelectMany(ni => ni.GetIPProperties().UnicastAddresses
					  .Where(ua => ua.Address.AddressFamily == AddressFamily.InterNetwork)
					  .Select(ua => ua.Address.ToString()));
            }
        }
    }
}
#endif
