﻿using System;
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
		public static IEnumerable<T> GetCustomAttributes<T>([NotNull] this ICustomAttributeProvider extends)
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
		public static IEnumerable<T> GetCustomAttributes<T>([NotNull] this ICustomAttributeProvider extends,
		                                                    bool inherits)
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
		public static T GetCustomAttribute<T>([NotNull] this ICustomAttributeProvider extends)
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
		public static T GetCustomAttribute<T>([NotNull] this ICustomAttributeProvider extends, bool inherits)
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
		/// <returns></returns>
		public static IEnumerable<T> GetCustomAttributesIncludingBaseInterfaces<T>([NotNull] this Type extends)
			where T : Attribute
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

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
		/// <returns></returns>
		public static IEnumerable<T> GetCustomAttributesIncludingBaseInterfaces<T>([NotNull] this MemberInfo extends)
			where T : Attribute
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.GetCustomAttributes<T>(true)
			              .Union(extends.DeclaringType?
			                            .GetInterfaces()
			                            .SelectMany(interfaceType => interfaceType
			                                                         .GetMember(extends.Name,
			                                                                    extends.MemberType,
			                                                                    BindingFlags.Instance)
			                                                         .FirstOrDefault()?
			                                                         .GetCustomAttributes<T>(true) ??
			                                                         Enumerable.Empty<T>())?
			                            .Except(null) ?? Enumerable.Empty<T>())
			              .Distinct();
		}
#endif

#if SIMPLSHARP
		/// <summary>
		/// Returns the value of the property
		/// Only needed for SIMPLSHARP since newer .Net contains this already
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="obj"></param>
		/// <returns>The property value for the obj parameter.</returns>
		public static object GetValue([NotNull] this PropertyInfo extends, object obj)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.GetValue(obj, null);
		}

		/// <summary>
		/// Sets the value of the property
		/// Only needed for SIMPLSHARP since newer .Net contains this already
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="obj"></param>
		/// <param name="value"></param>
		public static void SetValue([NotNull] this PropertyInfo extends, object obj, object value)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			extends.SetValue(obj, value, null);
		}
#endif

		/// <summary>
		/// Sets the value of a property
		/// Traverses the path to access properties nested in other properties
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="value"></param>
		/// <param name="path"></param>
		/// <returns>true if property was set, false if property was not found</returns>
		public static bool SetProperty([NotNull] this object extends, [CanBeNull] object value, [NotNull] params string[] path)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (path == null)
				throw new ArgumentNullException("path");

			object instance;
			PropertyInfo property = extends.GetPropertyInfo(out instance, path);

			if (property == null)
				return false;

			property.SetValue(instance, value);
			return true;
		}

		/// <summary>
		/// Gets the PropertyInfo of the specified property
		/// Traverses the path to access properties nested in other properties
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="instance"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		[CanBeNull]
		public static PropertyInfo GetPropertyInfo([NotNull] this object extends, out object instance, [NotNull] params string[] path)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (path == null)
				throw new ArgumentNullException("path");

			instance = extends;

			// Grab properties until the last item in the path
			for (int i = 0; i < path.Length - 1; i++)
			{
				PropertyInfo info =
					instance.GetType()
#if SIMPLSHARP
					             .GetCType()
#endif
					             .GetProperty(path[i],
					                          BindingFlags.Instance |
					                          BindingFlags.Static |
					                          BindingFlags.Public |
					                          BindingFlags.NonPublic |
					                          BindingFlags.FlattenHierarchy);
				if (info == null)
				{
					instance = null;
					return null;
				}

				instance = info.GetValue(instance);
			}

			// Set the property to the value
			PropertyInfo output =
				instance.GetType()
#if SIMPLSHARP
			               .GetCType()
#endif
				        .GetProperty(path[path.Length - 1],
				                     BindingFlags.Instance |
				                     BindingFlags.Static |
				                     BindingFlags.Public |
				                     BindingFlags.NonPublic |
				                     BindingFlags.FlattenHierarchy);

			if (output == null)
				instance = null;
			return output;
		}

		/// <summary>
		/// Gets the value of a property
		/// Traverses the path to access properties nested in other properties
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="value"></param>
		/// <param name="path"></param>
		/// <returns>true if property get was successful, false if the property was not found</returns>
		public static bool GetProperty<T>([NotNull] this object extends, [CanBeNull] out T value, [NotNull] params string[] path)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (path == null)
				throw new ArgumentNullException("path");

			value = default(T);

			object instance;
			PropertyInfo property = extends.GetPropertyInfo(out instance, path);

			if (property == null)
				return false;

			value = (T)property.GetValue(instance);
			return true;
		}

		public static bool CallMethod([NotNull] this object extends, string methodName)
		{
			object value;
			return CallMethod(extends, methodName, out value, new object[] { });
		}

		public static bool CallMethod<T>([NotNull] this object extends, string methodName, [CanBeNull] out T value)
		{
			return CallMethod(extends, methodName, out value, new object[] { });
		}

		public static bool CallMethod([NotNull] this object extends, string methodName, [NotNull] params object[] parameters)
		{
			object value;
			return CallMethod(extends, methodName, out value, parameters);
		}

		public static bool CallMethod<T>([NotNull] this object extends, string methodName, [CanBeNull] out T value, [NotNull] params object[] parameters)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (parameters == null)
				throw new ArgumentNullException("parameters");

			value = default(T);

			MethodInfo method =
				extends.GetType()
#if SIMPLSHARP
				       .GetCType()
#endif
				       .GetMethod(methodName,
				                  BindingFlags.Instance |
				                  BindingFlags.Static |
				                  BindingFlags.Public |
				                  BindingFlags.NonPublic |
				                  BindingFlags.FlattenHierarchy);
			if (method == null)
				return false;

			value = (T)method.Invoke(extends, parameters);

			return true;
		}

		/// <summary>
		/// Gets the EventArgs Type for the given event.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static Type GetEventArgsType([NotNull] this EventInfo extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			Type eventHandlerType = extends.EventHandlerType;
			return eventHandlerType == typeof(EventHandler)
				       ? typeof(EventArgs)
				       : eventHandlerType.GetInnerGenericTypes(typeof(EventHandler<>))
				                         .Single();
		}
	}
}
