using System;

namespace ICD.Common.Utils
{
	public static class UriUtils
	{
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
	}
}
