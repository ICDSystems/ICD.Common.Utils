using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
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
		/// Instantiates the given type using the constructor matching the given values.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		[PublicAPI]
		public static object Instantiate(Type type, params object[] values)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			ConstructorInfo constructor =
#if SIMPLSHARP
				((CType)type)
#else
				type
#endif
					.GetConstructors()
					.FirstOrDefault(c => MatchesConstructorParameters(c, values));

			try
			{
				if (constructor != null)
					return constructor.Invoke(values);
			}
			catch (TypeLoadException e)
			{
				throw new TypeLoadException(e.GetBaseException().Message);
			}

			string message = string.Format("Unable to find constructor for {0}", type.Name);
			throw new InvalidOperationException(message);
		}

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
			IEnumerable<CType> methodTypes
#else
			IEnumerable<Type> methodTypes
#endif
				= constructor.GetParameters().Select(p => p.ParameterType);
			return ParametersMatchTypes(methodTypes, parameters);
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
			IEnumerable<CType> methodTypes
#else
			IEnumerable<Type> methodTypes
#endif
				= method.GetParameters().Select(p => p.ParameterType);
			return ParametersMatchTypes(methodTypes, parameters);
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
			CType propertyType
#else
			Type propertyType
#endif
				= property.PropertyType;
			return ParametersMatchTypes(new[] {propertyType}, new[] {parameter});
		}

		/// <summary>
		/// Returns true if the parameters array is compatible with the given property types array.
		/// </summary>
		/// <param name="types"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
#if SIMPLSHARP
		private static bool ParametersMatchTypes(IEnumerable<CType> types, IEnumerable<object> parameters)
		{
			if (types == null)
				throw new ArgumentNullException("types");

			CType[] typesArray = types as CType[] ?? types.ToArray();
#else
		private static bool ParametersMatchTypes(IEnumerable<Type> types, IEnumerable<object> parameters)
		{
			if (types == null)
				throw new ArgumentNullException("types");

			Type[] typesArray = types as Type[] ?? types.ToArray();
#endif
			if (parameters == null)
				throw new ArgumentNullException("parameters");

			object[] parametersArray = parameters as object[] ?? parameters.ToArray();

			if (parametersArray.Length != typesArray.Length)
				return false;

			// Compares each pair of items in the two arrays.
			return !parametersArray.Where((t, index) => !ParameterMatchesType(typesArray[index], t)).Any();
		}

		/// <summary>
		/// Returns true if the parameter can be assigned to the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
#if SIMPLSHARP
		private static bool ParameterMatchesType(CType type, object parameter)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			// Can the parameter be assigned a null value?
			if (parameter == null)
				return (type.IsClass || !type.IsValueType || Nullable.GetUnderlyingType(type) != null);

			return type.IsInstanceOfType(parameter);
		}
#else
        private static bool ParameterMatchesType(Type type, object parameter)
        {
			if (type == null)
				throw new ArgumentNullException("type");

            TypeInfo info = type.GetTypeInfo();
			// Can the parameter be assigned a null value?
			if (parameter == null)
				return (info.IsClass || !info.IsValueType || Nullable.GetUnderlyingType(type) != null);

			return info.IsInstanceOfType(parameter);
        }
#endif

		/// <summary>
		/// Same as doing default(Type).
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
#if SIMPLSHARP
		public static object GetDefaultValue(CType type)
#else
		public static object GetDefaultValue(Type type)
#endif
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
		/// Creates an instance of the given type, calling the default constructor.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T CreateInstance<T>()
			where T : new()
		{
			return (T)CreateInstance(typeof(T));
		}

		/// <summary>
		/// Creates an instance of the given type, calling the default constructor.
		/// </summary>
		/// <returns></returns>
		public static object CreateInstance(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			try
			{
				return Activator.CreateInstance(type);
			}
			catch (TargetInvocationException e)
			{
				throw e.GetBaseException();
			}
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

#if SIMPLSHARP
			return assembly.GetCustomAttributes(typeof(T), false).Cast<T>();
#else
            return assembly.GetCustomAttributes<T>();
#endif
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
	}
}
