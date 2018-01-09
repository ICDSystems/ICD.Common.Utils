using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
#if SIMPLSHARP
using System.Globalization;
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif

namespace ICD.Common.Utils
{
	public static class EnumUtils
	{
		private static readonly Dictionary<Type, IcdHashSet<object>> s_EnumValuesCache =
			new Dictionary<Type, IcdHashSet<object>>();

		/// <summary>
		/// Returns true if the given type is an enum.
		/// </summary>
		/// <returns></returns>
		public static bool IsEnumType(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return type.IsAssignableTo(typeof(Enum)) || type
#if !SIMPLSHARP
				                                            .GetTypeInfo()
#endif
				                                            .IsEnum;
		}

		/// <summary>
		/// Returns true if the given type is an enum.
		/// </summary>
		/// <returns></returns>
		public static bool IsEnumType<T>()
		{
			return IsEnumType(typeof(T));
		}

		/// <summary>
		/// Returns true if the given value is an enum.
		/// </summary>
		/// <returns></returns>
		public static bool IsEnum(object value)
		{
			return value != null && IsEnumType(value.GetType());
		}

		/// <summary>
		/// Returns true if the given value is defined as part of the given enum type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsDefined<T>(T value)
		{
			if (!IsEnumType(typeof(T)))
				throw new InvalidOperationException(string.Format("{0} is not an enum", typeof(T).Name));

			if (!IsFlagsEnum<T>())
				return GetValues<T>().Any(v => v.Equals(value));

			int valueInt = (int)GetUnderlyingValue(value);

			// Check if all of the flag values are defined
			foreach (T flag in GetFlags(value))
			{
				int flagInt = (int)GetUnderlyingValue(flag);
				valueInt = valueInt - flagInt;
			}

			return valueInt == 0;
		}

		#region Values

		/// <summary>
		/// Gets the underlying value of the enum.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static object GetUnderlyingValue<T>(T value)
		{
			if (!IsEnumType(typeof(T)))
				throw new InvalidOperationException(string.Format("{0} is not an enum", typeof(T).Name));

#if SIMPLSHARP
			return Convert.ChangeType(value, ToEnum(value).GetTypeCode(), CultureInfo.InvariantCulture);
#else
            return Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()));
#endif
		}

		/// <summary>
		/// Gets the values from an enumeration.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetValues<T>()
		{
			return GetValues(typeof(T)).Cast<T>();
		}

		/// <summary>
		/// Gets the values from an enumeration.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IEnumerable<object> GetValues(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			// Reflection is slow and this method is called a lot, so we cache the results.
			if (!s_EnumValuesCache.ContainsKey(type))
				s_EnumValuesCache[type] = GetValuesUncached(type).ToIcdHashSet();

			return s_EnumValuesCache[type];
		}

		/// <summary>
		/// Gets the values from an enumeration without performing any caching. This is slow because of reflection.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static IEnumerable<object> GetValuesUncached(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (!IsEnumType(type))
				throw new InvalidOperationException(string.Format("{0} is not an enum", type.Name));

			return type
#if SIMPLSHARP
				.GetCType()
#else
				.GetTypeInfo()
#endif
				.GetFields(BindingFlags.Static | BindingFlags.Public)
				.Select(x => x.GetValue(null));
		}

		/// <summary>
		/// Gets the 0 value for the given enum type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T GetNoneValue<T>()
		{
			if (!IsEnumType(typeof(T)))
				throw new InvalidOperationException(string.Format("{0} is not an enum", typeof(T).Name));

			return (T)(object)0;
		}

		/// <summary>
		/// Gets the values from an enumeration except the 0 value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetValuesExceptNone<T>()
		{
			if (!IsEnumType(typeof(T)))
				throw new InvalidOperationException(string.Format("{0} is not an enum", typeof(T).Name));

			return GetValuesExceptNone(typeof(T)).Cast<T>();
		}

		/// <summary>
		/// Gets the values from an enumeration except the 0 value.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IEnumerable<object> GetValuesExceptNone(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (!IsEnumType(type))
				throw new InvalidOperationException(string.Format("{0} is not an enum", type.Name));

			return GetValues(type).Where(v => (int)v != 0);
		}

		#endregion

		#region Flags

		/// <summary>
		/// Returns true if the given enum type has the Flags attribute set.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static bool IsFlagsEnum<T>()
		{
			if (!IsEnumType<T>())
				throw new ArgumentException(string.Format("{0} is not an enum", typeof(T).Name));

			return IsFlagsEnum(typeof(T));
		}

		/// <summary>
		/// Returns true if the given enum type has the Flags attribute set.
		/// </summary>
		/// <returns></returns>
		public static bool IsFlagsEnum(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (!IsEnumType(type))
				throw new InvalidOperationException(string.Format("{0} is not an enum", type.Name));

			return type
#if !SIMPLSHARP
                .GetTypeInfo()
#endif
				.IsDefined(typeof(FlagsAttribute), false);
		}

		/// <summary>
		/// Gets the overlapping values of the given enum flags.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="values"></param>
		/// <returns></returns>
		public static T GetFlagsIntersection<T>(params T[] values)
		{
			if (values.Length == 0)
				return GetNoneValue<T>();

			int output = (int)(object)values.First();
			foreach (T item in values.Skip(1))
				output &= (int)(object)item;

			return (T)Enum.ToObject(typeof(T), output);
		}

		/// <summary>
		/// Gets all of the set flags on the given enum.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetFlags<T>(T value)
		{
			if (!IsEnum(value))
// ReSharper disable once CompareNonConstrainedGenericWithNull
				throw new ArgumentException(string.Format("{0} is not an enum", value == null ? "NULL" : value.GetType().Name), "value");

			return GetValues<T>().Where(e => HasFlag(value, e));
		}

		/// <summary>
		/// Gets all of the set flags on the given enum except 0.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetFlagsExceptNone<T>(T value)
		{
			if (!IsEnum(value))
// ReSharper disable once CompareNonConstrainedGenericWithNull
				throw new ArgumentException(string.Format("{0} is not an enum", value == null ? "NULL" : value.GetType().Name), "value");

			T none = GetNoneValue<T>();
			return GetFlags(value).Except(none);
		}

		/// <summary>
		/// Gets an enum value of the given type with every flag set.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T GetFlagsAllValue<T>()
		{
			if (!IsEnumType<T>())
				throw new ArgumentException(string.Format("{0} is not an enum", typeof(T).Name));

			int output = GetValues<T>().Aggregate(0, (current, value) => current | (int)(object)value);
			return (T)Enum.ToObject(typeof(T), output);
		}

		/// <summary>
		/// Returns true if the enum contains the given flag.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="flag"></param>
		/// <returns></returns>
		public static bool HasFlag<T>(T value, T flag)
		{
			if (!IsEnum(value))
// ReSharper disable once CompareNonConstrainedGenericWithNull
				throw new ArgumentException(string.Format("{0} is not an enum", value == null ? "NULL" : value.GetType().Name), "value");

			if (!IsEnum(flag))
// ReSharper disable once CompareNonConstrainedGenericWithNull
				throw new ArgumentException(string.Format("{0} is not an enum", flag == null ? "NULL" : flag.GetType().Name), "flag");

			return ToEnum(value).HasFlag(ToEnum(flag));
		}

		/// <summary>
		/// Returns true if the enum contains all of the given flags.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		public static bool HasFlags<T>(T value, T flags)
		{
			if (!IsEnum(value))
// ReSharper disable once CompareNonConstrainedGenericWithNull
				throw new ArgumentException(string.Format("{0} is not an enum", value == null ? "NULL" : value.GetType().Name), "value");

			if (!IsEnum(flags))
// ReSharper disable once CompareNonConstrainedGenericWithNull
				throw new ArgumentException(string.Format("{0} is not an enum", flags == null ? "NULL" : flags.GetType().Name), "flags");

			return ToEnum(value).HasFlags(ToEnum(flags));
		}

		/// <summary>
		/// Returns true if only a single flag is set on the given enum value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool HasSingleFlag<T>(T value)
		{
			if (!IsEnum(value))
// ReSharper disable once CompareNonConstrainedGenericWithNull
				throw new ArgumentException(string.Format("{0} is not an enum", value == null ? "NULL" : value.GetType().Name), "value");

			return (int)(object)value != (int)(object)GetNoneValue<T>() && !HasMultipleFlags(value);
		}

		/// <summary>
		/// Returns true if the enum has more than 1 flag set.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool HasMultipleFlags<T>(T value)
		{
			if (!IsEnum(value))
// ReSharper disable once CompareNonConstrainedGenericWithNull
				throw new ArgumentException(string.Format("{0} is not an enum", value == null ? "NULL" : value.GetType().Name), "value");

			return HasMultipleFlags((int)(object)value);
		}

		/// <summary>
		/// Returns true if the enum has more than 1 flag set.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool HasMultipleFlags(int value)
		{
			return ((value & (value - 1)) != 0);
		}

		/// <summary>
		/// Returns true if the enum contains any flags.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool HasAnyFlags<T>(T value)
		{
			return GetFlagsExceptNone(value).Any();
		}

		/// <summary>
		/// Returns true if the enum contains any of the given flag values.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool HasAnyFlags<T>(T value, T other)
		{
			T intersection = GetFlagsIntersection(value, other);
			return HasAnyFlags(intersection);
		}

		#endregion

		#region Conversion

		/// <summary>
		/// Shorthand for parsing string to enum.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data"></param>
		/// <param name="ignoreCase"></param>
		/// <returns></returns>
		public static T Parse<T>(string data, bool ignoreCase)
		{
			if (!IsEnumType<T>())
				throw new ArgumentException(string.Format("{0} is not an enum", typeof(T).Name));

			T output;
			if (TryParse(data, ignoreCase, out output))
				return output;

			string message = string.Format("Failed to parse {0} as {1}", StringUtils.ToRepresentation(data), typeof(T).Name);
			throw new FormatException(message);
		}

		/// <summary>
		/// Shorthand for parsing a string to enum. Returns false if the parse failed.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data"></param>
		/// <param name="ignoreCase"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public static bool TryParse<T>(string data, bool ignoreCase, out T result)
		{
			if (!IsEnumType<T>())
				throw new ArgumentException(string.Format("{0} is not an enum", typeof(T).Name));

			result = default(T);

			try
			{
				result = (T)Enum.Parse(typeof(T), data, ignoreCase);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Shorthand for parsing string to enum.
		/// Will fail if the resulting value is not defined as part of the enum.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data"></param>
		/// <param name="ignoreCase"></param>
		/// <returns></returns>
		public static T ParseStrict<T>(string data, bool ignoreCase)
		{
			if (!IsEnumType<T>())
				throw new ArgumentException(string.Format("{0} is not an enum", typeof(T).Name));

			T output;

			try
			{
				output = Parse<T>(data, ignoreCase);
			}
			catch (Exception e)
			{
				throw new FormatException(
					string.Format("Failed to parse {0} as {1}", StringUtils.ToRepresentation(data), typeof(T).Name), e);
			}

			if (!IsDefined(output))
				throw new ArgumentOutOfRangeException(string.Format("{0} is not a valid {1}", output, typeof(T).Name));

			return output;
		}

		/// <summary>
		/// Shorthand for parsing a string to enum. Returns false if the parse failed.
		/// Will fail if the resulting value is not defined as part of the enum.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data"></param>
		/// <param name="ignoreCase"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public static bool TryParseStrict<T>(string data, bool ignoreCase, out T result)
		{
			if (!IsEnumType<T>())
				throw new ArgumentException(string.Format("{0} is not an enum", typeof(T).Name));

			result = default(T);

			try
			{
				result = ParseStrict<T>(data, ignoreCase);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Converts the given enum value to an Enum.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Enum ToEnum<T>(T value)
		{
			if (!IsEnum(value))
// ReSharper disable once CompareNonConstrainedGenericWithNull
				throw new ArgumentException(string.Format("{0} is not an enum", value == null ? "NULL" : value.GetType().Name), "value");

			return ToEnum((object)value);
		}

		/// <summary>
		/// Converts the given enum value to an Enum.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Enum ToEnum(object value)
		{
			return (Enum)value;
		}

		#endregion
	}
}
