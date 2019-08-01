using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif

namespace ICD.Common.Utils
{
	/// <summary>
	/// Utility methods for browsing code attributes.
	/// Provides some basic caching for faster subsequent searches.
	/// </summary>
	public static class AttributeUtils
	{
		/// <summary>
		/// Gets the first attribute on the given class type matching the generic type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <returns></returns>
		[CanBeNull]
		public static T GetClassAttribute<T>(Type type)
		{
			return GetClassAttribute<T>(type, false);
		}

		/// <summary>
		/// Gets the first attribute on the given class type matching the generic type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <param name="inherit"></param>
		/// <returns></returns>
		[CanBeNull]
		public static T GetClassAttribute<T>(Type type, bool inherit)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return GetClassAttributes<T>(type, inherit).FirstOrDefault();
		}

		/// <summary>
		/// Gets the attributes on the given class type matching the generic type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetClassAttributes<T>(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return GetClassAttributes<T>(type, false);
		}

		/// <summary>
		/// Gets the attributes on the given class type matching the generic type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <param name="inherit"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetClassAttributes<T>(Type type, bool inherit)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			// ReSharper disable InvokeAsExtensionMethod
			return ReflectionExtensions.GetCustomAttributes<T>(
#if SIMPLSHARP
			                                                   (CType)
#endif
															   type, inherit);
			// ReSharper restore InvokeAsExtensionMethod
		}

		/// <summary>
		/// Returns the properties on the given instance with property attributes of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <param name="inherit"></param>
		/// <returns></returns>
		public static IEnumerable<PropertyInfo> GetProperties<T>(object instance, bool inherit)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			return instance.GetType()
#if SIMPLSHARP
			               .GetCType()
#else
			               .GetTypeInfo()
#endif
			               .GetProperties()
// ReSharper disable InvokeAsExtensionMethod
			               .Where(p => ReflectionExtensions.GetCustomAttributes<T>(p, inherit).Any());
// ReSharper restore InvokeAsExtensionMethod
		}
	}
}
