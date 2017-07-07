using NUnit.Framework;
using System;

namespace ICD.Common.Utils.Tests_NetStandard
{
	[TestFixture]
    public sealed class EnumUtilsTest
    {
		private enum eTestEnum
		{
			A, 
			B,
			C
		}

		[Test]
		public void IsEnumTypeTest()
		{
			Assert.IsTrue(EnumUtils.IsEnumType(typeof(eTestEnum)));
			Assert.IsTrue(EnumUtils.IsEnumType(typeof(Enum)));
			Assert.IsFalse(EnumUtils.IsEnumType(typeof(EnumUtilsTest)));
			Assert.Throws<ArgumentNullException>(() => EnumUtils.IsEnumType(null));
		}

		[Test]
		public void IsEnumTypeGenericTest()
		{
			Assert.IsTrue(EnumUtils.IsEnumType<eTestEnum>());
			Assert.IsTrue(EnumUtils.IsEnumType<Enum>());
			Assert.IsFalse(EnumUtils.IsEnumType<EnumUtilsTest>());
		}

		[Test]
		public void IsEnumTest()
		{
			Assert.IsTrue(EnumUtils.IsEnum(eTestEnum.A));
			Assert.IsTrue(EnumUtils.IsEnum(eTestEnum.A as object));
			Assert.IsTrue(EnumUtils.IsEnum(eTestEnum.A as Enum));
			Assert.IsFalse(EnumUtils.IsEnum(null));
			Assert.IsFalse(EnumUtils.IsEnum(""));
		}

		#region Values

		[Test]
		public void GetUnderlyingValueTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void GetValuesGenericTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void GetValuesTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void GetNoneValueGenericTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void GetValuesExceptNoneGenericTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void GetValuesExceptNoneTest()
		{
			Assert.Inconclusive();
		}

		#endregion

		#region Flags

		[Test]
		public void IsFlagsEnumGenericTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void IsFlagsEnumTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void GetFlagsIntersectionGenericTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void GetFlagsGenericTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void GetFlagsExceptNoneGenericTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void GetFlagsAllValueGenericTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void HasFlagGenericTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void HasFlagsGenericTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void HasSingleFlagGenericTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void HasMultipleFlagsGenericTest()
		{
			Assert.Inconclusive();
		}

		#endregion

		#region Conversion

		[Test]
		public void ParseGenericTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void TryParseGenericTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void ToEnumGenericTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void ToEnumTest()
		{
			Assert.Inconclusive();
		}

		#endregion
	}
}
