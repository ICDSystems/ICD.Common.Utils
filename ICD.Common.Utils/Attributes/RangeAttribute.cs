using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils.Attributes
{
	/// <summary>
	/// Indicates the valid ranges for a given value if that range is not equal to the range of the data type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | 
					AttributeTargets.Field | 
					AttributeTargets.Parameter | 
					AttributeTargets.ReturnValue)]
	public sealed class RangeAttribute : AbstractIcdAttribute
	{
		/// <summary>
		/// Remaps from the source numeric min/max to double min/max.
		/// </summary>
		private static readonly Dictionary<Type, Func<object, double>> s_RemapToDouble =
			new Dictionary<Type, Func<object, double>>
			{
				// Duh
				{ typeof(double), o => (double)o},

				// Signed - Clamping prevents an overflow due to loss of precision
				{ typeof(short), o => MathUtils.Clamp(Convert.ToDouble(o) / short.MaxValue, -1, 1) * double.MaxValue},
				{ typeof(int), o => MathUtils.Clamp(Convert.ToDouble(o) / int.MaxValue, -1, 1) * double.MaxValue},
				{ typeof(long), o => MathUtils.Clamp(Convert.ToDouble(o) / long.MaxValue, -1, 1) * double.MaxValue},
				{ typeof(float),  o => MathUtils.Clamp(Convert.ToDouble(o) / float.MaxValue, -1, 1) * double.MaxValue},
				{ typeof(decimal),  o => MathUtils.Clamp(Convert.ToDouble(o) / (double)decimal.MaxValue, -1, 1) * double.MaxValue},

				// Unsigned
				{ typeof(ushort), o => MathUtils.Clamp((Convert.ToDouble(o) / ushort.MaxValue - 0.5) * 2, -1, 1) * double.MaxValue},
				{ typeof(uint), o => MathUtils.Clamp((Convert.ToDouble(o) / uint.MaxValue - 0.5) * 2, -1, 1) * double.MaxValue},
				{ typeof(ulong), o => MathUtils.Clamp((Convert.ToDouble(o) / ulong.MaxValue - 0.5) * 2, -1, 1) * double.MaxValue},
				{ typeof(byte), o => MathUtils.Clamp((Convert.ToDouble(o) / byte.MaxValue - 0.5) * 2, -1, 1) * double.MaxValue}
			};

		/// <summary>
		/// Remaps from the double min/max to target numeric min/max.
		/// </summary>
		private static readonly Dictionary<Type, Func<double, object>> s_RemapFromDouble =
			new Dictionary<Type, Func<double, object>>
			{
				// Duh
				{typeof(double), v => v},

				// Signed
				{typeof(short), v => (short)(v / double.MaxValue * short.MaxValue)},
				{typeof(int), v => (int)(v / double.MaxValue * int.MaxValue)},
				{typeof(long), v => (long)(v / double.MaxValue * long.MaxValue)},
				{typeof(float), v => (float)(v / double.MaxValue * float.MaxValue)},
				{typeof(decimal), v => (decimal)(v / double.MaxValue) * decimal.MaxValue},

				// Unsigned
				{typeof(ushort), v => (ushort)((v / double.MaxValue + 1) / 2 * ushort.MaxValue)},
				{typeof(uint), v => (uint)((v / double.MaxValue + 1) / 2 * uint.MaxValue)},
				{typeof(ulong), v => (ulong)((v / double.MaxValue + 1) / 2 * ulong.MaxValue)},
				{typeof(byte), v => (byte)((v / double.MaxValue + 1) / 2 * byte.MaxValue)}
			};

		private readonly object m_Min;
		private readonly object m_Max;

		#region Properties

		/// <summary>
		/// Gets the min value for this range.
		/// </summary>
		public object Min { get { return m_Min; } }

		/// <summary>
		/// Gets the max value for this range.
		/// </summary>
		public object Max { get { return m_Max; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public RangeAttribute(ushort min, ushort max)
			: this((object)min, (object)max)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public RangeAttribute(short min, short max)
			: this((object)min, (object)max)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public RangeAttribute(uint min, uint max)
			: this((object)min, (object)max)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public RangeAttribute(int min, int max)
			: this((object)min, (object)max)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public RangeAttribute(ulong min, ulong max)
			: this((object)min, (object)max)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public RangeAttribute(long min, long max)
			: this((object)min, (object)max)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public RangeAttribute(float min, float max)
			: this((object)min, (object)max)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public RangeAttribute(double min, double max)
			: this((object)min, (object)max)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public RangeAttribute(byte min, byte max)
			: this((object)min, (object)max)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public RangeAttribute(sbyte min, sbyte max)
			: this((object)min, (object)max)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public RangeAttribute(decimal min, decimal max)
			: this((object)min, (object)max)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public RangeAttribute(object min, object max)
		{
			if (min == null)
				throw new ArgumentNullException("min");

			if (max == null)
				throw new ArgumentNullException("max");

			if (min.GetType() != max.GetType())
				throw new ArgumentException("Min and Max types do not match");

			if (!min.GetType().IsNumeric())
				throw new ArgumentException("Given types are not numeric");

			m_Min = min;
			m_Max = max;
		}

		#endregion

		#region Methods

		#region Remap To

		/// <summary>
		/// Remaps the given numeric value from its min/max range into double min/max range.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static double RemapToDouble(object value)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			Func<object, double> remap;
			if (!s_RemapToDouble.TryGetValue(value.GetType(), out remap))
				throw new NotSupportedException("Value type is not supported.");

			return remap(value);
		}

		/// <summary>
		/// Remaps the given numeric value from its min/max range into ushort min/max range.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static ushort RemapToUShort(object value)
		{
			return (ushort)(ushort.MaxValue * (RemapToDouble(value) / double.MaxValue + 1) / 2);
		}

		#endregion

		#region Remap From

		/// <summary>
		/// Remaps the given double value from its min/max range into the target type min/max range.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object RemapFromDouble(double value, Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			Func<double, object> remap;
			if (!s_RemapFromDouble.TryGetValue(type, out remap))
				throw new NotSupportedException("Value type is not supported.");

			return remap(value);
		}

		/// <summary>
		/// Remaps the given ushort value from its min/max range into the target type min/max range.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object RemapFromUShort(ushort value, Type type)
		{
			double intermediate = RemapToDouble(value);
			return RemapFromDouble(intermediate, type);
		}

		#endregion

		#region Remap To Attribute And Clamp

		/// <summary>
		/// Remaps the given numeric value from its min/max range into the min/max range of this attribute,
		/// clamps, and then remaps the result to double.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public double RemapAndClampToDouble(object value)
		{
			double intermediate = RemapToDouble(value);
			double min = RemapToDouble(Min);
			double max = RemapToDouble(Max);

			return MathUtils.Clamp(intermediate, min, max);
		}

		/// <summary>
		/// Remaps the given numeric value from its min/max range into the min/max range of this attribute,
		/// clamps, and then remaps the result to ushort.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public ushort RemapAndClampToUShort(object value)
		{
			double intermediate = RemapAndClampToDouble(value);
			return RemapToUShort(intermediate);
		}

		#endregion

		#region Remap And Clamp

		/// <summary>
		/// Remaps the ushort value from its min/max range into the min/max range of this attribute,
		/// clamps and returns the result.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public object RemapAndClamp(double value)
		{
			double min = RemapToDouble(Min);
			double max = RemapToDouble(Max);

			double clamp = MathUtils.Clamp(value, min, max);

			return RemapFromDouble(clamp, m_Min.GetType());
		}

		/// <summary>
		/// Remaps the ushort value from its min/max range into the min/max range of this attribute,
		/// clamps and returns the result.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public object RemapAndClamp(ushort value)
		{
			double intermediate = RemapToDouble(value);
			return RemapAndClamp(intermediate);
		}

		#endregion

		#endregion
	}
}