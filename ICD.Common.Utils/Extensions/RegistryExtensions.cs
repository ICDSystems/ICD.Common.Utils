#if NETSTANDARD
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using Microsoft.Win32;

namespace ICD.Common.Utils.Extensions
{
	public static class RegistryExtensions
	{
		/// <summary>
		/// Opens subkeys for the given registry key.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static IEnumerable<RegistryKey> OpenSubKeys([NotNull] this RegistryKey extends)
		{
			return extends.GetSubKeyNames()
			              .Select(extends.OpenSubKey)
			              .Where(k => k != null);
		}
	}
}

#endif
