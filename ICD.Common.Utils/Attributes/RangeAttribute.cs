using System;
using ICD.Common.Properties;

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
	public sealed class RangeAttribute : AbstractIcdAttribute
	{
		#region Properties

		[NotNull]
		public object Min { get; private set; }

		[NotNull]
		public object Max { get; private set; }

		[NotNull]
		private Type Type { get { return Min.GetType(); } }

		#endregion

		#region Constructors

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

		#endregion

		#region Methods

		/// <summary>
		/// Returns true if the given value is within the range of Min to Max.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool IsInRange(object value)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			if (value.GetType() != Type)
				throw new ArgumentException("the type of value does not match the type of min / max");

			if (value is ushort)
			{
				var castMin = (ushort)Min;
				var castMax = (ushort)Max;
				var castVal = (ushort)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			if (value is short)
			{
				var castMin = (short)Min;
				var castMax = (short)Max;
				var castVal = (short)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			if (value is uint)
			{
				var castMin = (uint)Min;
				var castMax = (uint)Max;
				var castVal = (uint)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			if (value is int)
			{
				var castMin = (int)Min;
				var castMax = (int)Max;
				var castVal = (int)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			if (value is ulong)
			{
				var castMin = (ulong)Min;
				var castMax = (ulong)Max;
				var castVal = (ulong)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			if (value is long)
			{
				var castMin = (long)Min;
				var castMax = (long)Max;
				var castVal = (long)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			if (value is float)
			{
				var castMin = (float)Min;
				var castMax = (float)Max;
				var castVal = (float)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			if (value is double)
			{
				var castMin = (double)Min;
				var castMax = (double)Max;
				var castVal = (double)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			if (value is decimal)
			{
				var castMin = (decimal)Min;
				var castMax = (decimal)Max;
				var castVal = (decimal)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			if (value is byte)
			{
				var castMin = (byte)Min;
				var castMax = (byte)Max;
				var castVal = (byte)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			if (value is sbyte)
			{
				var castMin = (sbyte)Min;
				var castMax = (sbyte)Max;
				var castVal = (sbyte)value;
				return (castVal >= castMin && castVal <= castMax);
			}

			throw new ArgumentException("the type of value is not a numeric type.");
		}

		public ushort RemapRangeToUShort(double value)
		{
			return (ushort)MathUtils.MapRange((double)Min, (double)Max, ushort.MinValue, ushort.MaxValue, value);
		}

		public ushort RemapRangeToUShort(float value)
		{
			return (ushort)MathUtils.MapRange((float)Min, (float)Max, ushort.MinValue, ushort.MaxValue, value);
		}

		public ushort RemapRangeToUShort(int value)
		{
			return (ushort)MathUtils.MapRange((int)Min, (int)Max, ushort.MinValue, ushort.MaxValue, value);
		}

		public ushort RemapRangeToUShort(ushort value)
		{
			return MathUtils.MapRange((ushort)Min, (ushort)Max, ushort.MinValue, ushort.MaxValue, value);
		}

		#endregion
	}
}