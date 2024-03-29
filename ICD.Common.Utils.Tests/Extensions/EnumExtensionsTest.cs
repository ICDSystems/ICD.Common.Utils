﻿using System;
using ICD.Common.Utils.Extensions;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Extensions
{
	[TestFixture]
	public sealed class EnumExtensionsTest
	{
		[Flags]
		public enum eTestEnum
		{
			A = 1,
			B = 2,
			C = 4
		}

		[TestCase(eTestEnum.A, eTestEnum.A, true)]
		[TestCase(eTestEnum.A | eTestEnum.B, eTestEnum.A, true)]
		[TestCase(eTestEnum.A | eTestEnum.B, eTestEnum.C, false)]
		public void HasFlagTest(eTestEnum value, eTestEnum flag, bool expected)
		{
			Assert.AreEqual(expected, EnumExtensions.HasFlag(value, flag));
		}

		[TestCase(eTestEnum.A, eTestEnum.A, true)]
		[TestCase(eTestEnum.A | eTestEnum.B, eTestEnum.A, true)]
		[TestCase(eTestEnum.A | eTestEnum.B, eTestEnum.C, false)]
		[TestCase(eTestEnum.A, eTestEnum.A | eTestEnum.B, false)]
		[TestCase(eTestEnum.A | eTestEnum.B, eTestEnum.A | eTestEnum.B, true)]
		public void HasFlagsTest(eTestEnum value, eTestEnum flags, bool expected)
		{
			Assert.AreEqual(expected, value.HasFlags(flags));
		}

		[TestCase(eTestEnum.A, eTestEnum.B)]
		[TestCase(eTestEnum.B, eTestEnum.C)]
		[TestCase(eTestEnum.C, eTestEnum.A)]
		[TestCase(eTestEnum.A | eTestEnum.B, null)]
		public void CycleNextTest(eTestEnum value, eTestEnum? expected)
		{
			if (EnumUtils.HasMultipleFlags(value))
				Assert.Catch(typeof(InvalidOperationException), () => value.CycleNext());
			else
				Assert.AreEqual(expected, value.CycleNext());
		}
	}
}
