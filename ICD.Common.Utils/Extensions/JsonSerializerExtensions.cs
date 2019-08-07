using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ICD.Common.Utils.Extensions
{
	public static class JsonSerializerExtensions
	{
		private const string PROPERTY_KEY = "k";
		private const string PROPERTY_VALUE = "v";

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
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (reader == null)
				throw new ArgumentNullException("reader");

			return extends.DeserializeDict(reader,
			                               (s, r) => s.Deserialize<TKey>(r),
			                               (s, r) => s.Deserialize<TValue>(r));
		}

		/// <summary>
		/// Deserializes a dictionary of items from the reader's current value.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="reader"></param>
		/// <param name="readKey"></param>
		/// <param name="readValue"></param>
		public static IEnumerable<KeyValuePair<TKey, TValue>> DeserializeDict<TKey, TValue>(this JsonSerializer extends,
		                                                                                    JsonReader reader,
																							Func<JsonSerializer, JsonReader,
																								TKey> readKey,
																							Func<JsonSerializer, JsonReader,
			                                                                                    TValue> readValue)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (reader == null)
				throw new ArgumentNullException("reader");

			if (readKey == null)
				throw new ArgumentNullException("readKey");

			if (readValue == null)
				throw new ArgumentNullException("readValue");

			return extends.DeserializeArray(reader, (s, r) => s.DeserializeKeyValuePair(r, readKey, readValue));
		}

		public static KeyValuePair<TKey, TValue> DeserializeKeyValuePair<TKey, TValue>(this JsonSerializer extends, JsonReader reader,
		                                                                               Func<JsonSerializer, JsonReader,
			                                                                               TKey> readKey,
		                                                                               Func<JsonSerializer, JsonReader,
			                                                                               TValue> readValue)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (reader == null)
				throw new ArgumentNullException("reader");

			if (readKey == null)
				throw new ArgumentNullException("readKey");

			if (readValue == null)
				throw new ArgumentNullException("readValue");

			if (reader.TokenType != JsonToken.StartObject)
				throw new FormatException(string.Format("Expected token {0} got {1}", JsonToken.StartObject, reader.TokenType));

			TKey key = default(TKey);
			TValue value = default(TValue);

			// Step into the first property
			reader.Read();

			while (reader.TokenType != JsonToken.EndObject)
			{
				if (reader.TokenType != JsonToken.PropertyName)
					throw new FormatException(string.Format("Expected token {0} got {1}", JsonToken.PropertyName, reader.TokenType));

				string propertyName = (string)reader.Value;

				// Step into the value
				reader.Read();

				switch (propertyName)
				{
					case PROPERTY_KEY:
						key = readKey(extends, reader);
						break;

					case PROPERTY_VALUE:
						value = readValue(extends, reader);
						break;

					default:
						throw new FormatException(string.Format("Unexpected property {0}", reader.Value));
				}

				// Step out of the value
				reader.Read();
			}

			return new KeyValuePair<TKey, TValue>(key, value);
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
			extends.SerializeDict(writer, items,
			                      (s, w, k) => s.Serialize(w, k),
			                      (s, w, v) => s.Serialize(w, v));
		}

		/// <summary>
		/// Serializes the given sequence of items to the writer.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="writer"></param>
		/// <param name="items"></param>
		/// <param name="writeKey"></param>
		/// <param name="writeValue"></param>
		public static void SerializeDict<TKey, TValue>(this JsonSerializer extends, JsonWriter writer,
													   IEnumerable<KeyValuePair<TKey, TValue>> items,
		                                               Action<JsonSerializer, JsonWriter, TKey> writeKey,
													   Action<JsonSerializer, JsonWriter, TValue> writeValue)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (writer == null)
				throw new ArgumentNullException("writer");

			if (writeKey == null)
				throw new ArgumentNullException("writeKey");

			if (writeValue == null)
				throw new ArgumentNullException("writeValue");

			extends.SerializeArray(writer, items, (s, w, kvp) => s.SerializeKeyValuePair(w, kvp, writeKey, writeValue));
		}

		public static void SerializeKeyValuePair<TKey, TValue>(this JsonSerializer extends, JsonWriter writer,
		                                                       KeyValuePair<TKey, TValue> kvp,
		                                                       Action<JsonSerializer, JsonWriter, TKey> writeKey,
		                                                       Action<JsonSerializer, JsonWriter, TValue> writeValue)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (writer == null)
				throw new ArgumentNullException("writer");

			if (writeKey == null)
				throw new ArgumentNullException("writeKey");

			if (writeValue == null)
				throw new ArgumentNullException("writeValue");

			writer.WriteStartObject();
			{
				writer.WritePropertyName(PROPERTY_KEY);
				writeKey(extends, writer, kvp.Key);

				writer.WritePropertyName(PROPERTY_VALUE);
				writeValue(extends, writer, kvp.Value);
			}
			writer.WriteEndObject();
		}
	}
}