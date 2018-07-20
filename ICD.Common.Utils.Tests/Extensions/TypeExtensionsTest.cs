using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ICD.Common.Utils.Extensions;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Extensions
{
	[TestFixture]
	public sealed class TypeExtensionsTest
	{
		[TestCase(typeof(byte), false)]
		[TestCase(typeof(byte?), true)]
		[TestCase(typeof(string), true)]
		public void CanBeNullTest(Type value, bool expected)
		{
			Assert.AreEqual(expected, value.CanBeNull());
		}

		[TestCase(typeof(byte), true)]
		[TestCase(typeof(decimal), true)]
		[TestCase(typeof(double), true)]
		[TestCase(typeof(float), true)]
		[TestCase(typeof(int), true)]
		[TestCase(typeof(long), true)]
		[TestCase(typeof(sbyte), true)]
		[TestCase(typeof(short), true)]
		[TestCase(typeof(uint), true)]
		[TestCase(typeof(ulong), true)]
		[TestCase(typeof(ushort), true)]
		[TestCase(typeof(string), false)]
		public void IsNumericTest(Type value, bool expected)
		{
			Assert.AreEqual(expected, value.IsNumeric());
		}

		[TestCase(typeof(byte), false)]
		[TestCase(typeof(decimal), true)]
		[TestCase(typeof(double), true)]
		[TestCase(typeof(float), true)]
		[TestCase(typeof(int), true)]
		[TestCase(typeof(long), true)]
		[TestCase(typeof(sbyte), true)]
		[TestCase(typeof(short), true)]
		[TestCase(typeof(uint), false)]
		[TestCase(typeof(ulong), false)]
		[TestCase(typeof(ushort), false)]
		[TestCase(typeof(string), false)]
		public void IsSignedNumericTest(Type value, bool expected)
		{
			Assert.AreEqual(expected, value.IsSignedNumeric());
		}

		[TestCase(typeof(byte), false)]
		[TestCase(typeof(decimal), true)]
		[TestCase(typeof(double), true)]
		[TestCase(typeof(float), true)]
		[TestCase(typeof(int), false)]
		[TestCase(typeof(long), false)]
		[TestCase(typeof(sbyte), false)]
		[TestCase(typeof(short), false)]
		[TestCase(typeof(uint), false)]
		[TestCase(typeof(ulong), false)]
		[TestCase(typeof(ushort), false)]
		[TestCase(typeof(string), false)]
		public void IsDecimalNumericTest(Type value, bool expected)
		{
			Assert.AreEqual(expected, value.IsDecimalNumeric());
		}

		[TestCase(typeof(byte), true)]
		[TestCase(typeof(decimal), false)]
		[TestCase(typeof(double), false)]
		[TestCase(typeof(float), false)]
		[TestCase(typeof(int), true)]
		[TestCase(typeof(long), true)]
		[TestCase(typeof(sbyte), true)]
		[TestCase(typeof(short), true)]
		[TestCase(typeof(uint), true)]
		[TestCase(typeof(ulong), true)]
		[TestCase(typeof(ushort), true)]
		[TestCase(typeof(string), false)]
		public void IsIntegerNumericTest(Type value, bool expected)
		{
			Assert.AreEqual(expected, value.IsIntegerNumeric());
		}

		[Test]
		public void GetAssemblyTest()
		{
			Assembly assembly = typeof(TypeExtensionsTest).GetAssembly();
			Assert.IsTrue(assembly.FullName.Contains("ICD.Common.Utils"));
		}

		[TestCase(typeof(string), typeof(object), true)]
		[TestCase(typeof(object), typeof(string), false)]
		public void IsAssignableToTest(Type a, Type b, bool expected)
		{
			Assert.AreEqual(expected, a.IsAssignableTo(b));
		}

		[Test]
		public void GetAllTypesTest()
		{
			Type[] allTypes = typeof(B).GetAllTypes().ToArray();

			Assert.AreEqual(6, allTypes.Length);

			Assert.IsTrue(allTypes.Contains(typeof(E)));
			Assert.IsTrue(allTypes.Contains(typeof(D)));
			Assert.IsTrue(allTypes.Contains(typeof(C)));
			Assert.IsTrue(allTypes.Contains(typeof(B)));
			Assert.IsTrue(allTypes.Contains(typeof(A)));
			Assert.IsTrue(allTypes.Contains(typeof(object)));
		}

		[Test]
		public void GetBaseTypesTest()
		{
			Type[] baseTypes = typeof(B).GetBaseTypes().ToArray();

			Assert.AreEqual(2, baseTypes.Length);

			Assert.IsFalse(baseTypes.Contains(typeof(B)));
			Assert.IsTrue(baseTypes.Contains(typeof(A)));
			Assert.IsTrue(baseTypes.Contains(typeof(object)));
		}

		[Test]
		public void GetImmediateInterfacesTest()
		{
			Type[] interfaces = typeof(ICollection<int>).GetImmediateInterfaces().ToArray();

			Assert.AreEqual(1, interfaces.Length);
			Assert.AreEqual(typeof(IEnumerable<int>), interfaces[0]);

			interfaces = typeof(IEnumerable<int>).GetImmediateInterfaces().ToArray();

			Assert.AreEqual(1, interfaces.Length);
			Assert.AreEqual(typeof(IEnumerable), interfaces[0]);
		}

		[Test]
		public void GetMinimalInterfacesTest()
		{
			Assert.Inconclusive();
		}

		[TestCase(typeof(int), "Int32")]
		[TestCase(typeof(List<int>), "List")]
		public void GetNameWithoutGenericArityTest(Type type, string expected)
		{
			Assert.AreEqual(expected, type.GetNameWithoutGenericArity());
		}

		[TestCase(typeof(string), "string")]
		[TestCase(typeof(int?), "int?")]
		[TestCase(typeof(List<int?>), "List<int?>")]
		public void GetSyntaxNameTest(Type type, string expected)
		{
			Assert.AreEqual(expected, type.GetSyntaxName());
		}

		private interface C
		{
		}

		private interface D
		{
		}

		private interface E : C, D
		{
		}

		private class A
		{
		}

		private class B : A, E
		{
		}
	}
}
