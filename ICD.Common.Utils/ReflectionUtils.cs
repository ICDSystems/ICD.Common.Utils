﻿using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.IO;
using MethodInfoExtensions = ICD.Common.Utils.Extensions.MethodInfoExtensions;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
using Crestron.SimplSharp.Reflection;
using Activator = Crestron.SimplSharp.Reflection.Activator;
#else
using System.Reflection;
using Activator = System.Activator;
#endif
#if NETSTANDARD
using Microsoft.Extensions.DependencyModel;
using System.Runtime.Loader;
#endif

namespace ICD.Common.Utils
{
	public static class ReflectionUtils
	{
		/// <summary>
		/// Returns true if the parameters match the constructor parameters.
		/// </summary>
		/// <param name="constructor"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static bool MatchesConstructorParameters([NotNull] ConstructorInfo constructor, [NotNull] IEnumerable<object> parameters)
		{
			if (constructor == null)
				throw new ArgumentNullException("constructor");

			if (parameters == null)
				throw new ArgumentNullException("parameters");

			return MatchesMethodParameters(constructor, parameters);
		}

		/// <summary>
		/// Returns true if the parameter values match the method signature parameters.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static bool MatchesMethodParameters([NotNull] MethodBase method, [NotNull] IEnumerable<object> parameters)
		{
			if (method == null)
				throw new ArgumentNullException("method");

			if (parameters == null)
				throw new ArgumentNullException("parameters");

			ParameterInfo[] methodParameters = method.GetParameters();

			// Bail early if the counts don't match
			ICollection<object> parametersCollection = parameters as ICollection<object>;
			if (parametersCollection != null && parametersCollection.Count != methodParameters.Length)
				return false;

			// Compare the parameters to the objects
			IEnumerable<Type> parameterTypes = methodParameters.Select(p => (Type)p.ParameterType);
			return ParametersMatchTypes(parameterTypes, parameters);
		}

		/// <summary>
		/// Returns true if the if the parameter match the property parameter.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public static bool MatchesPropertyParameter([NotNull] PropertyInfo property, object parameter)
		{
			if (property == null)
				throw new ArgumentNullException("property");

			return ParameterMatchesType(property.PropertyType, parameter);
		}

		/// <summary>
		/// Returns true if the parameters array is compatible with the given property types array.
		/// </summary>
		/// <param name="types"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		private static bool ParametersMatchTypes([NotNull] IEnumerable<Type> types, [NotNull] IEnumerable<object> parameters)
		{
			if (types == null)
				throw new ArgumentNullException("types");

			if (parameters == null)
				throw new ArgumentNullException("parameters");

			return types
#if SIMPLSHARP
				   .Cast<object>()
#endif
			       .SequenceEqual(parameters, (a, b) => ParameterMatchesType((Type)a, b));
		}

		/// <summary>
		/// Returns true if the parameter can be assigned to the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		private static bool ParameterMatchesType([NotNull] Type type, object parameter)
		{
			if (type == null)
				throw new ArgumentNullException("type");

#if SIMPLSHARP
			// Can the parameter be assigned a null value?
			if (parameter == null)
				return (type.IsClass || !type.IsValueType || Nullable.GetUnderlyingType(type) != null);
			return type.IsInstanceOfType(parameter);
#else
			TypeInfo info = type.GetTypeInfo();
			// Can the parameter be assigned a null value?
			if (parameter == null)
				return (info.IsClass || !info.IsValueType || Nullable.GetUnderlyingType(type) != null);
			return info.IsInstanceOfType(parameter);
#endif
		}

		/// <summary>
		/// Same as doing default(Type).
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		[CanBeNull]
		public static object GetDefaultValue([NotNull] Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return type
#if !SIMPLSHARP
				       .GetTypeInfo()
#endif
				       .IsValueType
				       ? Activator.CreateInstance(type)
				       : null;
		}

		/// <summary>
		/// Platform independent delegate instantiation.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="firstArgument"></param>
		/// <param name="method"></param>
		/// <returns></returns>
		[NotNull]
		public static Delegate CreateDelegate([NotNull] Type type, object firstArgument, [NotNull] MethodInfo method)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (method == null)
				throw new ArgumentNullException("method");

			return
#if SIMPLSHARP
				CDelegate
#else
				Delegate
#endif
					.CreateDelegate(type, firstArgument, method);
		}

		/// <summary>
		/// Creates an instance of the given type, calling the default constructor.
		/// </summary>
		/// <returns></returns>
		[NotNull]
		public static T CreateInstance<T>([NotNull] Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return (T)CreateInstance(type, new object[0]);
		}

		/// <summary>
		/// Creates an instance of the given type, calling the default constructor.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[NotNull]
		public static T CreateInstance<T>(params object[] parameters)
		{
			if (parameters == null)
				throw new ArgumentNullException("parameters");

			return (T)CreateInstance(typeof(T), parameters);
		}

		/// <summary>
		/// Creates an instance of the given type, calling the default constructor.
		/// </summary>
		/// <returns></returns>
		[NotNull]
		public static object CreateInstance([NotNull] Type type, params object[] parameters)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (parameters == null)
				throw new ArgumentNullException("parameters");
			try
			{
				if (parameters.Length == 0)
					return Activator.CreateInstance(type);

				ConstructorInfo constructor = GetConstructor(type, parameters);
				return constructor.Invoke(parameters);
			}
			catch (Exception e)
			{
				// Crestron sandbox doesn't let us reference System.Reflection types...
				switch (e.GetType().Name)
				{
					case "TypeLoadException":
					case "TargetInvocationException":
						throw e.GetBaseException();
				}

				throw;
			}
		}

		/// <summary>
		/// Gets the constructor matching the given parameters.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		[NotNull]
		public static ConstructorInfo GetConstructor([NotNull] Type type, params object[] parameters)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (parameters == null)
				throw new ArgumentNullException("parameters");

			ConstructorInfo info;
			
			if(!type
#if SIMPLSHARP
				.GetCType()
#endif
				.GetConstructors()
				.Where(c => MatchesConstructorParameters(c, parameters))
				.TryFirst(out info))
				throw new ArgumentException("Couldn't find a constructor matching the given parameters.");

			return info;
		}

		/// <summary>
		/// Loads the assembly at the given path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		[NotNull]
		public static Assembly LoadAssemblyFromPath([NotNull] string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			if (string.IsNullOrEmpty(path))
				throw new ArgumentException("Path is empty", "path");

			string fileNameWithOutExtension = IcdPath.GetFileNameWithoutExtension(path);

#if !NETSTANDARD
			try
			{
				return Assembly.Load(new AssemblyName {Name = fileNameWithOutExtension});
			}
			catch (Exception)
			{
				return Assembly.LoadFrom(path);
			}
#else
			bool inCompileLibraries = DependencyContext.Default.CompileLibraries.Any(l => l.Name.Equals(fileNameWithOutExtension, StringComparison.OrdinalIgnoreCase));
			bool inRuntimeLibraries = DependencyContext.Default.RuntimeLibraries.Any(l => l.Name.Equals(fileNameWithOutExtension, StringComparison.OrdinalIgnoreCase));

			return inCompileLibraries || inRuntimeLibraries
				? Assembly.Load(new AssemblyName(fileNameWithOutExtension))
				: AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
#endif
		}

		/// <summary>
		/// Changes the given value to the given type.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		[CanBeNull]
		public static object ChangeType(object value, [NotNull] Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			// Handle null value
			if (value == null)
			{
				if (type.CanBeNull())
					return null;

				throw new InvalidCastException(string.Format("Unable to convert NULL to type {0}", type.Name));
			}

			Type valueType = value.GetType();
			if (valueType.IsAssignableTo(type))
				return value;

			try
			{
				// Handle enum
				if (type.IsEnum)
				{
					if (valueType.IsIntegerNumeric())
						return Enum.ToObject(type, value);

					string valueAsString = value as string;
					if (valueAsString != null)
						return Enum.Parse(type, valueAsString, false);
				}

				return Convert.ChangeType(value, type, null);
			}
			catch (Exception e)
			{
				string valueString = valueType.ToString();
				string message = string.Format("Failed to convert {0} to type {1} - {2}", valueString, type, e.Message);
				throw new InvalidCastException(message, e);
			}
		}

		/// <summary>
		/// Subscribes to the event on the given instance using the event handler.
		/// </summary>
		/// <param name="instance">The instance with the event. Null for static types.</param>
		/// <param name="eventInfo">The EventInfo for the event.</param>
		/// <param name="eventHandler">The EventHandler for the callback.</param>
		/// <returns></returns>
		[NotNull]
		public static Delegate SubscribeEvent(object instance, [NotNull] EventInfo eventInfo, [NotNull] Action<object> eventHandler)
		{
			if (eventInfo == null)
				throw new ArgumentNullException("eventInfo");

			if (eventHandler == null)
				throw new ArgumentNullException("eventHandler");

			object handler = eventHandler.Target;
// ReSharper disable InvokeAsExtensionMethod
			MethodInfo callback = MethodInfoExtensions.GetMethodInfo(eventHandler);
// ReSharper restore InvokeAsExtensionMethod

			return SubscribeEvent(instance, eventInfo, handler, callback);
		}

		/// <summary>
		/// Subscribes to the event on the given instance using the event handler.
		/// </summary>
		/// <param name="instance">The instance with the event. Null for static types.</param>
		/// <param name="eventInfo">The EventInfo for the event.</param>
		/// <param name="eventHandler">The EventHandler for the callback.</param>
		/// <returns></returns>
		[NotNull]
		public static Delegate SubscribeEvent<T>(object instance, [NotNull] EventInfo eventInfo, [NotNull] Action<object, T> eventHandler)
		{
			if (eventInfo == null)
				throw new ArgumentNullException("eventInfo");

			if (eventHandler == null)
				throw new ArgumentNullException("eventHandler");

			object handler = eventHandler.Target;
// ReSharper disable InvokeAsExtensionMethod
			MethodInfo callback = MethodInfoExtensions.GetMethodInfo(eventHandler);
// ReSharper restore InvokeAsExtensionMethod

			return SubscribeEvent(instance, eventInfo, handler, callback);
		}

		/// <summary>
		/// Subscribes to the event on the given instance using the handler and eventHandler method.
		/// </summary>
		/// <param name="instance">The instance with the event. Null for static types.</param>
		/// <param name="eventInfo">The EventInfo for the event.</param>
		/// <param name="handler">The instance with the eventHandler MethodInfo. Null for static types.</param>
		/// <param name="callback">The MethodInfo for the eventHandler method.</param>
		/// <returns></returns>
		[NotNull]
		public static Delegate SubscribeEvent(object instance, [NotNull] EventInfo eventInfo, object handler, [NotNull] MethodInfo callback)
		{
			if (eventInfo == null)
				throw new ArgumentNullException("eventInfo");

			if (callback == null)
				throw new ArgumentNullException("callback");

			Delegate output = CreateDelegate(eventInfo.EventHandlerType, handler, callback);
			eventInfo.AddEventHandler(instance, output);
			return output;
		}

		/// <summary>
		/// Unsubscribes from the event on the given instance.
		/// </summary>
		/// <param name="instance">The instance with the event. Null for static types.</param>
		/// <param name="eventInfo">The EventInfo for the event.</param>
		/// <param name="callback">The Delegate to be removed from the event.</param>
		public static void UnsubscribeEvent(object instance, [NotNull] EventInfo eventInfo, [NotNull] Delegate callback)
		{
			if (eventInfo == null)
				throw new ArgumentNullException("eventInfo");

			if (callback == null)
				throw new ArgumentNullException("callback");

			eventInfo.RemoveEventHandler(instance, callback);
		}
	}
}
