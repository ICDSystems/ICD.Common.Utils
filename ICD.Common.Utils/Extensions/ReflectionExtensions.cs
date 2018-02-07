using System;
using System.Collections.Generic;
using System.Linq;
#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif

namespace ICD.Common.Utils.Extensions
{
	/// <summary>
	/// Extension methods for use with reflection objects.
	/// </summary>
	public static class ReflectionExtensions
	{
		/// <summary>
		/// Returns the custom attributes attached to the member.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetCustomAttributes<T>(this ICustomAttributeProvider extends)
			where T : Attribute
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.GetCustomAttributes<T>(false);
		}

		/// <summary>
		/// Returns the custom attributes attached to the member.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="inherits"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetCustomAttributes<T>(this ICustomAttributeProvider extends, bool inherits)
			where T : Attribute
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.GetCustomAttributes(typeof(T), inherits).Cast<T>();
		}

		/// <summary>
		/// Returns the custom attribute attached to the member.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static T GetCustomAttribute<T>(this ICustomAttributeProvider extends)
			where T : Attribute
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.GetCustomAttribute<T>(false);
		}

		/// <summary>
		/// Returns the custom attribute attached to the member.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="inherits"></param>
		/// <returns></returns>
		public static T GetCustomAttribute<T>(this ICustomAttributeProvider extends, bool inherits)
			where T : Attribute
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.GetCustomAttributes<T>(inherits).First();
		}
	}
}
