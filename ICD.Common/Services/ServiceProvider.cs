using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;

namespace ICD.Common.Services
{
	public sealed class ServiceProvider : IDisposable
	{
		#region Static

		private static ServiceProvider s_Instance;

		private static ServiceProvider Instance { get { return s_Instance ?? (s_Instance = new ServiceProvider()); } }

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
			return Instance.GetServiceInstance(tService);
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
			try
			{
				return Instance.GetServiceInstance(tService);
			}
			catch (ServiceNotFoundException)
			{
				return null;
			}
		}

		/// <summary>
		/// Registers a service instance to the type given.
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <param name="service"></param>
		[PublicAPI]
		public static void AddService<TService>(TService service)
		{
			AddService(typeof(TService), service);
		}

		/// <summary>
		/// Registers a service instance to the type given.
		/// </summary>
		/// <param name="tService"></param>
		/// <param name="service"></param>
		[PublicAPI]
		public static void AddService(Type tService, object service)
		{
			Instance.AddServiceInstance(tService, service);
		}

		#endregion

		private readonly Dictionary<Type, object> m_Services = new Dictionary<Type, object>();
		private readonly SafeCriticalSection m_ServicesSection = new SafeCriticalSection();

		#region Methods

		private object GetServiceInstance(Type tService)
		{
			try
			{
				m_ServicesSection.Enter();

				object service;
				if (m_Services.TryGetValue(tService, out service) && service != null)
					return service;
				throw new ServiceNotFoundException(tService);
			}
			finally
			{
				m_ServicesSection.Leave();
			}
		}

		[PublicAPI]
		private void AddServiceInstance(Type tService, object service)
		{
			m_ServicesSection.Enter();
			m_Services[tService] = service;
			m_ServicesSection.Leave();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			try
			{
				m_ServicesSection.Enter();
				foreach (object service in m_Services.Values.Distinct())
				{
					if (!(service is IDisposable))
						continue;
					((IDisposable)service).Dispose();
				}
				m_Services.Clear();
			}
			finally
			{
				m_ServicesSection.Leave();
			}
		}

		public static void DisposeStatic()
		{
			if (s_Instance != null)
				s_Instance.Dispose();
			s_Instance = null;
		}

		#endregion
	}
}
