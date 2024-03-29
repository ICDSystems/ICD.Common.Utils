﻿using System;
using NUnit.Framework;
using RangeAttribute = ICD.Common.Utils.Attributes.RangeAttribute;

namespace ICD.Common.Utils.Tests.Attributes
{
	[TestFixture]
	public sealed class RangeAttributeTest : AbstractIcdAttributeTest<RangeAttribute>
	{
		#region Properties

		[TestCase(1)]
		[TestCase(1.0f)]
		[TestCase(1.0)]
		public void MinTest(object min)
		{
			Assert.AreEqual(min, new RangeAttribute(min, min).Min);
		}

		[TestCase(1)]
		[TestCase(1.0f)]
		[TestCase(1.0)]
		public void MaxTest(object max)
		{
			Assert.AreEqual(max, new RangeAttribute(max, max).Max);
		}

		#endregion

		#region Methods

		[TestCase((double)0, (double)0)]
		[TestCase((double)1, (double)1)]
		[TestCase(ushort.MaxValue, double.MaxValue)]
		[TestCase(short.MinValue, double.MinValue)]
		public void RemapToDoubleTest(object value, double expected)
		{
			Assert.AreEqual(expected, RangeAttribute.RemapToDouble(value));
		}

		[TestCase((double)0, typeof(ushort), (ushort)32767)]
		[TestCase(double.MinValue, typeof(ushort), ushort.MinValue)]
		[TestCase(double.MaxValue, typeof(ushort), ushort.MaxValue)]
		public void RemapFromDoubleTest(double value, Type type, object expected)
		{
			Assert.AreEqual(expected, RangeAttribute.RemapFromDouble(value, type));
		}

		[TestCase(short.MinValue, typeof(ushort), ushort.MinValue)]
		[TestCase(short.MaxValue, typeof(ushort), short.MaxValue)]
		public static void Clamp(object value, Type type, object expected)
		{
			Assert.AreEqual(expected, RangeAttribute.Clamp(value, type));
		}

		[TestCase(double.MinValue, typeof(ushort), ushort.MinValue)]
		[TestCase(double.MaxValue, typeof(ushort), ushort.MaxValue)]
		public void Clamp(double value, Type type, double expected)
		{
			Assert.AreEqual(expected, RangeAttribute.Clamp(value, type));
		}

		[TestCase(short.MinValue, typeof(ushort), ushort.MinValue)]
		[TestCase(short.MaxValue, typeof(ushort), ushort.MaxValue)]
		public void RemapTest(object value, Type type, object expected)
		{
			Assert.AreEqual(expected, RangeAttribute.Remap(value, type));
		}

		[TestCase(0, 100, ushort.MaxValue, typeof(ushort), ushort.MaxValue)]
		[TestCase(0, 100, ushort.MaxValue, typeof(short), short.MaxValue)]
		public void ClampMinMaxThenRemapTest(object min, object max, object value, Type type, object expected)
		{
			Assert.AreEqual(expected, new RangeAttribute(min, max).ClampMinMaxThenRemap(value, type));
		}

		[TestCase(0, 100, ushort.MaxValue, 100)]
		[TestCase(0, 100, ushort.MinValue, 0)]
		public void RemapMinMaxTest(object min, object max, object value, object expected)
		{
			Assert.AreEqual(expected, new RangeAttribute(min, max).RemapMinMax(value));
		}

		#endregion
	}
}
