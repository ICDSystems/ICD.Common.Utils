using System;
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

		#region Remap To

		[TestCase(0, 0)]
		[TestCase(1.0, 1.0)]
		[TestCase(ushort.MaxValue, double.MaxValue)]
		[TestCase(short.MinValue, double.MinValue)]
		public void RemapToDoubleTest(object value, double expected)
		{
			Assert.AreEqual(expected, RangeAttribute.RemapToDouble(value));
		}

		[TestCase(0, (ushort)0)]
		[TestCase(1.0, (ushort)1)]
		[TestCase(double.MaxValue, ushort.MaxValue)]
		[TestCase(double.MinValue, ushort.MinValue)]
		public void RemapToUShortTest(object value, ushort expected)
		{
			Assert.AreEqual(expected, RangeAttribute.RemapToUShort(value));
		}

		#endregion

		#region Remap From

		[TestCase(0, typeof(ushort), (ushort)0)]
		[TestCase(double.MinValue, typeof(ushort), ushort.MinValue)]
		[TestCase(double.MaxValue, typeof(ushort), ushort.MaxValue)]
		public void RemapFromDoubleTest(double value, Type type, object expected)
		{
			Assert.AreEqual(expected, RangeAttribute.RemapFromDouble(value, type));
		}

		[TestCase((ushort)0, typeof(double), 0)]
		[TestCase(ushort.MinValue, typeof(double), double.MinValue)]
		[TestCase(ushort.MaxValue, typeof(double), double.MaxValue)]
		public void RemapFromUShortTest(ushort value, Type type, object expected)
		{
			Assert.AreEqual(expected, RangeAttribute.RemapFromUShort(value, type));
		}

		#endregion

		#region Remap To Attribute And Clamp
		
		public void RemapAndClampToDoubleTest(object value, object min, object max, double expected)
		{
			Assert.AreEqual(expected, new RangeAttribute(min, max).RemapAndClampToDouble(value));
		}
		
		public void RemapAndClampToUShortTest(object value, object min, object max, ushort expected)
		{
			Assert.AreEqual(expected, new RangeAttribute(min, max).RemapAndClampToDouble(value));
		}

		#endregion

		#region Remap And Clamp
		
		public void RemapAndClampFromDoubleTest(double value, object min, object max, object expected)
		{
			Assert.AreEqual(expected, new RangeAttribute(min, max).RemapAndClamp(value));
		}
		
		public void RemapAndClampFromUShortTest(ushort value, object min, object max, object expected)
		{
			Assert.AreEqual(expected, new RangeAttribute(min, max).RemapAndClamp(value));
		}

		#endregion

		#endregion
	}
}
