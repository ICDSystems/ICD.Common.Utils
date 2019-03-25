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
	public sealed class RangeAttribute : AbstractIcdAttribute
	{
		#region Properties

		public object Min { get; private set; }
		public object Max { get; private set; }

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

		public T GetMin<T>()
		{
			return (T)Convert.ChangeType(Min, typeof(T), null);
		}

		public T GetMax<T>()
		{
			return (T)Convert.ChangeType(Max, typeof(T), null);
		}

		public bool IsInRange(object value)
		{
			if (value is ushort)
			{
				if (!(Min is ushort))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castVal = (ushort)value;
				return (castVal >= GetMin<ushort>() && castVal <= GetMax<ushort>());
			}

			if (value is short)
			{
				if (!(Min is short))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castVal = (short)value;
				return (castVal >= GetMin<short>() && castVal <= GetMax<short>());
			}

			if (value is uint)
			{
				if (!(Min is uint))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castVal = (uint)value;
				return (castVal >= GetMin<uint>() && castVal <= GetMax<uint>());
			}

			if (value is int)
			{
				if (!(Min is int))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castVal = (int)value;
				return (castVal >= GetMin<int>() && castVal <= GetMax<int>());
			}

			if (value is ulong)
			{
				if (!(Min is ulong))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castVal = (ulong)value;
				return (castVal >= GetMin<ulong>() && castVal <= GetMax<ulong>());
			}

			if (value is long)
			{
				if (!(Min is long))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castVal = (long)value;
				return (castVal >= GetMin<long>() && castVal <= GetMax<long>());
			}

			if (value is float)
			{
				if (!(Min is float))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castVal = (float)value;
				return (castVal >= GetMin<float>() && castVal <= GetMax<float>());
			}

			if (value is double)
			{
				if (!(Min is double))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castVal = (double)value;
				return (castVal >= GetMin<double>() && castVal <= GetMax<double>());
			}

			if (value is decimal)
			{
				if (!(Min is decimal))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castVal = (decimal)value;
				return (castVal >= GetMin<decimal>() && castVal <= GetMax<decimal>());
			}

			if (value is byte)
			{
				if (!(Min is byte))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castVal = (byte)value;
				return (castVal >= GetMin<byte>() && castVal <= GetMax<byte>());
			}

			if (value is sbyte)
			{
				if (!(Min is sbyte))
					throw new ArgumentException("the type of value does not match the type of min / max");

				var castVal = (sbyte)value;
				return (castVal >= GetMin<sbyte>() && castVal <= GetMax<sbyte>());
			}

			throw new ArgumentException("the type of value is not a numeric type.");
		}

		public ushort RemapRangeToUshort(double value)
		{
			return (ushort)MathUtils.MapRange(GetMin<double>(), GetMax<double>(), ushort.MinValue, ushort.MaxValue, value);
		}

		public ushort RemapRangeToUshort(float value)
		{
			return (ushort)MathUtils.MapRange(GetMin<float>(), GetMax<float>(), ushort.MinValue, ushort.MaxValue, value);
		}

		public ushort RemapRangeToUshort(int value)
		{
			return (ushort)MathUtils.MapRange(GetMin<int>(), GetMax<int>(), ushort.MinValue, ushort.MaxValue, value);
		}

		public ushort RemapRangeToUshort(ushort value)
		{
			return MathUtils.MapRange(GetMin<ushort>(), GetMax<ushort>(), ushort.MinValue, ushort.MaxValue, value);
		}

		#endregion
	}
}