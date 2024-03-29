﻿using System;
using System.Collections.Generic;
using System.Linq;
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
		/// Returns these enum flags, excluding the other enum flags.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		[PublicAPI]
		public static T ExcludeFlags<T>(this T extends, T other)
			where T : struct, IConvertible
		{
			return EnumUtils.ExcludeFlags(extends, other);
		}

		/// <summary>
		/// Returns these enum flags, including the other enum flags.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		[PublicAPI]
		public static T IncludeFlags<T>(this T extends, T other)
			where T : struct, IConvertible
		{
			return EnumUtils.IncludeFlags(extends, other);
		}

		/// <summary>
		/// Returns the enum, with the other flags set or unset
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="other"></param>
		/// <param name="set"></param>
		/// <returns></returns>
		public static T SetFlags<T>(this T extends, T other, bool set)
			where T : struct, IConvertible
		{
			return set ? 
				EnumUtils.IncludeFlags(extends, other) : 
				EnumUtils.ExcludeFlags(extends, other);
		}

		/// <summary>
		/// Returns the enum value as a ushort.
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

		/// <summary>
		/// Builds a comma delimited string of the defined enum flags, followed by the numeric remainder.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static string ToStringUndefined<T>(this T extends)
			where T : struct, IConvertible
		{
			return EnumUtils.ToStringUndefined(extends);
		}

		/// <summary>
		/// Returns the next defined enum value for the enum type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static T CycleNext<T>(this T extends)
			where T : struct, IConvertible
		{
			if (EnumUtils.IsFlagsEnum(typeof(T)) && !EnumUtils.HasSingleFlag(extends))
				throw new InvalidOperationException(string.Format("Cannot cycle enum with multiple flags - {0}", extends));

			IEnumerable<T> values = EnumUtils.GetValues<T>();
			IList<T> list = values as IList<T> ?? values.ToArray();

			int index = list.BinarySearch(extends);

			return index == list.Count - 1 ? list[0] : list[index + 1];
		}
	}
}
