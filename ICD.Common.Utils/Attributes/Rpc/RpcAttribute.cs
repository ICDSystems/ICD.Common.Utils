using System;
using System.Collections.Generic;
using System.Linq;
#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Attributes.Rpc
{
	/// <summary>
	/// Represents a method that can be called by the server via RPC.
	/// </summary>
	[PublicAPI]
	[MeansImplicitUse]
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class RpcAttribute : AbstractIcdAttribute
	{
		private static readonly Dictionary<Type, Dictionary<string, IcdHashSet<MethodInfo>>> s_MethodCache;
		private static readonly Dictionary<Type, Dictionary<string, IcdHashSet<PropertyInfo>>> s_PropertyCache;

		private static readonly SafeCriticalSection s_MethodCacheSection;
		private static readonly SafeCriticalSection s_PropertyCacheSection;

		private readonly string m_Key;

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		static RpcAttribute()
		{
			s_MethodCache = new Dictionary<Type, Dictionary<string, IcdHashSet<MethodInfo>>>();
			s_PropertyCache = new Dictionary<Type, Dictionary<string, IcdHashSet<PropertyInfo>>>();

			s_MethodCacheSection = new SafeCriticalSection();
			s_PropertyCacheSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="key"></param>
		public RpcAttribute(string key)
		{
			m_Key = key;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets the method on the client with the given key, matching the parameter types.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="key"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		[CanBeNull]
		public static MethodInfo GetMethod(object client, string key, IEnumerable<object> parameters)
		{
			return GetMethods(client, key).FirstOrDefault(m => ReflectionUtils.MatchesMethodParameters(m, parameters));
		}

		/// <summary>
		/// Gets the property on the client with the given key, matching the parameter type.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="key"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		[CanBeNull]
		public static PropertyInfo GetProperty(object client, string key, object parameter)
		{
			return GetProperties(client, key).FirstOrDefault(p => ReflectionUtils.MatchesPropertyParameter(p, parameter));
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Returns the methods on the client with the given key.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		private static IEnumerable<MethodInfo> GetMethods(object client, string key)
		{
			s_MethodCacheSection.Enter();

			try
			{
				Type clientType = client.GetType();

				// Cache the methods for the key so subsequent calls are faster.
				if (!s_MethodCache.ContainsKey(clientType))
					s_MethodCache[clientType] = new Dictionary<string, IcdHashSet<MethodInfo>>();

				if (!s_MethodCache[clientType].ContainsKey(key))
				{
					s_MethodCache[clientType][key] =
						clientType
#if SIMPLSHARP
							.GetCType()
#else
                        .GetTypeInfo()
#endif
							.GetMethods(BindingFlags.Public |
							            BindingFlags.NonPublic |
							            BindingFlags.Instance)
							.Where(m => m.GetCustomAttributes<RpcAttribute>(true)
							             .Any(a => a.m_Key == key))
							.ToHashSet();
				}

				return s_MethodCache[clientType][key];
			}
			finally
			{
				s_MethodCacheSection.Leave();
			}
		}

		/// <summary>
		/// Returns the properties on the client with the given key.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		private static IEnumerable<PropertyInfo> GetProperties(object client, string key)
		{
			s_PropertyCacheSection.Enter();

			try
			{
				Type clientType = client.GetType();

				// Cache the properties for the key so subsequent calls are faster.
				if (!s_PropertyCache.ContainsKey(clientType))
					s_PropertyCache[clientType] = new Dictionary<string, IcdHashSet<PropertyInfo>>();

				if (!s_PropertyCache[clientType].ContainsKey(key))
				{
					s_PropertyCache[clientType][key] =
						clientType
#if SIMPLSHARP
							.GetCType()
#else
                        .GetTypeInfo()
#endif
							.GetProperties(BindingFlags.Public |
							               BindingFlags.NonPublic |
							               BindingFlags.Instance)
							.Where(p => p.CanWrite && p.GetCustomAttributes<RpcAttribute>(true)
							                           .Any(a => a.m_Key == key))
							.ToHashSet();
				}

				return s_PropertyCache[clientType][key];
			}
			finally
			{
				s_PropertyCacheSection.Leave();
			}
		}

		#endregion
	}
}
