using System;
using System.Text;
using ICD.Common.Properties;

namespace ICD.Common.Utils
{
	public static class EncodingUtils
	{
		/// <summary>
		/// Strips leading Byte Order Mark characters from the given UTF8 data.
		/// </summary>
		/// <param name="data">Input string to remove leading BOM chars from.</param>
		/// <returns>Input string with leading BOM chars removed.</returns>
		/// <exception cref="ArgumentNullException">Data is null.</exception>
		[PublicAPI]
		public static string StripUtf8Bom(string data)
		{
			if (data == null)
				throw new ArgumentNullException("data");

			byte[] preamble = Encoding.UTF8.GetPreamble();
			string preambleString = Encoding.UTF8.GetString(preamble, 0, preamble.Length);

			if (data.StartsWith(preambleString, StringComparison.Ordinal))
				data = data.Remove(0, preambleString.Length);

			return data;
		}
	}
}
