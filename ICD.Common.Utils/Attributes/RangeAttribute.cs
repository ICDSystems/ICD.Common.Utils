using System;

namespace ICD.Common.Utils.Attributes
{
	/// <summary>
	/// Indicates the valid ranges for a given value if that range is not equal to the range of the data type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | 
					AttributeTargets.Field | 
					AttributeTargets.Parameter | 
					AttributeTargets.ReturnValue,
					AllowMultiple = false,
					Inherited = true)]
	public class RangeAttribute : AbstractIcdAttribute
	{
		public object Min { get; private set; }
		public object Max { get; private set; }

		public RangeAttribute(ushort min, ushort max)
		{
			Min = min;
			Max = max;
		}

		public RangeAttribute(short min, short max)
		{
			Min = min;
			Max = max;
		}

		public RangeAttribute(uint min, uint max)
		{
			Min = min;
			Max = max;
		}

		public RangeAttribute(int min, int max)
		{
			Min = min;
			Max = max;
		}

		public RangeAttribute(ulong min, ulong max)
		{
			Min = min;
			Max = max;
		}

		public RangeAttribute(long min, long max)
		{
			Min = min;
			Max = max;
		}

		public RangeAttribute(float min, float max)
		{
			Min = min;
			Max = max;
		}

		public RangeAttribute(double min, double max)
		{
			Min = min;
			Max = max;
		}

		public RangeAttribute(byte min, byte max)
		{
			Min = min;
			Max = max;
		}

		public RangeAttribute(sbyte min, sbyte max)
		{
			Min = min;
			Max = max;
		}

		public RangeAttribute(decimal min, decimal max)
		{
			Min = min;
			Max = max;
		}

		public bool IsInRange(object value)
		{
			if (value is ushort)
			{
				if (!(Min is ushort))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castMin = (ushort)Min;
				var castMax = (ushort)Max;
				var castVal = (ushort)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			if (value is short)
			{
				if (!(Min is short))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castMin = (short)Min;
				var castMax = (short)Max;
				var castVal = (short)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			if (value is uint)
			{
				if (!(Min is uint))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castMin = (uint)Min;
				var castMax = (uint)Max;
				var castVal = (uint)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			if (value is int)
			{
				if (!(Min is int))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castMin = (int)Min;
				var castMax = (int)Max;
				var castVal = (int)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			if (value is ulong)
			{
				if (!(Min is ulong))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castMin = (ulong)Min;
				var castMax = (ulong)Max;
				var castVal = (ulong)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			if (value is long)
			{
				if (!(Min is long))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castMin = (long)Min;
				var castMax = (long)Max;
				var castVal = (long)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			if (value is float)
			{
				if (!(Min is float))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castMin = (float)Min;
				var castMax = (float)Max;
				var castVal = (float)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			if (value is double)
			{
				if (!(Min is double))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castMin = (double)Min;
				var castMax = (double)Max;
				var castVal = (double)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			if (value is decimal)
			{
				if (!(Min is decimal))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castMin = (decimal)Min;
				var castMax = (decimal)Max;
				var castVal = (decimal)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			if (value is byte)
			{
				if (!(Min is byte))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castMin = (byte)Min;
				var castMax = (byte)Max;
				var castVal = (byte)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			if (value is sbyte)
			{
				if (!(Min is sbyte))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castMin = (sbyte)Min;
				var castMax = (sbyte)Max;
				var castVal = (sbyte)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			throw new ArgumentException("the type of value is not a numeric type.");
		}

		public ushort RemapRangeToUshort(double value)
		{
			return (ushort)MathUtils.MapRange((double)Min, (double)Max, ushort.MinValue, ushort.MaxValue, value);
		}

		public ushort RemapRangeToUshort(float value)
		{
			return (ushort)MathUtils.MapRange((float)Min, (float)Max, ushort.MinValue, ushort.MaxValue, value);
		}

		public ushort RemapRangeToUshort(int value)
		{
			return (ushort)MathUtils.MapRange((int)Min, (int)Max, ushort.MinValue, ushort.MaxValue, value);
		}

		public ushort RemapRangeToUshort(ushort value)
		{
			return MathUtils.MapRange((ushort)Min, (ushort)Max, ushort.MinValue, ushort.MaxValue, value);
		}
	}
}