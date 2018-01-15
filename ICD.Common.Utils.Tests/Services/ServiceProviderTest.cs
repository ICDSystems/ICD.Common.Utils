using System;
using ICD.Common.Utils.Services;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Services
{
	[TestFixture]
    public sealed class ServiceProviderTest
    {
		[Test]
		public void GetServiceGenericTest()
		{
			Assert.Throws<ServiceNotFoundException>(() => ServiceProvider.GetService<ServiceProviderTest>());

			ServiceProvider.AddService(this);

			Assert.AreEqual(this, ServiceProvider.GetService<ServiceProviderTest>());

			// Cleanup
			ServiceProvider.RemoveService(this);
		}

		[Test]
		public void GetServiceTest()
		{
			Assert.Throws<ServiceNotFoundException>(() => ServiceProvider.GetService(typeof(ServiceProviderTest)));

			ServiceProvider.AddService(typeof(ServiceProviderTest), "Test");

			Assert.AreEqual("Test", ServiceProvider.GetService(typeof(ServiceProviderTest)));

			// Cleanup
			ServiceProvider.RemoveService(typeof(ServiceProviderTest), "Test");
		}

		[Test]
		public void TryGetServiceGenericTest()
		{
			Assert.IsNull(ServiceProvider.TryGetService<ServiceProviderTest>());

			ServiceProvider.AddService(this);

			Assert.AreEqual(this, ServiceProvider.TryGetService<ServiceProviderTest>());

			// Cleanup
			ServiceProvider.RemoveService(this);
		}

		[Test]
		public void TryGetServiceTest()
		{
			Assert.IsNull(ServiceProvider.TryGetService(typeof(ServiceProviderTest)));

			ServiceProvider.AddService(typeof(ServiceProviderTest), "Test");

			Assert.AreEqual("Test", ServiceProvider.TryGetService(typeof(ServiceProviderTest)));

			// Cleanup
			ServiceProvider.RemoveService(typeof(ServiceProviderTest), "Test");
		}

		[Test]
		public void AddServiceGenericTest()
		{
			Assert.IsNull(ServiceProvider.TryGetService<ServiceProviderTest>());

			ServiceProvider.AddService(typeof(ServiceProviderTest), this);
			Assert.AreEqual(this, ServiceProvider.GetService<ServiceProviderTest>());

			Assert.Throws<InvalidOperationException>(() => ServiceProvider.AddService(this));

			// Cleanup
			ServiceProvider.RemoveService(typeof(ServiceProviderTest), this);
		}

		[Test]
		public void AddServiceTest()
		{
			Assert.IsNull(ServiceProvider.TryGetService(typeof(ServiceProviderTest)));

			ServiceProvider.AddService(typeof(ServiceProviderTest), "Test");
			Assert.AreEqual("Test", ServiceProvider.GetService(typeof(ServiceProviderTest)));

			Assert.Throws<InvalidOperationException>(() => ServiceProvider.AddService(typeof(ServiceProviderTest), "Test"));

			// Cleanup
			ServiceProvider.RemoveService(typeof(ServiceProviderTest), "Test");
		}

		[Test]
		public void RemoveServiceGenericTest()
	    {
		    Assert.IsFalse(ServiceProvider.RemoveService(this));

		    ServiceProvider.AddService(this);

		    Assert.IsFalse(ServiceProvider.RemoveService(new ServiceProviderTest()));
		    Assert.IsTrue(ServiceProvider.RemoveService(this));
		}

		[Test]
		public void RemoveServiceTest()
	    {
		    Assert.IsFalse(ServiceProvider.RemoveService(typeof(ServiceProviderTest), "Test"));

		    ServiceProvider.AddService(typeof(ServiceProviderTest), "Test");

		    Assert.IsFalse(ServiceProvider.RemoveService(typeof(string), "Test"));
		    Assert.IsFalse(ServiceProvider.RemoveService(typeof(ServiceProviderTest), "Test2"));
			Assert.IsTrue(ServiceProvider.RemoveService(typeof(ServiceProviderTest), "Test"));
		}
	}
}
