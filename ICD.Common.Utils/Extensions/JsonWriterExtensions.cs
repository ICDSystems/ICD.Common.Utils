using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using Newtonsoft.Json;

namespace ICD.Common.Utils.Extensions
{
	public static class JsonWriterExtensions
	{
		/// <summary>
		/// Serializes the given sequence of items to the writer.
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <param name="extends"></param>
		/// <param name="writer"></param>
		/// <param name="items"></param>
		public static void SerializeArray<TItem>(this JsonSerializer extends, JsonWriter writer, IEnumerable<TItem> items)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (writer == null)
				throw new ArgumentNullException("writer");

			if (items == null)
				throw new ArgumentNullException("items");

			extends.SerializeArray(writer, items, (s, w, item) => s.Serialize(w, item));
		}

		/// <summary>
		/// Serializes the given sequence of items to the writer.
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <param name="extends"></param>
		/// <param name="writer"></param>
		/// <param name="items"></param>
		/// <param name="write"></param>
		public static void SerializeArray<TItem>(this JsonSerializer extends, JsonWriter writer, IEnumerable<TItem> items,
												 Action<JsonSerializer, JsonWriter, TItem> write)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (writer == null)
				throw new ArgumentNullException("writer");

			if (items == null)
				throw new ArgumentNullException("items");

			if (write == null)
				throw new ArgumentNullException("write");

			writer.WriteStartArray();
			{
				foreach (TItem item in items)
					write(extends, writer, item);
			}
			writer.WriteEndArray();
		}

		/// <summary>
		/// Writes the type value.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="type"></param>
		[PublicAPI]
		public static void WriteType(this JsonWriter extends, Type type)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (type == null)
			{
				extends.WriteNull();
				return;
			}

			string name = type.GetMinimalName();
			extends.WriteValue(name);
		}

		/// <summary>
		/// Writes the property name and value to the writer.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		public static void WriteProperty(this JsonWriter extends, string propertyName, object value)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			extends.WritePropertyName(propertyName);
			extends.WriteValue(value);
		}

		/// <summary>
		/// Writes the property name and value to the writer.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		public static void WriteProperty(this JsonWriter extends, string propertyName, string value)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			extends.WritePropertyName(propertyName);
			extends.WriteValue(value);
		}

		/// <summary>
		/// Writes the property name and value to the writer.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		public static void WriteProperty(this JsonWriter extends, string propertyName, DateTime value)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			extends.WritePropertyName(propertyName);
			extends.WriteValue(value);
		}

		/// <summary>
		/// Writes the property name and value to the writer.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		public static void WriteProperty(this JsonWriter extends, string propertyName, bool value)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			extends.WritePropertyName(propertyName);
			extends.WriteValue(value);
		}

		/// <summary>
		/// Writes the property name and value to the writer.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		public static void WriteProperty(this JsonWriter extends, string propertyName, int value)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			extends.WritePropertyName(propertyName);
			extends.WriteValue(value);
		}
	}
}
