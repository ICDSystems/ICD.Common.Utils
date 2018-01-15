using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils.Services
{
	public sealed class ServiceProvider
	{
		private static ServiceProvider s_Instance;

		private readonly Dictionary<Type, object> m_Services;
		private readonly SafeCriticalSection m_ServicesSection;

		private static ServiceProvider Instance { get { return s_Instance ?? (s_Instance = new ServiceProvider()); } }

		/// <summary>
		/// Prevent external instantiation.
		/// </summary>
		private ServiceProvider()
		{
			m_Services = new Dictionary<Type, object>();
			m_ServicesSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Retrieves the registered service of the given type. Use this for required dependencies.
		/// </summary>
		/// <typeparam name="TService">service type to retrieve</typeparam>
		/// <returns></returns>
		/// <exception cref="ServiceNotFoundException">Thrown if a service of the given type is not registered</exception>
		[PublicAPI]
		[NotNull]
		public static TService GetService<TService>()
		{
			return (TService)GetService(typeof(TService));
		}

		/// <summary>
		/// Retrieves the registered service of the given type. Use this for required dependencies.
		/// </summary>
		/// <param name="tService">service type to retrieve</param>
		/// <returns></returns>
		/// <exception cref="ServiceNotFoundException">Thrown if a service of the given type is not registered</exception>
		[PublicAPI]
		[NotNull]
		public static object GetService(Type tService)
		{
			if (tService == null)
				throw new ArgumentNullException("tService");

			return Instance.GetServiceInstance(tService);
		}

		/// <summary>
		/// Retrieves the registered services.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<object> GetServices()
		{
			return Instance.GetServicesInstance();
		}

		/// <summary>
		/// Retrieves the registered service of the given type. Returns null if the service type was not found.
		/// Use this for optional dependencies.
		/// </summary>
		/// <typeparam name="TService">service type to retrieve</typeparam>
		/// <returns>requested service or null if that service type is not registered</returns>
		[PublicAPI]
		[CanBeNull]
		public static TService TryGetService<TService>()
		{
			return (TService)TryGetService(typeof(TService));
		}

		/// <summary>
		/// Retrieves the registered service of the given type. Returns null if the service type was not found.
		/// Use this for optional dependencies.
		/// </summary>
		/// <param name="tService">service type to retrieve</param>
		/// <returns>requested service or null if that service type is not registered</returns>
		[PublicAPI]
		[CanBeNull]
		public static object TryGetService(Type tService)
		{
			if (tService == null)
				throw new ArgumentNullException("tService");

			return Instance.TryGetServiceInstance(tService);
		}

		/// <summary>
		/// Registers a service instance to the type given.
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <param name="service"></param>
		[PublicAPI]
		public static void AddService<TService>(TService service)
		{
// ReSharper disable once CompareNonConstrainedGenericWithNull
			if (service == null)
				throw new ArgumentNullException("service");

			AddService(typeof(TService), service);
		}

		/// <summary>
		/// Registers a service instance to the type given.
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <param name="service"></param>
		[PublicAPI]
		public static bool TryAddService<TService>(TService service)
		{
			// ReSharper disable once CompareNonConstrainedGenericWithNull
			if (service == null)
				throw new ArgumentNullException("service");

			return TryAddService(typeof(TService), service);
		}

		/// <summary>
		/// Registers a service instance to the type given.
		/// </summary>
		/// <param name="tService"></param>
		/// <param name="service"></param>
		[PublicAPI]
		public static void AddService(Type tService, object service)
		{
			if (tService == null)
				throw new ArgumentNullException("tService");

			if (service == null)
				throw new ArgumentNullException("service");

			Instance.AddServiceInstance(tService, service);
		}

		/// <summary>
		/// Registers a service instance to the type given.
		/// </summary>
		/// <param name="tService"></param>
		/// <param name="service"></param>
		[PublicAPI]
		public static bool TryAddService(Type tService, object service)
		{
			if (tService == null)
				throw new ArgumentNullException("tService");

			if (service == null)
				throw new ArgumentNullException("service");

			return Instance.TryAddServiceInstance(tService, service);
		}

		/// <summary>
		/// Attempts to remove the given service from every registered type.
		/// </summary>
		/// <param name="service"></param>
		[PublicAPI]
		public static void RemoveAllServices(object service)
		{
			// ReSharper disable once CompareNonConstrainedGenericWithNull
			if (service == null)
				throw new ArgumentNullException("service");

			Instance.RemoveAllServicesInstance(service);
		}

		/// <summary>
		/// Attempts to remove the given service from the given type.
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <param name="service"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool RemoveService<TService>(TService service)
		{
// ReSharper disable once CompareNonConstrainedGenericWithNull
			if (service == null)
				throw new ArgumentNullException("service");

			return RemoveService(typeof(TService), service);
		}

		/// <summary>
		/// Attempts to remove the given service from the given type.
		/// </summary>
		/// <param name="tService"></param>
		/// <param name="service"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool RemoveService(Type tService, object service)
		{
			// ReSharper disable once CompareNonConstrainedGenericWithNull
			if (service == null)
				throw new ArgumentNullException("service");

			return Instance.RemoveServiceInstance(tService, service);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Retrieves the registered service of the given type. Returns null if the service type was not found.
		/// Use this for optional dependencies.
		/// </summary>
		/// <param name="tService">service type to retrieve</param>
		/// <returns>requested service or null if that service type is not registered</returns>
		[CanBeNull]
		private object TryGetServiceInstance(Type tService)
		{
			if (tService == null)
				throw new ArgumentNullException("tService");

			try
			{
				m_ServicesSection.Enter();

				object service;
				m_Services.TryGetValue(tService, out service);
				return service;
			}
			finally
			{
				m_ServicesSection.Leave();
			}
		}

		/// <summary>
		/// Gets the service of the given type.
		/// </summary>
		/// <param name="tService"></param>
		/// <returns></returns>
		private object GetServiceInstance(Type tService)
		{
			if (tService == null)
				throw new ArgumentNullException("tService");

			object service = TryGetService(tService);
			if (service != null)
				return service;

			throw new ServiceNotFoundException(tService);
		}

		/// <summary>
		/// Gets the registered services.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<object> GetServicesInstance()
		{
			return m_ServicesSection.Execute(() => m_Services.Values.ToList());
		}

		/// <summary>
		/// Adds the given service under the given type.
		/// </summary>
		/// <param name="tService"></param>
		/// <param name="service"></param>
		private void AddServiceInstance(Type tService, object service)
		{
			if (tService == null)
				throw new ArgumentNullException("tService");

			if (service == null)
				throw new ArgumentNullException("service");

			try
			{
				m_ServicesSection.Enter();

				if (m_Services.ContainsKey(tService))
				{
					string message = string.Format("{0} already contains a {1} service", GetType().Name, tService.Name);
					throw new InvalidOperationException(message);
				}

				m_Services.Add(tService, service);
			}
			finally
			{
				m_ServicesSection.Leave();
			}
		}

		/// <summary>
		/// Adds the given service under the given type.
		/// </summary>
		/// <param name="tService"></param>
		/// <param name="service"></param>
		private bool TryAddServiceInstance(Type tService, object service)
		{
			if (tService == null)
				throw new ArgumentNullException("tService");

			if (service == null)
				throw new ArgumentNullException("service");

			try
			{
				m_ServicesSection.Enter();

				if (m_Services.ContainsKey(tService))
				{
					return false;
				}

				m_Services.Add(tService, service);
			}
			finally
			{
				m_ServicesSection.Leave();
			}
			return true;
		}

		/// <summary>
		/// Removes the given service from the given type.
		/// </summary>
		/// <param name="tService"></param>
		/// <param name="service"></param>
		private bool RemoveServiceInstance(Type tService, object service)
		{
			if (tService == null)
				throw new ArgumentNullException("tService");

			if (service == null)
				throw new ArgumentNullException("service");

			try
			{
				m_ServicesSection.Enter();

				if (!m_Services.ContainsKey(tService))
					return false;

				return m_Services[tService] == service && m_Services.Remove(tService);
			}
			finally
			{
				m_ServicesSection.Leave();
			}
		}

		/// <summary>
		/// Removes the given service from all registered types.
		/// </summary>
		/// <param name="service"></param>
		private void RemoveAllServicesInstance(object service)
		{
			if (service == null)
				throw new ArgumentNullException("service");

			m_ServicesSection.Execute(() => m_Services.RemoveAllValues(service));
		}

		#endregion
	}
}
