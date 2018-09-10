using System;
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
			if (value == null)
				return ToString(null);

			IXmlConverter converter = XmlConverterAttribute.GetConverterForInstance(value);

			StringBuilder builder = new StringBuilder();

			using (IcdStringWriter stringWriter = new IcdStringWriter(builder))
			{
				using (IcdXmlTextWriter writer = new IcdXmlTextWriter(stringWriter))
					converter.WriteXml(writer, elementName, value);
			}

			return builder.ToString();
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
		private static object DeserializeObject(Type type, string xml)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			using (IcdXmlReader reader = new IcdXmlReader(xml))
				return DeserializeObject(type, reader);
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
