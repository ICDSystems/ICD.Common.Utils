using System;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronXml;
#else
using System.Xml;
#endif

namespace ICD.Common.Utils.Xml
{
	public static class IcdXmlConvert
	{
		public static string ToString(int value)
		{
			return XmlConvert.ToString(value);
		}

		public static string ToString(bool value)
		{
			return XmlConvert.ToString(value);
		}

		public static string ToString(float value)
		{
			return XmlConvert.ToString(value);
		}

		public static string ToString(double value)
		{
			return XmlConvert.ToString(value);
		}

		public static string ToString(decimal value)
		{
			return XmlConvert.ToString(value);
		}

		public static string ToString(long value)
		{
			return XmlConvert.ToString(value);
		}

		public static string ToString(ulong value)
		{
			return XmlConvert.ToString(value);
		}

		public static string ToString(Guid value)
		{
			return XmlConvert.ToString(value);
		}

		public static string ToString(TimeSpan value)
		{
			return XmlConvert.ToString(value);
		}

		public static string ToString(int? value)
		{
			return value.HasValue ? ToString(value.Value) : null;
		}

		public static string ToString(object child)
		{
			if (child == null)
				return null;

			if (child is bool)
				return ToString((bool)child);
			if (child is byte)
				return ToString((byte)child);
			if (child is decimal)
				return ToString((decimal)child);
			if (child is char)
				return ToString((char)child);
			if (child is double)
				return ToString((double)child);
			if (child is Guid)
				return ToString((Guid)child);
			if (child is float)
				return ToString((float)child);
			if (child is int)
				return ToString((int)child);
			if (child is long)
				return ToString((long)child);
			if (child is sbyte)
				return ToString((sbyte)child);
			if (child is short)
				return ToString((short)child);
			if (child is TimeSpan)
				return ToString((TimeSpan)child);
			if (child is uint)
				return ToString((uint)child);
			if (child is ulong)
				return ToString((ulong)child);
			if (child is ushort)
				return ToString((ushort)child);

			return child.ToString();
		}
	}
}
