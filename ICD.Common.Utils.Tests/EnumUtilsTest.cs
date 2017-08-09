using NUnit.Framework;
using System;
using System.Linq;

namespace ICD.Common.Utils.Tests_NetStandard
{
	[TestFixture]
    public sealed class EnumUtilsTest
    {
		public enum eTestEnum
		{
			None = 0,
			A = 1, 
			B = 2,
			C = 3,
		}

		[Flags]
		public enum eTestFlagsEnum
		{
			None = 0,
			A = 1,
			B = 2,
			C = 4
		}
		
		[Test]
		public void GetValuesTest()
		{
			eTestEnum[] values = EnumUtils.GetValues<eTestEnum>().ToArray();

			Assert.AreEqual(4, values.Length);
			Assert.AreEqual(eTestEnum.None, values[0]);
			Assert.AreEqual(eTestEnum.A, values[1]);
			Assert.AreEqual(eTestEnum.B, values[2]);
			Assert.AreEqual(eTestEnum.C, values[3]);
		}
		
		[Test]
		public void IsEnumTypeTest()
		{
			Assert.IsTrue(EnumUtils.IsEnumType(typeof(eTestEnum)));
			Assert.IsTrue(EnumUtils.IsEnumType(typeof(eTestFlagsEnum)));
			Assert.IsTrue(EnumUtils.IsEnumType(typeof(Enum)));
			Assert.IsFalse(EnumUtils.IsEnumType(typeof(EnumUtilsTest)));
			Assert.Throws<ArgumentNullException>(() => EnumUtils.IsEnumType(null));
		}

		[Test]
		public void IsEnumTypeGenericTest()
		{
			Assert.IsTrue(EnumUtils.IsEnumType<eTestEnum>());
			Assert.IsTrue(EnumUtils.IsEnumType<eTestFlagsEnum>());
			Assert.IsTrue(EnumUtils.IsEnumType<Enum>());
			Assert.IsFalse(EnumUtils.IsEnumType<EnumUtilsTest>());
		}

		[Test]
		public void IsEnumTest()
		{
			Assert.IsTrue(EnumUtils.IsEnum(eTestEnum.A));
			Assert.IsTrue(EnumUtils.IsEnum(eTestFlagsEnum.A));
			Assert.IsTrue(EnumUtils.IsEnum(eTestEnum.A as object));
			Assert.IsTrue(EnumUtils.IsEnum(eTestEnum.A as Enum));
			Assert.IsFalse(EnumUtils.IsEnum(null));
			Assert.IsFalse(EnumUtils.IsEnum(""));
		}

		#region Values

		[Test]
		public void GetUnderlyingValueTest()
		{
			Assert.AreEqual(0, EnumUtils.GetUnderlyingValue(eTestEnum.None));
			Assert.AreEqual(1, EnumUtils.GetUnderlyingValue(eTestEnum.A));
			Assert.AreEqual(2, EnumUtils.GetUnderlyingValue(eTestEnum.B));
			Assert.AreEqual(3, EnumUtils.GetUnderlyingValue(eTestEnum.C));
		}

		[Test]
		public void GetValuesGenericTest()
		{
			eTestEnum[] values = EnumUtils.GetValues<eTestEnum>().ToArray();

			Assert.AreEqual(4, values.Length);
			Assert.IsTrue(values.Contains(eTestEnum.None));
			Assert.IsTrue(values.Contains(eTestEnum.A));
			Assert.IsTrue(values.Contains(eTestEnum.B));
			Assert.IsTrue(values.Contains(eTestEnum.C));
		}

		[Test]
		public void GetNoneValueGenericTest()
		{
			Assert.AreEqual(eTestEnum.None, EnumUtils.GetNoneValue<eTestEnum>());
			Assert.AreEqual(eTestFlagsEnum.None, EnumUtils.GetNoneValue<eTestFlagsEnum>());
		}

		[Test]
		public void GetValuesExceptNoneGenericTest()
		{
			eTestEnum[] values = EnumUtils.GetValuesExceptNone<eTestEnum>().ToArray();

			Assert.AreEqual(3, values.Length);
			Assert.IsFalse(values.Contains(eTestEnum.None));
			Assert.IsTrue(values.Contains(eTestEnum.A));
			Assert.IsTrue(values.Contains(eTestEnum.B));
			Assert.IsTrue(values.Contains(eTestEnum.C));
		}

		[Test]
		public void GetValuesExceptNoneTest()
		{
			object[] values = EnumUtils.GetValuesExceptNone(typeof(eTestEnum)).ToArray();

			Assert.AreEqual(3, values.Length);
			Assert.IsFalse(values.Contains(eTestEnum.None));
			Assert.IsTrue(values.Contains(eTestEnum.A));
			Assert.IsTrue(values.Contains(eTestEnum.B));
			Assert.IsTrue(values.Contains(eTestEnum.C));
		}

		#endregion

		#region Flags

		[Test]
		public void IsFlagsEnumGenericTest()
		{
			Assert.IsFalse(EnumUtils.IsFlagsEnum<eTestEnum>());
			Assert.IsTrue(EnumUtils.IsFlagsEnum<eTestFlagsEnum>());
		}

		[Test]
		public void IsFlagsEnumTest()
		{
			Assert.IsFalse(EnumUtils.IsFlagsEnum(typeof(eTestEnum)));
			Assert.IsTrue(EnumUtils.IsFlagsEnum(typeof(eTestFlagsEnum)));
		}

		[Test]
		public void GetFlagsIntersectionGenericTest()
		{
			Assert.AreEqual(eTestFlagsEnum.B, EnumUtils.GetFlagsIntersection(eTestFlagsEnum.A | eTestFlagsEnum.B, eTestFlagsEnum.B | eTestFlagsEnum.C));
		}

		[Test]
		public void GetFlagsGenericTest()
		{
			eTestFlagsEnum a = EnumUtils.GetFlagsAllValue<eTestFlagsEnum>();
			eTestFlagsEnum[] aValues = EnumUtils.GetFlags(a).ToArray();

			Assert.AreEqual(4, aValues.Length);
			Assert.IsTrue(aValues.Contains(eTestFlagsEnum.None));
			Assert.IsTrue(aValues.Contains(eTestFlagsEnum.A));
			Assert.IsTrue(aValues.Contains(eTestFlagsEnum.B));
			Assert.IsTrue(aValues.Contains(eTestFlagsEnum.C));
		}

		[Test]
		public void GetFlagsExceptNoneGenericTest()
		{
			eTestFlagsEnum a = EnumUtils.GetFlagsAllValue<eTestFlagsEnum>();
			eTestFlagsEnum[] aValues = EnumUtils.GetFlagsExceptNone(a).ToArray();

			Assert.AreEqual(3, aValues.Length);
			Assert.IsFalse(aValues.Contains(eTestFlagsEnum.None));
			Assert.IsTrue(aValues.Contains(eTestFlagsEnum.A));
			Assert.IsTrue(aValues.Contains(eTestFlagsEnum.B));
			Assert.IsTrue(aValues.Contains(eTestFlagsEnum.C));
		}

		[Test]
		public void GetFlagsAllValueGenericTest()
		{
			eTestFlagsEnum value = EnumUtils.GetFlagsAllValue<eTestFlagsEnum>();
			Assert.AreEqual(eTestFlagsEnum.None | eTestFlagsEnum.A | eTestFlagsEnum.B | eTestFlagsEnum.C, value);
		}

		[Test]
		public void HasFlagGenericTest()
		{
			Assert.IsTrue(EnumUtils.HasFlag(eTestFlagsEnum.A | eTestFlagsEnum.B, eTestFlagsEnum.A));
			Assert.IsFalse(EnumUtils.HasFlag(eTestFlagsEnum.A | eTestFlagsEnum.B, eTestFlagsEnum.C));
		}

		[Test]
		public void HasFlagsGenericTest()
		{
			Assert.IsTrue(EnumUtils.HasFlags(eTestFlagsEnum.A | eTestFlagsEnum.B, eTestFlagsEnum.A | eTestFlagsEnum.B));
			Assert.IsFalse(EnumUtils.HasFlags(eTestFlagsEnum.A | eTestFlagsEnum.B, eTestFlagsEnum.A | eTestFlagsEnum.C));
		}

		[Test]
		public void HasSingleFlagGenericTest()
		{
			Assert.IsTrue(EnumUtils.HasSingleFlag(eTestFlagsEnum.A));
			Assert.IsFalse(EnumUtils.HasSingleFlag(eTestFlagsEnum.None));
			Assert.IsFalse(EnumUtils.HasSingleFlag(eTestFlagsEnum.A | eTestFlagsEnum.B));
		}

		[Test]
		public void HasMultipleFlagsGenericTest()
		{
			Assert.IsFalse(EnumUtils.HasMultipleFlags(eTestFlagsEnum.A));
			Assert.IsFalse(EnumUtils.HasMultipleFlags(eTestFlagsEnum.None));
			Assert.IsTrue(EnumUtils.HasMultipleFlags(eTestFlagsEnum.A | eTestFlagsEnum.B));
		}

        [TestCase(false, eTestFlagsEnum.None)]
        [TestCase(true, eTestFlagsEnum.B)]
        [TestCase(true, eTestFlagsEnum.B | eTestFlagsEnum.C)]
        public void HasAnyFlagsTest(bool expected, eTestFlagsEnum value)
        {
            Assert.AreEqual(expected, EnumUtils.HasAnyFlags(value));
        }

        [TestCase(false, eTestFlagsEnum.None, eTestFlagsEnum.None)]
        [TestCase(false, eTestFlagsEnum.None, eTestFlagsEnum.B)]
        [TestCase(true, eTestFlagsEnum.B, eTestFlagsEnum.B)]
        [TestCase(false, eTestFlagsEnum.None | eTestFlagsEnum.A, eTestFlagsEnum.B | eTestFlagsEnum.C)]
        public void HasAnyFlagsValueTest(bool expected, eTestFlagsEnum value, eTestFlagsEnum other)
        {
            Assert.AreEqual(expected, EnumUtils.HasAnyFlags(value, other));
        }

        #endregion

        #region Conversion

        [Test]
		public void ParseGenericTest()
		{
			Assert.AreEqual(eTestEnum.A, EnumUtils.Parse<eTestEnum>("A", false));
			Assert.AreEqual(eTestEnum.A, EnumUtils.Parse<eTestEnum>("a", true));
		}

		[Test]
		public void TryParseGenericTest()
		{
			eTestEnum output;

			Assert.IsTrue(EnumUtils.TryParse("A", false, out output));
			Assert.AreEqual(eTestEnum.A, output);

			Assert.IsTrue(EnumUtils.TryParse("a", true, out output));
			Assert.AreEqual(eTestEnum.A, output);

			Assert.IsFalse(EnumUtils.TryParse("derp", true, out output));
			Assert.AreEqual(default(eTestEnum), output);
		}

		[Test]
		public void ToEnumGenericTest()
		{
			Assert.AreEqual(eTestEnum.A, EnumUtils.ToEnum(eTestEnum.A));
			Assert.AreNotEqual(eTestEnum.B, EnumUtils.ToEnum(eTestEnum.A));
		}

		[Test]
		public void ToEnumTest()
		{
			Assert.AreEqual(eTestEnum.A, EnumUtils.ToEnum((object)eTestEnum.A));
			Assert.AreNotEqual(eTestEnum.B, EnumUtils.ToEnum((object)eTestEnum.A));
		}

		#endregion
	}
}
