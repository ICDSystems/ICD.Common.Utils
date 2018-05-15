using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
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

			try
			{
				return extends.GetCustomAttributes(typeof(T), inherits).Cast<T>();
			}
			// Crestron bug?
			catch (ArgumentNullException)
			{
				return Enumerable.Empty<T>();
			}
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

#if NETSTANDARD
		/// <summary>
		/// Returns the custom attributes attached to the member.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="inherits"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetCustomAttributesIncludingBaseInterfaces<T>(this Type extends)
			where T : Attribute
		{
			return extends.GetCustomAttributes<T>(true)
				.Union(extends.GetInterfaces()
					.SelectMany(interfaceType => interfaceType
						.GetCustomAttributes<T>(true)))
				.Distinct();
		}

		/// <summary>
		/// Returns the custom attributes attached to the member.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="inherits"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetCustomAttributesIncludingBaseInterfaces<T>(this MemberInfo extends)
			where T : Attribute
		{
			return extends.GetCustomAttributes<T>(true)
				.Union(extends.DeclaringType?
					       .GetInterfaces()
					       .SelectMany(interfaceType => interfaceType
						                                    .GetMember(
							                                    extends.Name,
							                                    extends.MemberType, 
							                                    BindingFlags.Instance)
						                                    .FirstOrDefault()?
						                                    .GetCustomAttributes<T>(true) ?? Enumerable.Empty<T>())?
					       .Except(null) ?? Enumerable.Empty<T>())
				.Distinct();
		}
#endif
	}
}
