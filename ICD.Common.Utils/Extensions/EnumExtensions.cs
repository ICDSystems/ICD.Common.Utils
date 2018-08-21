using System;
using ICD.Common.Properties;

namespace ICD.Common.Utils.Extensions
{
	public static class EnumExtensions
	{
		/// <summary>
		/// Check to see if a flags enumeration has a specific flag set.
		/// </summary>
		/// <param name="extends">Flags enumeration to check</param>
		/// <param name="value">Flag to check for</param>
		/// <returns></returns>
		[PublicAPI]
		public static bool HasFlag<T>(this T extends, T value)
			where T : struct, IConvertible
		{
			return EnumUtils.HasFlag(extends, value);
		}

		/// <summary>
		/// Check to see if a flags enumeration has all of the given flags set.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool HasFlags<T>(this T extends, T values)
			where T : struct, IConvertible
		{
			return EnumUtils.HasFlags(extends, values);
		}

		/// <summary>
		/// Returns the enum value as a 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI("S+")]
		public static ushort ToUShort<T>(this T extends)
			where T : struct, IConvertible
		{
			return (ushort)(object)extends;
		}
	}
}
