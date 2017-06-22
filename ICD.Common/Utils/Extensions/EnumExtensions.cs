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
		public static bool HasFlag(this Enum extends, Enum value)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (value == null)
				throw new ArgumentNullException("value");

			if (EnumUtils.HasMultipleFlags(value))
				throw new ArgumentException("Value has multiple flags", "value");

			return extends.HasFlags(value);
		}

		/// <summary>
		/// Check to see if a flags enumeration has all of the given flags set.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool HasFlags(this Enum extends, Enum value)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (value == null)
				throw new ArgumentNullException("value");

			// Not as good as the .NET 4 version of this function, but should be good enough
			if (extends.GetType() != value.GetType())
			{
				string message = string.Format("Enumeration type mismatch.  The flag is of type '{0}', was expecting '{1}'.",
				                               value.GetType(), extends.GetType());
				throw new ArgumentException(message);
			}

			ulong num = Convert.ToUInt64(value);
			return (Convert.ToUInt64(extends) & num) == num;
		}

		/// <summary>
		/// Casts the enum to the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static T Cast<T>(this Enum extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return (T)Enum.ToObject(typeof(T), extends);
		}
	}
}
