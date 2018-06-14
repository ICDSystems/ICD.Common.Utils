using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICD.Common.Utils.Extensions
{
	/// <summary>
	/// Extension methods for working with JSON.
	/// </summary>
	public static class JsonExtensions
	{
		/// <summary>
		/// Writes the object value.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="value"></param>
		/// <param name="serializer"></param>
		/// <param name="converter"></param>
		[PublicAPI]
		public static void WriteObject(this JsonWriter extends, object value, JsonSerializer serializer,
		                               JsonConverter converter)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (serializer == null)
				throw new ArgumentNullException("serializer");

			if (converter == null)
				throw new ArgumentNullException("converter");

			JObject jObject = JObject.FromObject(value, serializer);
			jObject.WriteTo(extends, converter);
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

			// Find the smallest possible name representation for the type that will still resolve
			string name = Type.GetType(type.FullName) == null
				              ? type.AssemblyQualifiedName
				              : type.FullName;

			extends.WriteValue(name);
		}

		/// <summary>
		/// Gets the current value as a Type.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static Type GetValueAsType(this JsonReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			string value = extends.GetValueAsString();
			return Type.GetType(value);
		}

		/// <summary>
		/// Gets the current value as an unsigned integer.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static uint GetValueAsUInt(this JsonReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (extends.TokenType == JsonToken.Integer)
				return (uint)(long)extends.Value;

			string message = string.Format("Token {0} {1} is not {2}", extends.TokenType, extends.Value, JsonToken.Integer);
			throw new InvalidCastException(message);
		}

		/// <summary>
		/// Gets the current value as an integer.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static int GetValueAsInt(this JsonReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (extends.TokenType == JsonToken.Integer)
				return (int)(long)extends.Value;

			string message = string.Format("Token {0} {1} is not {2}", extends.TokenType, extends.Value, JsonToken.Integer);
			throw new InvalidCastException(message);
		}

		/// <summary>
		/// Gets the current value as a string.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string GetValueAsString(this JsonReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (extends.TokenType == JsonToken.String || extends.TokenType == JsonToken.Null)
				return extends.Value as string;

			string message = string.Format("Token {0} {1} is not {2}", extends.TokenType, extends.Value, JsonToken.String);
			throw new InvalidCastException(message);
		}

		/// <summary>
		/// Gets the current value as a bool.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool GetValueAsBool(this JsonReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (extends.TokenType == JsonToken.Boolean)
				return (bool)extends.Value;

			string message = string.Format("Token {0} {1} is not {2}", extends.TokenType, extends.Value, JsonToken.Boolean);
			throw new InvalidCastException(message);
		}

		/// <summary>
		/// Gets the current value as an enum.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static T GetValueAsEnum<T>(this JsonReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (extends.TokenType == JsonToken.String)
				return EnumUtils.Parse<T>(extends.GetValueAsString(), true);

			return (T)(object)extends.GetValueAsInt();
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
		/// Deserializes an array of items from the reader's current value.
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <param name="extends"></param>
		/// <param name="reader"></param>
		public static IEnumerable<TItem> DeserializeArray<TItem>(this JsonSerializer extends, JsonReader reader)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (reader == null)
				throw new ArgumentNullException("reader");

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

			return DeserializeArrayIterator(extends, reader, read);
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

				// Read out of the last value
				reader.Read();
			}
		}

		/// <summary>
		/// Serializes the given sequence of key-value-pairs to the writer.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="writer"></param>
		/// <param name="items"></param>
		public static void SerializeDictionary<TKey, TValue>(this JsonSerializer extends, JsonWriter writer,
		                                                     IEnumerable<KeyValuePair<TKey, TValue>> items)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (writer == null)
				throw new ArgumentNullException("writer");

			if (items == null)
				throw new ArgumentNullException("items");

			writer.WriteStartObject();
			{
				foreach (KeyValuePair<TKey, TValue> kvp in items)
				{
					writer.WritePropertyName(kvp.Key.ToString());
					extends.Serialize(writer, kvp.Value);
				}
			}
			writer.WriteEndObject();
		}

		/// <summary>
		/// Deserializes a dictionary of items from the reader's current value.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="reader"></param>
		public static IEnumerable<KeyValuePair<TKey, TValue>> DeserializeDictionary<TKey, TValue>(this JsonSerializer extends,
		                                                                                          JsonReader reader)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (reader == null)
				throw new ArgumentNullException("reader");

			if (reader.TokenType == JsonToken.Null)
				return Enumerable.Empty<KeyValuePair<TKey, TValue>>();

			if (reader.TokenType != JsonToken.StartObject)
				throw new FormatException(string.Format("Expected token {0} got {1}", JsonToken.StartObject, reader.TokenType));

			return DeserializeDictionaryIterator<TKey, TValue>(extends, reader);
		}

		/// <summary>
		/// Deserializes a dictionary of items from the reader's current value.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="serializer"></param>
		/// <param name="reader"></param>
		private static IEnumerable<KeyValuePair<TKey, TValue>> DeserializeDictionaryIterator<TKey, TValue>(
			JsonSerializer serializer, JsonReader reader)
		{
			// Step into the first key
			reader.Read();

			while (reader.TokenType != JsonToken.EndObject)
			{
				TKey key = (TKey)Convert.ChangeType(reader.Value, typeof(TKey), null);

				// Step into the value
				reader.Read();

				TValue value = serializer.Deserialize<TValue>(reader);

				yield return new KeyValuePair<TKey, TValue>(key, value);

				// Read out of the last value
				reader.Read();
			}
		}
	}
}
