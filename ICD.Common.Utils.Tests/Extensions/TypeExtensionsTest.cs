using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Extensions;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Extensions
{
	[TestFixture]
	public sealed class TypeExtensionsTest
	{
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
