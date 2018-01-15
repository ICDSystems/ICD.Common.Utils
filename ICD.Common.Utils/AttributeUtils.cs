using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
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
		// Avoid caching the same assembly multiple times.
		private static readonly IcdHashSet<Assembly> s_CachedAssemblies;
		private static readonly IcdHashSet<Type> s_CachedTypes;

		private static readonly Dictionary<Attribute, MethodInfo> s_AttributeToMethodCache;
		private static readonly Dictionary<Type, IcdHashSet<Attribute>> s_TypeToAttributesCache;

		private static ILoggerService Logger { get { return ServiceProvider.TryGetService<ILoggerService>(); } }

		/// <summary>
		/// Constructor.
		/// </summary>
		static AttributeUtils()
		{
			s_CachedAssemblies = new IcdHashSet<Assembly>();
			s_CachedTypes = new IcdHashSet<Type>();

			s_AttributeToMethodCache = new Dictionary<Attribute, MethodInfo>();
			s_TypeToAttributesCache = new Dictionary<Type, IcdHashSet<Attribute>>();
		}

		#region Caching

		/// <summary>
		/// Pre-emptively caches the given assemblies for lookup.
		/// </summary>
		/// <param name="assemblies"></param>
		public static void CacheAssemblies(IEnumerable<Assembly> assemblies)
		{
			if (assemblies == null)
				throw new ArgumentNullException("assemblies");

			foreach (Assembly assembly in assemblies)
				CacheAssembly(assembly);
		}

		/// <summary>
		/// Pre-emptively caches the given assembly for lookup.
		/// </summary>
		/// <param name="assembly"></param>
		public static bool CacheAssembly(Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");

			if (s_CachedAssemblies.Contains(assembly))
				return true;

#if SIMPLSHARP
			CType[] types;
#else
            Type[] types;
#endif
			try
			{
				types = assembly.GetTypes();
			}
#if STANDARD
			catch (ReflectionTypeLoadException e)
			{
				foreach (Exception inner in e.LoaderExceptions)
				{
					Logger.AddEntry(eSeverity.Error, inner, "{0} failed to cache assembly {1}", typeof(AttributeUtils).Name,
					                assembly.GetName().Name);
				}

				return false;
			}
#endif
			catch (TypeLoadException e)
			{
#if SIMPLSHARP
				Logger.AddEntry(eSeverity.Error, e, "{0} failed to cache assembly {1}", typeof(AttributeUtils).Name,
				                assembly.GetName().Name);
#else
				Logger.AddEntry(eSeverity.Error, e, "{0} failed to cache assembly {1} - could not load type {2}",
								typeof(AttributeUtils).Name, assembly.GetName().Name, e.TypeName);
#endif
				return false;
			}

			foreach (var type in types)
				CacheType(type);

			s_CachedAssemblies.Add(assembly);
			return true;
		}

		/// <summary>
		/// Pre-emptively caches the given type for lookup.
		/// </summary>
		/// <param name="type"></param>
#if SIMPLSHARP
		public static void CacheType(CType type)
#else
		public static void CacheType(Type type)
#endif
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (s_CachedTypes.Contains(type))
				return;
			s_CachedTypes.Add(type);

			MethodInfo[] methods;

			try
			{
#if SIMPLSHARP
				s_TypeToAttributesCache[type] = new IcdHashSet<Attribute>(type.GetCustomAttributes<Attribute>(false));
				methods = type.GetMethods();
#else
				s_TypeToAttributesCache[type] =
					new IcdHashSet<Attribute>(ReflectionExtensions.GetCustomAttributes<Attribute>(type.GetTypeInfo(), false));
				methods = type.GetTypeInfo().GetMethods();
#endif
			}
			// GetMethods for Open Generic Types is not supported.
			catch (NotSupportedException)
			{
				return;
			}
			// Not sure why this happens :/
			catch (InvalidProgramException)
			{
				return;
			}

			foreach (MethodInfo method in methods)
				CacheMethod(method);
		}

		/// <summary>
		/// Caches the method.
		/// </summary>
		/// <param name="method"></param>
		private static void CacheMethod(MethodInfo method)
		{
			if (method == null)
				throw new ArgumentNullException("method");

			foreach (Attribute attribute in ReflectionExtensions.GetCustomAttributes<Attribute>(method, false))
				s_AttributeToMethodCache[attribute] = method;
		}

		#endregion

		#region Lookup

		/// <summary>
		/// Gets the first attribute on the given class type matching the generic type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <returns></returns>
		[CanBeNull]
		public static T GetClassAttribute<T>(Type type)
			where T : Attribute
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return GetClassAttributes<T>(type).FirstOrDefault();
		}

		/// <summary>
		/// Gets the attributes on the given class type matching the generic type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetClassAttributes<T>(Type type)
			where T : Attribute
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return GetClassAttributes(type).OfType<T>();
		}

		/// <summary>
		/// Gets the attributes on the given class.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IEnumerable<Attribute> GetClassAttributes(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			CacheType(type);

			return s_TypeToAttributesCache.ContainsKey(type)
				       ? s_TypeToAttributesCache[type].ToArray()
				       : Enumerable.Empty<Attribute>();
		}

		/// <summary>
		/// Gets all of the cached method attributes of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetMethodAttributes<T>()
			where T : Attribute
		{
			return s_AttributeToMethodCache.Select(p => p.Key)
			                               .OfType<T>()
			                               .ToArray();
		}

		/// <summary>
		/// Gets the cached method for the given attribute.
		/// </summary>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public static MethodInfo GetMethod(Attribute attribute)
		{
			if (attribute == null)
				throw new ArgumentNullException("attribute");

			return s_AttributeToMethodCache[attribute];
		}

		#endregion
	}
}
