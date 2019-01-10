using System;
using System.Collections.Generic;

namespace ICD.Common.Utils
{
	public static class UriUtils
	{
		private static readonly Dictionary<string, ushort> s_SchemeToPort =
			new Dictionary<string, ushort>(StringComparer.OrdinalIgnoreCase)
			{
				{Uri.UriSchemeHttp, 80},
				{Uri.UriSchemeHttps, 443}
			};

		/// <summary>
		/// Attempts to parse the given URI string into a System.Uri instance.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		public static bool TryParse(string uri, out Uri output)
		{
			output = null;

			if (string.IsNullOrEmpty(uri))
				return false;

			try
			{
				output = new Uri(uri);
				return true;
			}
			catch (UriFormatException)
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the port number for the given URI scheme.
		/// </summary>
		/// <param name="scheme"></param>
		/// <param name="port"></param>
		/// <returns></returns>
		public static bool TryGetPortForScheme(string scheme, out ushort port)
		{
			return s_SchemeToPort.TryGetValue(scheme, out port);
		}
	}
}
