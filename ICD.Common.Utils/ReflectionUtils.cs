using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.IO;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
using Crestron.SimplSharp.Reflection;
using Activator = Crestron.SimplSharp.Reflection.Activator;
#else
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using System.Runtime.Loader;
using Activator = System.Activator;
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
		public static bool MatchesConstructorParameters(ConstructorInfo constructor, IEnumerable<object> parameters)
		{
			if (constructor == null)
				throw new ArgumentNullException("constructor");

			if (parameters == null)
				throw new ArgumentNullException("parameters");

#if SIMPLSHARP
			IEnumerable<CType>
#else
			IEnumerable<Type>
#endif
				parameterTypes = constructor.GetParameters().Select(p => p.ParameterType);

			return ParametersMatchTypes(parameterTypes, parameters);
		}

		/// <summary>
		/// Returns true if the parameters match the method parameters.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static bool MatchesMethodParameters(MethodBase method, IEnumerable<object> parameters)
		{
			if (method == null)
				throw new ArgumentNullException("method");

			if (parameters == null)
				throw new ArgumentNullException("parameters");

#if SIMPLSHARP
			IEnumerable<CType>
#else
			IEnumerable<Type> 
#endif
				parameterTypes = method.GetParameters().Select(p => p.ParameterType);

			return ParametersMatchTypes(parameterTypes, parameters);
		}

		/// <summary>
		/// Returns true if the if the parameter match the property parameter.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public static bool MatchesPropertyParameter(PropertyInfo property, object parameter)
		{
			if (property == null)
				throw new ArgumentNullException("property");

#if SIMPLSHARP
			CType
#else
			Type
#endif
				propertyType = property.PropertyType;

			return ParametersMatchTypes(new[] {propertyType}, new[] {parameter});
		}

		/// <summary>
		/// Returns true if the parameters array is compatible with the given property types array.
		/// </summary>
		/// <param name="types"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		private static bool ParametersMatchTypes(
#if SIMPLSHARP
			IEnumerable<CType>
#else	
			IEnumerable<Type>
#endif
				types, IEnumerable<object> parameters)
		{
			if (types == null)
				throw new ArgumentNullException("types");

			if (parameters == null)
				throw new ArgumentNullException("parameters");

#if SIMPLSHARP
			CType[]
#else
			Type[]
#endif
				typesArray = types as
#if SIMPLSHARP
				             CType[]
#else
				             Type[]
#endif
				             ?? types.ToArray();

			object[] parametersArray = parameters as object[] ?? parameters.ToArray();

			if (parametersArray.Length != typesArray.Length)
				return false;

			// Compares each pair of items in the two arrays.
			return !parametersArray.Where((t, index) => !ParameterMatchesType(typesArray[index], t))
			                       .Any();
		}

		/// <summary>
		/// Returns true if the parameter can be assigned to the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		private static bool ParameterMatchesType(Type type, object parameter)
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
		public static object GetDefaultValue(Type type)
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
		/// Returns true if the given type has a public parameterless constructor.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool HasPublicParameterlessConstructor(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			const BindingFlags binding = BindingFlags.Instance | BindingFlags.Public;

#if SIMPLSHARP
			return ((CType)type).GetConstructor(binding, null, new CType[0], null)
#else
			return type.GetConstructor(binding, null, new Type[0], null) 
#endif
			       != null;
		}

		/// <summary>
		/// Platform independant delegate instantiation.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="firstArgument"></param>
		/// <param name="method"></param>
		/// <returns></returns>
		public static Delegate CreateDelegate(Type type, object firstArgument, MethodInfo method)
		{
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
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
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
		public static object CreateInstance(Type type, params object[] parameters)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (parameters == null)
				throw new ArgumentNullException("parameters");

			ConstructorInfo constructor =
				type
#if SIMPLSHARP
					.GetCType()
#endif
					.GetConstructors()
					.FirstOrDefault(c => MatchesConstructorParameters(c, parameters));

			try
			{
				if (constructor != null)
					return constructor.Invoke(parameters);
			}
			catch (TypeLoadException e)
			{
				throw e.GetBaseException();
			}
			catch (TargetInvocationException e)
			{
				throw e.GetBaseException();
			}

			string message = string.Format("Unable to find constructor for {0}", type.Name);
			throw new InvalidOperationException(message);
		}

		/// <summary>
		/// Creates an instance of the given type, calling the default constructor.
		/// </summary>
		/// <returns></returns>
		public static T CreateInstance<T>(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (!type.IsAssignableTo(typeof(T)))
				throw new InvalidOperationException("Type is not assignable to T");

			return (T)CreateInstance(type);
		}

		/// <summary>
		/// Gets the custom attributes added to the given assembly.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetCustomAttributes<T>(Assembly assembly)
			where T : Attribute
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");

			try
			{
				return assembly.GetCustomAttributes<T>();
			}
			catch (FileNotFoundException)
			{
				return Enumerable.Empty<T>();
			}
		}

		/// <summary>
		/// Loads the assembly at the given path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Assembly LoadAssemblyFromPath(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			if (string.IsNullOrEmpty(path))
				throw new ArgumentException("Path is empty", "path");

			string fileNameWithOutExtension = IcdPath.GetFileNameWithoutExtension(path);

#if SIMPLSHARP

			try
			{
				return Assembly.Load(new AssemblyName {Name = fileNameWithOutExtension});
			}
			catch (IOException)
			{
				return Assembly.LoadFrom(path);
			}
			catch (FileNotFoundException)
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
		/// Finds the corresponding property info on the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		public static PropertyInfo GetImplementation(Type type, PropertyInfo property)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (property == null)
				throw new ArgumentNullException("property");

			if (type.IsInterface)
				throw new InvalidOperationException("Type must not be an interface");

			property = type
#if SIMPLSHARP
				.GetCType()
#else
				.GetTypeInfo()
#endif
				.GetProperty(property.Name, property.PropertyType);

			if (property == null)
				return null;

			return property.DeclaringType == type
				? property
				: GetImplementation(property.DeclaringType, property);
		}

		/// <summary>
		/// Changes the given value to the given type.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object ChangeType(object value, Type type)
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

					if (value is string)
						return Enum.Parse(type, value as string, false);
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
		/// Subscribes to the event on the given instance using the handler and callback method.
		/// </summary>
		/// <param name="instance">The instance with the event. Null for static types.</param>
		/// <param name="eventInfo">The EventInfo for the event.</param>
		/// <param name="handler">The instance with the callback MethodInfo. Null for static types.</param>
		/// <param name="callback">The MethodInfo for the callback method.</param>
		/// <returns></returns>
		public static Delegate SubscribeEvent(object instance, EventInfo eventInfo, object handler, MethodInfo callback)
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
		public static void UnsubscribeEvent(object instance, EventInfo eventInfo, Delegate callback)
		{
			if (eventInfo == null)
				throw new ArgumentNullException("eventInfo");

			if (callback == null)
				throw new ArgumentNullException("callback");

			eventInfo.RemoveEventHandler(instance, callback);
		}
	}
}
