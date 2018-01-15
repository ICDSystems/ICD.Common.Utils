using System;

namespace ICD.Common.Utils.Services
{
	public sealed class ServiceNotFoundException : Exception
	{
		private const string DEFAULT_MESSAGE = "The requested service {0} was not found in the service provider";

		public Type ServiceType { get; set; }

		public ServiceNotFoundException()
		{
		}

		public ServiceNotFoundException(string message) : base(message)
		{
		}

		public ServiceNotFoundException(string message, Exception inner) : base(message, inner)
		{
		}

		public ServiceNotFoundException(Type type) : base(string.Format(DEFAULT_MESSAGE, type.Name))
		{
			ServiceType = type;
		}
	}
}
