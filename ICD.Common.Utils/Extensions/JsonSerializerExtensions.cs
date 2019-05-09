using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;

namespace ICD.Common.Utils.Extensions
{
	public static class JsonSerializerExtensions
	{
		/// <summary>
		/// Deserializes an array of items from the reader's current value.
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <param name="extends"></param>
		/// <param name="reader"></param>
		public static IEnumerable<TItem> DeserializeArray<TItem>(this JsonSerializer extends, JsonReader reader)
		{
			return extends.DeserializeArray(reader, (s, r) => extends.Deserialize<TItem>(reader));
		}

		/// <summary>
		/// Deserializes an array of items from the reader's current value.
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <param name="extends"></param>
		/// <param name="reader"></param>
		/// <param name="read"></param>
		public static IEnumerable<TItem> DeserializeArray<TItem>(this JsonSerializer extends, JsonReader reader,
																 Func<JsonSerializer, JsonReader, TItem> read)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (reader == null)
				throw new ArgumentNullException("reader");

			if (read == null)
				throw new ArgumentNullException("read");

			if (reader.TokenType == JsonToken.Null)
				return Enumerable.Empty<TItem>();

			if (reader.TokenType != JsonToken.StartArray)
				throw new FormatException(string.Format("Expected token {0} got {1}", JsonToken.StartArray, reader.TokenType));

			// ToArray to ensure everything gets read before moving onto the next token
			return DeserializeArrayIterator(extends, reader, read).ToArray();
		}

		/// <summary>
		/// Deserializes an array of items from the reader's current value.
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <param name="serializer"></param>
		/// <param name="reader"></param>
		/// <param name="read"></param>
		private static IEnumerable<TItem> DeserializeArrayIterator<TItem>(JsonSerializer serializer, JsonReader reader,
																		  Func<JsonSerializer, JsonReader, TItem> read)
		{
			// Step into the first value
			reader.Read();

			while (reader.TokenType != JsonToken.EndArray)
			{
				TItem output = read(serializer, reader);
				yield return output;

				// Step out of the value
				reader.Read();
			}
		}

		/// <summary>
		/// Deserializes a dictionary of items from the reader's current value.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="reader"></param>
		public static IEnumerable<KeyValuePair<TKey, TValue>> DeserializeDict<TKey, TValue>(this JsonSerializer extends,
		                                                                                    JsonReader reader)
		{
			return extends.DeserializeDict<TKey, TValue>(reader, p => (TKey)Convert.ChangeType(p, typeof(TKey), CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Deserializes a dictionary of items from the reader's current value.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="reader"></param>
		/// <param name="getKey"></param>
		public static IEnumerable<KeyValuePair<TKey, TValue>> DeserializeDict<TKey, TValue>(this JsonSerializer extends,
		                                                                                    JsonReader reader,
		                                                                                    Func<string, TKey> getKey)
		{
			return extends.DeserializeDict(reader, getKey, (s, r) => extends.Deserialize<TValue>(reader));
		}

		/// <summary>
		/// Deserializes a dictionary of items from the reader's current value.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="reader"></param>
		/// <param name="getKey"></param>
		/// <param name="readValue"></param>
		public static IEnumerable<KeyValuePair<TKey, TValue>> DeserializeDict<TKey, TValue>(this JsonSerializer extends,
		                                                                                    JsonReader reader,
		                                                                                    Func<string, TKey> getKey,
		                                                                                    Func<JsonSerializer, JsonReader,
			                                                                                    TValue> readValue)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (reader == null)
				throw new ArgumentNullException("reader");

			if (getKey == null)
				throw new ArgumentNullException("getKey");

			if (readValue == null)
				throw new ArgumentNullException("readValue");

			if (reader.TokenType == JsonToken.Null)
				return Enumerable.Empty<KeyValuePair<TKey, TValue>>();

			if (reader.TokenType != JsonToken.StartObject)
				throw new FormatException(string.Format("Expected token {0} got {1}", JsonToken.StartObject, reader.TokenType));

			// ToArray to ensure everything gets read before moving onto the next token
			return DeserializeDictIterator(extends, reader, getKey, readValue).ToArray();
		}

		/// <summary>
		/// Deserializes a dictionary of items from the reader's current value.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="serializer"></param>
		/// <param name="reader"></param>
		/// <param name="getKey"></param>
		/// <param name="readValue"></param>
		private static IEnumerable<KeyValuePair<TKey, TValue>> DeserializeDictIterator<TKey, TValue>(
			JsonSerializer serializer, JsonReader reader,
			Func<string, TKey> getKey,
			Func<JsonSerializer, JsonReader, TValue> readValue)
		{
			// Step into the first property
			reader.Read();

			while (reader.TokenType != JsonToken.EndObject)
			{
				if (reader.TokenType != JsonToken.PropertyName)
					throw new FormatException();

				TKey key = getKey((string)reader.Value);

				// Step into the value
				reader.Read();

				TValue value = readValue(serializer, reader);
				yield return new KeyValuePair<TKey, TValue>(key, value);

				// Step out of the value
				reader.Read();
			}
		}

		/// <summary>
		/// Serializes the given sequence of items to the writer.
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <param name="extends"></param>
		/// <param name="writer"></param>
		/// <param name="items"></param>
		public static void SerializeArray<TItem>(this JsonSerializer extends, JsonWriter writer, IEnumerable<TItem> items)
		{
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

			if (write == null)
				throw new ArgumentNullException("write");

			if (items == null)
			{
				writer.WriteNull();
				return;
			}

			writer.WriteStartArray();
			{
				foreach (TItem item in items)
					write(extends, writer, item);
			}
			writer.WriteEndArray();
		}

		/// <summary>
		/// Serializes the given sequence of items to the writer.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="writer"></param>
		/// <param name="items"></param>
		public static void SerializeDict<TKey, TValue>(this JsonSerializer extends, JsonWriter writer,
													   IEnumerable<KeyValuePair<TKey, TValue>> items)
		{
			extends.SerializeDict(writer, items, k => k.ToString());
		}

		/// <summary>
		/// Serializes the given sequence of items to the writer.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="writer"></param>
		/// <param name="items"></param>
		/// <param name="getPropertyName"></param>
		public static void SerializeDict<TKey, TValue>(this JsonSerializer extends, JsonWriter writer,
													   IEnumerable<KeyValuePair<TKey, TValue>> items,
													   Func<TKey, string> getPropertyName)
		{
			extends.SerializeDict(writer, items, getPropertyName, (s, w, v) => s.Serialize(w, v));
		}

		/// <summary>
		/// Serializes the given sequence of items to the writer.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="writer"></param>
		/// <param name="items"></param>
		/// <param name="getPropertyName"></param>
		/// <param name="writeValue"></param>
		public static void SerializeDict<TKey, TValue>(this JsonSerializer extends, JsonWriter writer,
													   IEnumerable<KeyValuePair<TKey, TValue>> items,
													   Func<TKey, string> getPropertyName,
													   Action<JsonSerializer, JsonWriter, TValue> writeValue)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (writer == null)
				throw new ArgumentNullException("writer");

			if (getPropertyName == null)
				throw new ArgumentNullException("getPropertyName");

			if (writeValue == null)
				throw new ArgumentNullException("writeValue");

			if (items == null)
			{
				writer.WriteNull();
				return;
			}

			writer.WriteStartObject();
			{
				foreach (KeyValuePair<TKey, TValue> kvp in items)
				{
					string propertyName = getPropertyName(kvp.Key);
					writer.WritePropertyName(propertyName);
					writeValue(extends, writer, kvp.Value);
				}
			}
			writer.WriteEndObject();
		}
	}
}