using NUnit.Framework;
using System;
using System.Collections.Generic;
using ICD.Common.Properties;
#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif

namespace ICD.Common.Utils.Tests
{
	[TestFixture]
	public sealed class ReflectionUtilsTest
	{
		[UsedImplicitly]
		private string TestProperty { get; set; }

		[UsedImplicitly]
		private void TestMethod(string param1, int param2)
		{
		}

		[UsedImplicitly]
		private sealed class TestClass
		{
			public string Param1 { get; }
			public int Param2 { get; }

			public TestClass(string param1, int param2)
			{
				Param1 = param1;
				Param2 = param2;
			}
		}

		[Test]
		public void MatchesConstructorParametersTest()
		{
			Assert.Throws<ArgumentNullException>(() => ReflectionUtils.MatchesConstructorParameters(null, new object[] { "test", 10 }));

			ConstructorInfo constructor = typeof(TestClass).GetConstructor(new[] {typeof(string), typeof(int)});

			Assert.IsTrue(ReflectionUtils.MatchesConstructorParameters(constructor, new object[] {"test", 10}));
			Assert.IsTrue(ReflectionUtils.MatchesConstructorParameters(constructor, new object[] {null, 10}));
			Assert.IsFalse(ReflectionUtils.MatchesConstructorParameters(constructor, new object[] {"test", "test"}));
			Assert.IsFalse(ReflectionUtils.MatchesConstructorParameters(constructor, new object[] {"test"}));
			Assert.IsFalse(ReflectionUtils.MatchesConstructorParameters(constructor, new object[] {"test", "test", "test"}));
			Assert.IsFalse(ReflectionUtils.MatchesConstructorParameters(constructor, new object[] {10, 10}));
		}

		[Test]
		public void MatchesMethodParametersTest()
		{
			Assert.Throws<ArgumentNullException>(() => ReflectionUtils.MatchesMethodParameters(null, new object[] {"test", 10}));

			MethodInfo method = GetType().GetMethod("TestMethod",
			                                        BindingFlags.NonPublic |
			                                        BindingFlags.Instance);

			Assert.IsTrue(ReflectionUtils.MatchesMethodParameters(method, new object[] {"test", 10}));
			Assert.IsTrue(ReflectionUtils.MatchesMethodParameters(method, new object[] {null, 10}));
			Assert.IsFalse(ReflectionUtils.MatchesMethodParameters(method, new object[] {"test", "test"}));
			Assert.IsFalse(ReflectionUtils.MatchesMethodParameters(method, new object[] {"test"}));
			Assert.IsFalse(ReflectionUtils.MatchesMethodParameters(method, new object[] {"test", "test", "test"}));
			Assert.IsFalse(ReflectionUtils.MatchesMethodParameters(method, new object[] {10, 10}));
		}

		[Test]
		public void MatchesPropertyParameterTest()
		{
			Assert.Throws<ArgumentNullException>(() => ReflectionUtils.MatchesPropertyParameter(null, 10));

			PropertyInfo prop = GetType().GetProperty("TestProperty",
			                                          BindingFlags.NonPublic |
			                                          BindingFlags.Instance |
			                                          BindingFlags.GetProperty);

			Assert.IsTrue(ReflectionUtils.MatchesPropertyParameter(prop, "test"));
			Assert.IsTrue(ReflectionUtils.MatchesPropertyParameter(prop, null));
			Assert.IsFalse(ReflectionUtils.MatchesPropertyParameter(prop, 1));
			Assert.IsFalse(ReflectionUtils.MatchesPropertyParameter(prop, new string[0]));
		}

		[TestCase(typeof(string), null)]
		[TestCase(typeof(int), 0)]
		public void GetDefaultValueTest(Type type, object expected)
		{
			Assert.AreEqual(expected, ReflectionUtils.GetDefaultValue(type));
		}

		[Test]
		public void CreateInstanceGenericTest()
		{
			List<string> output = ReflectionUtils.CreateInstance<List<string>>();
			Assert.NotNull(output);
		}

		[Test]
		public void CreateInstanceTest()
		{
			List<string> output = ReflectionUtils.CreateInstance(typeof(List<string>)) as List<string>;
			Assert.NotNull(output);
		}

		[TestCase("test", 10)]
		[TestCase(null, 0)]
		public void CreateInstanceTest(string param1, int param2)
		{
			TestClass result = ReflectionUtils.CreateInstance(typeof(TestClass), param1, param2) as TestClass;

			Assert.NotNull(result);
			Assert.AreEqual(param1, result.Param1);
			Assert.AreEqual(param2, result.Param2);
		}

		[Test]
		public void GetCustomAttributesTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void LoadAssemblyFromPathTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void GetImplementationTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void ChangeTypeTest()
		{
			// Same type
			Assert.AreEqual(10, ReflectionUtils.ChangeType(10, typeof(int)));

			// Null
			Assert.AreEqual(null, ReflectionUtils.ChangeType(null, typeof(string)));

			// Enums
			Assert.AreEqual(BindingFlags.GetProperty, ReflectionUtils.ChangeType((int)(object)BindingFlags.GetProperty, typeof(BindingFlags)));
			Assert.AreEqual(BindingFlags.GetProperty, ReflectionUtils.ChangeType(BindingFlags.GetProperty.ToString(), typeof(BindingFlags)));
			Assert.AreEqual(BindingFlags.GetProperty, ReflectionUtils.ChangeType(((int)(object)BindingFlags.GetProperty).ToString(), typeof(BindingFlags)));

			// Everything else
			Assert.AreEqual(10, ReflectionUtils.ChangeType("10", typeof(int)));
		}
	}
}
