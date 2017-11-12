using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Attributes.Rpc;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Attributes.Rpc
{
	[TestFixture]
    public sealed class RpcAttributeTest : AbstractIcdAttributeTest
    {
		private class TestClass
		{
			[Rpc("A")]
			public int TestProperty { get; set; }

			[Rpc("A")]
			public void TestMethod()
			{
			}

			[Rpc("B")]
			public void TestMethod(int a)
			{
			}

			[Rpc("C")]
			public void TestMethod(int a, string b)
			{
			}
		}

		[Test]
		public void GetMethodTest()
		{
			TestClass instance = new TestClass();

			Assert.NotNull(RpcAttribute.GetMethod(instance, "A", new object[] { }));
			Assert.Null(RpcAttribute.GetMethod(instance, "B", new object[] { }));
			Assert.NotNull(RpcAttribute.GetMethod(instance, "B", new object[] { 1 }));
			Assert.NotNull(RpcAttribute.GetMethod(instance, "C", new object[] { 1, null }));
			Assert.NotNull(RpcAttribute.GetMethod(instance, "C", new object[] { 1, "test" }));
		}

		[Test]
		public void GetPropertyTest()
		{
			TestClass instance = new TestClass();

			Assert.NotNull(RpcAttribute.GetProperty(instance, "A", "test"));
			Assert.NotNull(RpcAttribute.GetProperty(instance, "A", null));
			Assert.Null(RpcAttribute.GetProperty(instance, "B", "test"));
			Assert.Null(RpcAttribute.GetProperty(instance, "B", 1));
		}
	}
}
