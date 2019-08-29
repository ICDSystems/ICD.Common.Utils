﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ICD.Common.Utils.IO;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronXml;
#else
using System.Xml;
#endif

namespace ICD.Common.Utils.Xml
{
	public static class IcdXmlConvert
	{
		/// <summary>
		/// Serializes the given instance to an xml string.
		/// </summary>
		/// <param name="elementName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string SerializeObject(string elementName, object value)
		{
			StringBuilder builder = new StringBuilder();

			using (IcdStringWriter stringWriter = new IcdStringWriter(builder))
			{
				using (IcdXmlTextWriter writer = new IcdXmlTextWriter(stringWriter))
					SerializeObject(writer, elementName, value);
			}

			return builder.ToString();
		}

		/// <summary>
		/// Serializes the given instance to xml.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="elementName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static void SerializeObject(IcdXmlTextWriter writer, string elementName, object value)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			IXmlConverter converter = XmlConverterAttribute.GetConverterForInstance(value);

			converter.WriteXml(writer, elementName, value);
		}

		/// <summary>
		/// Deserializes the given xml to an instance of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static T DeserializeObject<T>(string xml)
		{
			return (T)DeserializeObject(typeof(T), xml);
		}

		/// <summary>
		/// Deserializes the given xml to an instance of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static object DeserializeObject(Type type, string xml)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			using (IcdXmlReader reader = new IcdXmlReader(xml))
			{
				if (reader.ReadToNextElement())
					return DeserializeObject(type, reader);

				throw new FormatException("Expected element in XML");
			}
		}

		/// <summary>
		/// Deserializes the current node to an instance of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static T DeserializeObject<T>(IcdXmlReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			return (T)DeserializeObject(typeof(T), reader);
		}

		/// <summary>
		/// Deserializes the current node to an instance of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static object DeserializeObject(Type type, IcdXmlReader reader)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (reader == null)
				throw new ArgumentNullException("reader");

			IXmlConverter converter = XmlConverterAttribute.GetConverterForType(type);

			return converter.ReadXml(reader);
		}

		/// <summary>
		/// Deserializes the child elements as items in an array.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static IEnumerable<T> DeserializeArray<T>(IcdXmlReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			return DeserializeArray(typeof(T), reader).Cast<T>();
		}

		/// <summary>
		/// Deserializes the child elements as items in an array.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static IEnumerable<object> DeserializeArray(Type type, IcdXmlReader reader)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (reader == null)
				throw new ArgumentNullException("reader");

			if (reader.NodeType != XmlNodeType.Element)
				throw new FormatException("Expected start element for array");

			string arrayName = reader.Name;

			// Read into the first element
			do
			{
				reader.Read();
			} while (reader.NodeType != XmlNodeType.Element && reader.NodeType != XmlNodeType.EndElement);

			// Empty array
			if (reader.NodeType == XmlNodeType.EndElement)
				yield break;

			// Read the items
			IXmlConverter converter = XmlConverterAttribute.GetConverterForType(type);
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				yield return converter.ReadXml(reader);
				reader.SkipInsignificantWhitespace();
			}

			if (reader.NodeType != XmlNodeType.EndElement || reader.Name != arrayName)
				throw new FormatException("Expected end element for array");

			// Read out of the array end element
			reader.Read();
		}

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

		public static T FromString<T>(string value)
		{
			return (T)FromString(typeof(T), value);
		}

		public static object FromString(Type type, string value)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (type == typeof(bool))
				return ToBool(value);
			if (type == typeof(byte))
				return ToByte(value);
			if (type == typeof(decimal))
				return ToDecimal(value);
			if (type == typeof(char))
				return ToChar(value);
			if (type == typeof(double))
				return ToDouble(value);
			if (type == typeof(Guid))
				return ToGuid(value);
			if (type == typeof(float))
				return ToSingle(value);
			if (type == typeof(int))
				return ToInt32(value);
			if (type == typeof(long))
				return ToInt64(value);
			if (type == typeof(sbyte))
				return ToSByte(value);
			if (type == typeof(short))
				return ToInt16(value);
			if (type == typeof(TimeSpan))
				return ToTimeSpan(value);
			if (type == typeof(uint))
				return ToUInt32(value);
			if (type == typeof(ulong))
				return ToUInt64(value);
			if (type == typeof(ushort))
				return ToUInt16(value);

			return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
		}

		public static bool ToBool(string data)
		{
			return XmlConvert.ToBoolean(data);
		}

		public static byte ToByte(string data)
		{
			return XmlConvert.ToByte(data);
		}

		public static char ToChar(string data)
		{
			return XmlConvert.ToChar(data);
		}

		public static DateTime ToDateTime(string data, string format)
		{
			return XmlConvert.ToDateTime(data, format);
		}

		public static DateTime ToDateTime(string data, string[] formats)
		{
			return XmlConvert.ToDateTime(data, formats);
		}

		public static DateTime ToDateTime(string data, XmlDateTimeSerializationMode dateTimeOption)
		{
			return XmlConvert.ToDateTime(data, dateTimeOption);
		}

		public static decimal ToDecimal(string data)
		{
			return XmlConvert.ToDecimal(data);
		}

		public static double ToDouble(string data)
		{
			return XmlConvert.ToDouble(data);
		}

		public static Guid ToGuid(string data)
		{
			return XmlConvert.ToGuid(data);
		}

		public static short ToInt16(string data)
		{
			return XmlConvert.ToInt16(data);
		}

		public static int ToInt32(string data)
		{
			return XmlConvert.ToInt32(data);
		}

		public static long ToInt64(string data)
		{
			return XmlConvert.ToInt64(data);
		}

		public static sbyte ToSByte(string data)
		{
			return XmlConvert.ToSByte(data);
		}

		public static float ToSingle(string data)
		{
			return XmlConvert.ToSingle(data);
		}

		public static TimeSpan ToTimeSpan(string data)
		{
			return XmlConvert.ToTimeSpan(data);
		}

		public static ushort ToUInt16(string data)
		{
			return XmlConvert.ToUInt16(data);
		}

		public static uint ToUInt32(string data)
		{
			return XmlConvert.ToUInt32(data);
		}

		public static ulong ToUInt64(string data)
		{
			return XmlConvert.ToUInt64(data);
		}
	}
}
