using System;
using System.Linq;
using ICD.Common.Utils.Extensions;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Extensions
{
    [TestFixture]
    public sealed class TypeExtensionsTest
    {
        [Test]
        public void IsAssignableToTest()
        {
            Assert.IsTrue(typeof(string).IsAssignableTo(typeof(object)));
            Assert.IsFalse(typeof(object).IsAssignableTo(typeof(string)));
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