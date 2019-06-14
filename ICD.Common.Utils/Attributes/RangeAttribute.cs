using System;
using System.Collections.Generic;
using System.Globalization;
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
		private static readonly Dictionary<Type, Func<double, double>> s_Clamp =
			new Dictionary<Type, Func<double, double>>
			{
				// Duh
				{typeof(double), o => o},

				// Signed
				{typeof(short), o => o < short.MinValue ? short.MinValue : o > short.MaxValue ? short.MaxValue : o},
				{typeof(int), o => o < int.MinValue ? int.MinValue : o > int.MaxValue ? int.MaxValue : o},
				{typeof(long), o => o < long.MinValue ? long.MinValue : o > long.MaxValue ? long.MaxValue : o},
				{typeof(float),o => o < float.MinValue ? float.MinValue : o > float.MaxValue ? float.MaxValue : o},
				{typeof(decimal), o => o < (double)decimal.MinValue ? (double)decimal.MinValue : o > (double)decimal.MaxValue ? (double)decimal.MaxValue : o},

				// Unsigned
				{typeof(ushort), o => o < ushort.MinValue ? ushort.MinValue : o > ushort.MaxValue ? ushort.MaxValue : o},
				{typeof(uint), o => o < uint.MinValue ? uint.MinValue : o > uint.MaxValue ? uint.MaxValue : o},
				{typeof(ulong), o => o < ulong.MinValue ? ulong.MinValue : o > ulong.MaxValue ? ulong.MaxValue : o},
				{typeof(byte), o => o < byte.MinValue ? byte.MinValue : o > byte.MaxValue ? byte.MaxValue : o}
			};

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

		/// <summary>
		/// Gets the min value of a given numeric type as a double.
		/// </summary>
		private static readonly Dictionary<Type, double> s_MinAsDouble =
			new Dictionary<Type, double>
			{
				// Duh
				{typeof(double), double.MinValue},

				// Signed
				{typeof(short), Convert.ToDouble(short.MinValue)},
				{typeof(int), Convert.ToDouble(int.MinValue)},
				{typeof(long), Convert.ToDouble(long.MinValue)},
				{typeof(float), Convert.ToDouble(float.MinValue)},
				{typeof(decimal), Convert.ToDouble(decimal.MinValue)},

				// Unsigned
				{typeof(ushort), Convert.ToDouble(ushort.MinValue)},
				{typeof(uint), Convert.ToDouble(uint.MinValue)},
				{typeof(ulong), Convert.ToDouble(ulong.MinValue)},
				{typeof(byte), Convert.ToDouble(byte.MinValue)}
			};

		/// <summary>
		/// Gets the min value of a given numeric type as a double.
		/// </summary>
		private static readonly Dictionary<Type, double> s_MaxAsDouble =
			new Dictionary<Type, double>
			{
				// Duh
				{typeof(double), double.MaxValue},

				// Signed
				{typeof(short), Convert.ToDouble(short.MaxValue)},
				{typeof(int), Convert.ToDouble(int.MaxValue)},
				{typeof(long), Convert.ToDouble(long.MaxValue)},
				{typeof(float), Convert.ToDouble(float.MaxValue)},
				{typeof(decimal), Convert.ToDouble(decimal.MaxValue)},

				// Unsigned
				{typeof(ushort), Convert.ToDouble(ushort.MaxValue)},
				{typeof(uint), Convert.ToDouble(uint.MaxValue)},
				{typeof(ulong), Convert.ToDouble(ulong.MaxValue)},
				{typeof(byte), Convert.ToDouble(byte.MaxValue)}
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
		/// Clamps the given numeric value into the valid ranges of the target numeric type.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object Clamp(object value, Type type)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			if (type == null)
				throw new ArgumentNullException("type");

			if (!type.IsNumeric())
				throw new ArgumentException("Target type is not numeric");

			if (!value.GetType().IsNumeric())
				throw new ArgumentException("Source value is not numeric");

			double doubleValue = Convert.ToDouble(value);
			double clamped = Clamp(doubleValue, type);

			return Convert.ChangeType(clamped, value.GetType(), CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Clamps the given double value into the valid ranges of the target numeric type.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static double Clamp(double value, Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			Func<double, double> clamp;
			if (!s_Clamp.TryGetValue(type, out clamp))
				throw new NotSupportedException("Value type is not supported.");

			return clamp(value);
		}

		/// <summary>
		/// Remaps the numeric value into the min-max range of the target numeric type.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object Remap(object value, Type type)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			if (type == null)
				throw new ArgumentNullException("type");

			if (!type.IsNumeric())
				throw new ArgumentException("Target type is not numeric");

			if (!value.GetType().IsNumeric())
				throw new ArgumentException("Source value is not numeric");

			double intermediate = RemapToDouble(value);
			return RemapFromDouble(intermediate, type);
		}

		/// <summary>
		/// Clamps the given numeric value to the defined min/max then remaps to the target numeric type.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public object ClampMinMaxThenRemap(object value, Type type)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			if (type == null)
				throw new ArgumentNullException("type");

			if (!type.IsNumeric())
				throw new ArgumentException("Target type is not numeric");

			if (!value.GetType().IsNumeric())
				throw new ArgumentException("Source value is not numeric");

			double min = Convert.ToDouble(Min);
			double max = Convert.ToDouble(Max);
			double doubleValue = Convert.ToDouble(value);

			double clamped = MathUtils.Clamp(doubleValue, min, max);
			object remapped = RemapMinMax(clamped, type);

			return Convert.ChangeType(remapped, value.GetType(), CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Remaps the given numeric value to the defined min/max.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public object RemapMinMax(object value)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			if (!value.GetType().IsNumeric())
				throw new ArgumentException("Source value is not numeric");

			double sourceMin = GetMinAsDouble(value.GetType());
			double sourceMax = GetMaxAsDouble(value.GetType());

			double targetMin = Convert.ToDouble(Min);
			double targetMax = Convert.ToDouble(Max);

			double doubleValue = Convert.ToDouble(value);

			double remapped = MathUtils.MapRange(sourceMin, sourceMax, targetMin, targetMax, doubleValue);

			return Convert.ChangeType(remapped, value.GetType(), CultureInfo.InvariantCulture);
		}

		private object RemapMinMax(object value, Type type)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			if (type == null)
				throw new ArgumentNullException("type");

			if (!type.IsNumeric())
				throw new ArgumentException("Target type is not numeric");

			if (!value.GetType().IsNumeric())
				throw new ArgumentException("Source value is not numeric");

			double sourceMin = Convert.ToDouble(Min);
			double sourceMax = Convert.ToDouble(Max);

			double targetMin = GetMinAsDouble(type);
			double targetMax = GetMaxAsDouble(type);

			double doubleValue = Convert.ToDouble(value);

			double remapped = MathUtils.MapRange(sourceMin, sourceMax, targetMin, targetMax, doubleValue);

			return Convert.ChangeType(remapped, type, CultureInfo.InvariantCulture);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the min value for the given numeric type as a double.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static double GetMinAsDouble(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (!type.IsNumeric())
				throw new ArgumentException("Target type is not numeric");

			double min;
			if (!s_MinAsDouble.TryGetValue(type, out min))
				throw new NotSupportedException("Type is not supported.");

			return min;
		}

		/// <summary>
		/// Gets the max value for the given numeric type as a double.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static double GetMaxAsDouble(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (!type.IsNumeric())
				throw new ArgumentException("Target type is not numeric");

			double max;
			if (!s_MaxAsDouble.TryGetValue(type, out max))
				throw new NotSupportedException("Type is not supported.");

			return max;
		}

		#endregion
	}
}