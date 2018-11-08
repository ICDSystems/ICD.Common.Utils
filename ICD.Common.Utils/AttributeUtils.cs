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

		private static readonly Dictionary<Attribute, Type> s_AttributeToTypeCache;
		private static readonly Dictionary<Type, IcdHashSet<Attribute>> s_TypeToAttributesCache;

		private static ILoggerService Logger { get { return ServiceProvider.TryGetService<ILoggerService>(); } }

		/// <summary>
		/// Constructor.
		/// </summary>
		static AttributeUtils()
		{
			s_CachedAssemblies = new IcdHashSet<Assembly>();
			s_CachedTypes = new IcdHashSet<Type>();

			s_AttributeToTypeCache = new Dictionary<Attribute, Type>();
			s_TypeToAttributesCache = new Dictionary<Type, IcdHashSet<Attribute>>();
		}

		#region Caching

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

			s_CachedAssemblies.Add(assembly);

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
					if (inner is System.IO.FileNotFoundException)
					{
						Logger.AddEntry(eSeverity.Error,
						                "{0} failed to cache assembly {1} - Could not find one or more dependencies by path",
						                typeof(AttributeUtils).Name, assembly.GetName().Name);
						continue;
					}

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

			try
			{
				s_TypeToAttributesCache[type] = new IcdHashSet<Attribute>(type.GetCustomAttributes<Attribute>(false));
				foreach (Attribute attribute in s_TypeToAttributesCache[type])
					s_AttributeToTypeCache[attribute] = type;
			}
			// GetMethods for Open Generic Types is not supported.
			catch (NotSupportedException)
			{
			}
			// Not sure why this happens :/
			catch (InvalidProgramException)
			{
			}
		}

		#endregion

		#region Lookup

		/// <summary>
		/// Gets the class attributes of the given generic type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetClassAttributes<T>()
			where T : Attribute
		{
			return s_AttributeToTypeCache.Select(kvp => kvp.Key)
			                             .OfType<T>();
		}

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

			IcdHashSet<Attribute> attributes;

			return s_TypeToAttributesCache.TryGetValue(type, out attributes)
				       ? attributes.ToArray(attributes.Count)
				       : Enumerable.Empty<Attribute>();
		}

		/// <summary>
		/// Gets the type with the given attribute.
		/// </summary>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public static Type GetClass(Attribute attribute)
		{
			if (attribute == null)
				throw new ArgumentNullException("attribute");

			return s_AttributeToTypeCache[attribute];
		}

		#endregion
	}
}
