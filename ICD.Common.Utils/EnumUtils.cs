using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif

namespace ICD.Common.Utils
{
	public static class EnumUtils
	{
		private static readonly Dictionary<Type, object[]> s_EnumValuesCache;
		private static readonly SafeCriticalSection s_EnumValuesCacheSection;

		private static readonly Dictionary<Type, Dictionary<object, object[]>> s_EnumFlagsCache;
		private static readonly SafeCriticalSection s_EnumFlagsCacheSection;

		/// <summary>
		/// Static constructor.
		/// </summary>
		static EnumUtils()
		{
			s_EnumValuesCache = new Dictionary<Type, object[]>();
			s_EnumValuesCacheSection = new SafeCriticalSection();
			s_EnumFlagsCache = new Dictionary<Type, Dictionary<object, object[]>>();
			s_EnumFlagsCacheSection = new SafeCriticalSection();
		}

		#region Validation

		/// <summary>
		/// Returns true if the given type is an enum.
		/// </summary>
		/// <returns></returns>
		public static bool IsEnumType<T>()
		{
			return IsEnumType(typeof(T));
		}

		/// <summary>
		/// Returns true if the given type is an enum.
		/// </summary>
		/// <returns></returns>
		public static bool IsEnumType(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return type
#if !SIMPLSHARP
				       .GetTypeInfo()
#endif
				       .IsEnum || type.IsAssignableTo(typeof(Enum));
		}

		/// <summary>
		/// Returns true if the given value is an enum.
		/// </summary>
		/// <returns></returns>
		public static bool IsEnum<T>(T value)
		{
			// ReSharper disable once CompareNonConstrainedGenericWithNull
			return value != null && IsEnumType(value.GetType());
		}

		/// <summary>
		/// Returns true if the given enum type has the Flags attribute set.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static bool IsFlagsEnum<T>()
		{
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

			return type
#if !SIMPLSHARP
                .GetTypeInfo()
#endif
				.IsDefined(typeof(FlagsAttribute), false);
		}

		/// <summary>
		/// Returns true if the given value is defined as part of the given enum type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsDefined<T>(T value)
			where T : struct, IConvertible
		{
			T all = GetFlagsAllValue<T>();
			return HasFlags(all, value);
		}

		/// <summary>
		/// Returns true if the given value is defined as part of the given enum type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsDefined(Type type, int value)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (!IsEnumType(type))
				throw new InvalidOperationException(string.Format("{0} is not an enum", type.Name));

			int all = GetFlagsAllValue(type);
			return HasFlags(all, value);
		}

		#endregion

		#region Values

		/// <summary>
		/// Gets the names of the values in the enumeration.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IEnumerable<string> GetNames<T>()
			where T : struct, IConvertible
		{
			return GetNames(typeof(T));
		}

		/// <summary>
		/// Gets the names of the values in the enumeration.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IEnumerable<string> GetNames([NotNull] Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return GetValues(type).Select(v => v.ToString());
		}

		/// <summary>
		/// Gets the values from an enumeration.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetValues<T>()
			where T : struct, IConvertible
		{
			return GetValues(typeof(T)).Cast<T>();
		}

		/// <summary>
		/// Gets the values from an enumeration.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<object> GetValues(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			// Reflection is slow and this method is called a lot, so we cache the results.
			return s_EnumValuesCacheSection.Execute(() => s_EnumValuesCache.GetOrAddNew(type, () => GetValuesUncached(type).ToArray()));
		}

		/// <summary>
		/// Gets the values from an enumeration except the 0 value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetValuesExceptNone<T>()
			where T : struct, IConvertible
		{
			return GetValuesExceptNone(typeof(T)).Cast<T>();
		}

		/// <summary>
		/// Gets the values from an enumeration except the 0 value.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IEnumerable<object> GetValuesExceptNone([NotNull] Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return GetValues(type).Where(v => (int)v != 0);
		}

		/// <summary>
		/// Gets the values from an enumeration without performing any caching. This is slow because of reflection.
		/// </summary>
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

		#endregion

		#region Flags

		/// <summary>
		/// Excludes b from a.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static T ExcludeFlags<T>(T a, T b)
			where T : struct, IConvertible
		{
			int aInt = (int)(object)a;
			int bInt = (int)(object)b;

			return (T)(object)ExcludeFlags(aInt, bInt);
		}

		/// <summary>
		/// Excludes b from a.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static int ExcludeFlags(int a, int b)
		{
			return a & ~b;
		}

		/// <summary>
		/// Includes a and b.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static T IncludeFlags<T>(T a, T b)
			where T : struct, IConvertible
		{
			int aInt = (int)(object)a;
			int bInt = (int)(object)b;

			return (T)(object)IncludeFlags(aInt, bInt);
		}

		/// <summary>
		/// Includes a and b.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static int IncludeFlags(int a, int b)
		{
			return a | b;
		}

		/// <summary>
		/// Gets the overlapping values of the given enum flags.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="values"></param>
		/// <returns></returns>
		public static T GetFlagsIntersection<T>(params T[] values)
			where T : struct, IConvertible
		{
			if (values == null)
				throw new ArgumentNullException("values");

			if (values.Length == 0)
				return default(T);

			int output = 0;
			bool first = true;

			foreach (T value in values)
			{
				if (first)
				{
					output = (int)(object)value;
					first = false;
				}
				else
				{
					output &= (int)(object)value;
				}

				if (output == 0)
					return default(T);
			}

			return (T)(object)output;
		}

		/// <summary>
		/// Gets the overlapping values of the given enum flags.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static T GetFlagsIntersection<T>(T a, T b)
			where T : struct, IConvertible
		{
			int aInt = (int)(object)a;
			int bInt = (int)(object)b;

			return (T)(object)(aInt & bInt);
		}

		/// <summary>
		/// Gets all of the set flags on the given enum.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetFlags<T>(T value)
			where T : struct, IConvertible
		{
			return GetFlags(typeof(T), value).Cast<T>();
		}

		/// <summary>
		/// Gets all of the set flags on the given enum.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static IEnumerable<object> GetFlags(Type type, object value)
		{
			return s_EnumFlagsCacheSection.Execute(() => s_EnumFlagsCache
			                                             .GetOrAddNew(type, () => new Dictionary<object, object[]>())
			                                             .GetOrAddNew(value, () => GetValues(type)
				                                                          .Where(f => !HasMultipleFlags((int)f) && HasFlag(value, f))
				                                                          .ToArray()));
		}

		/// <summary>
		/// Gets all of the set flags on the given enum type except 0.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetFlagsExceptNone<T>()
			where T : struct, IConvertible
		{
			return GetFlagsExceptNone(typeof(T)).Cast<T>();
		}

		/// <summary>
		/// Gets all of the set flags on the given enum except 0.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetFlagsExceptNone<T>(T value)
			where T : struct, IConvertible
		{
			return GetFlagsExceptNone(typeof(T), value).Cast<T>();
		}

		/// <summary>
		/// Gets all of the set flags on the given enum type except 0.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IEnumerable<object> GetFlagsExceptNone([NotNull] Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			object allValue = GetFlagsAllValue(type);
			return GetFlagsExceptNone(type, allValue);
		}

		/// <summary>
		/// Gets all of the set flags on the given enum except 0.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static IEnumerable<object> GetFlagsExceptNone([NotNull] Type type, object value)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return GetFlags(type, value).Where(f => (int)f != 0);
		}

		/// <summary>
		/// Gets all of the flag combinations on the given flag enum value.
		/// 
		/// IE: If you have an enum type with flags{a, b, c}, and you pass this method {a|b},
		/// It will return {a, b, a|b}
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetAllFlagCombinationsExceptNone<T>(T value)
			where T : struct, IConvertible
		{
			int maxEnumValue = (GetValues<T>().Max(v => (int)(object)v) * 2) -1;
			return Enumerable.Range(1, maxEnumValue).Select(i => (T)(object)i ).Where(v => HasFlags(value, v));
		}

		/// <summary>
		/// Gets an enum value of the given type with every flag set.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T GetFlagsAllValue<T>()
			where T : struct, IConvertible
		{
			int output = GetValues<T>().Aggregate(0, (current, value) => current | (int)(object)value);
			return (T)Enum.ToObject(typeof(T), output);
		}

		/// <summary>
		/// Gets an enum value of the given type with every flag set.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static int GetFlagsAllValue(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return GetValues(type).Aggregate(0, (current, value) => current | (int)value);
		}
		
		/// <summary>
		/// Gets an enum value of the given type with the inverse of the flags set
		/// </summary>
		/// <param name="value"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T GetInverseFlags<T>(T value)
			where T : struct, IConvertible
		{
			int output = GetFlagsAllValue(typeof(T)) & ~(int)(object)value;
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
			where T : struct, IConvertible
		{
			return HasFlags(value, flag);
		}

		/// <summary>
		/// Returns true if the enum contains the given flag.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="flag"></param>
		/// <returns></returns>
		public static bool HasFlag(object value, object flag)
		{
			return HasFlag((int)value, (int)flag);
		}

		/// <summary>
		/// Returns true if the enum contains the given flag.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="flag"></param>
		/// <returns></returns>
		public static bool HasFlag(int value, int flag)
		{
			return HasFlags(value, flag);
		}

		/// <summary>
		/// Returns true if the enum contains all of the given flags.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		public static bool HasFlags<T>(T value, T flags)
			where T : struct, IConvertible
		{
			int a = (int)(object)value;
			int b = (int)(object)flags;

			return HasFlags(a, b);
		}

		/// <summary>
		/// Returns true if the enum contains the given flag.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		public static bool HasFlags(int value, int flags)
		{
			return (value & flags) == flags;
		}

		/// <summary>
		/// Returns true if only a single flag is set on the given enum value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool HasSingleFlag<T>(T value)
			where T : struct, IConvertible
		{
			int numeric = (int)(object)value;

			return HasAnyFlags(numeric) && !HasMultipleFlags(numeric);
		}

		/// <summary>
		/// Returns true if the enum has more than 1 flag set.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool HasMultipleFlags<T>(T value)
			where T : struct, IConvertible
		{
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
			return (value & (value - 1)) != 0;
		}

		/// <summary>
		/// Returns true if the enum contains any flags.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool HasAnyFlags<T>(T value)
			where T : struct, IConvertible
		{
			return HasAnyFlags((int)(object)value);
		}

		/// <summary>
		/// Returns true if the enum has any flags set.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool HasAnyFlags(int value)
		{
			return value > 0;
		}

		/// <summary>
		/// Returns true if the enum contains any of the given flag values.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool HasAnyFlags<T>(T value, T other)
			where T : struct, IConvertible
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
			where T : struct, IConvertible
		{
			T output;
			if (TryParse(data, ignoreCase, out output))
				return output;

			string message = string.Format("Failed to parse {0} as {1}", StringUtils.ToRepresentation(data), typeof(T).Name);
			throw new FormatException(message);
		}

		/// <summary>
		/// Shorthand for parsing string to enum.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="data"></param>
		/// <param name="ignoreCase"></param>
		/// <returns></returns>
		public static int Parse(Type type, string data, bool ignoreCase)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			int output;
			if (TryParse(type, data, ignoreCase, out output))
				return output;

			string message = string.Format("Failed to parse {0} as {1}", StringUtils.ToRepresentation(data), type.Name);
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
			where T : struct, IConvertible
		{
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
		/// Shorthand for parsing a string to enum. Returns false if the parse failed.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="data"></param>
		/// <param name="ignoreCase"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public static bool TryParse(Type type, string data, bool ignoreCase, out int result)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			result = 0;

			try
			{
				result = (int)Enum.Parse(type, data, ignoreCase);
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
			where T : struct, IConvertible
		{
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
		/// Shorthand for parsing string to enum.
		/// Will fail if the resulting value is not defined as part of the enum.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="data"></param>
		/// <param name="ignoreCase"></param>
		/// <returns></returns>
		public static int ParseStrict(Type type, string data, bool ignoreCase)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			int output;

			try
			{
				output = Parse(type, data, ignoreCase);
			}
			catch (Exception e)
			{
				throw new FormatException(
					string.Format("Failed to parse {0} as {1}", StringUtils.ToRepresentation(data), type.Name), e);
			}

			if (!IsDefined(type, output))
				throw new ArgumentOutOfRangeException(string.Format("{0} is not a valid {1}", output, type.Name));

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
			where T : struct, IConvertible
		{
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

		#endregion

		#region Formatting

		/// <summary>
		/// Builds a comma delimited string of the defined enum flags, followed by the numeric remainder.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToStringUndefined<T>(T value)
			where T : struct, IConvertible
		{
			if (!IsFlagsEnum<T>())
				return value.ToString();

			long remainder = (int)(object)value;

			StringBuilder output = new StringBuilder();
			string format = "{0}";

			foreach (T flag in GetFlagsExceptNone(value))
			{
				output.AppendFormat(format, flag);
				remainder -= (int)(object)flag;
				format = ", {0}";
			}

			if (remainder != 0)
				output.AppendFormat(", {0}", remainder);

			return output.ToString();
		}

		#endregion
	}
}
