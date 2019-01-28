using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using Newtonsoft.Json;

namespace ICD.Common.Utils.Extensions
{
	/// <summary>
	/// Extension methods for working with JSON.
	/// </summary>
	public static class JsonReaderExtensions
	{
		/// <summary>
		/// Reads the current token in the reader and deserializes to the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static T ReadAsObject<T>(this JsonReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			JsonSerializer serializer = new JsonSerializer();
			return extends.ReadAsObject<T>(serializer);
		}

		/// <summary>
		/// Reads the current token in the reader and deserializes to the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="serializer"></param>
		/// <returns></returns>
		public static T ReadAsObject<T>(this JsonReader extends, JsonSerializer serializer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (serializer == null)
				throw new ArgumentNullException("serializer");

			return serializer.Deserialize<T>(extends);
		}

		/// <summary>
		/// Reads through the current object token and calls the callback for each property value.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="serializer"></param>
		/// <param name="readPropertyValue"></param>
		public static void ReadObject(this JsonReader extends, JsonSerializer serializer,
		                              Action<string, JsonReader, JsonSerializer> readPropertyValue)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (serializer == null)
				throw new ArgumentNullException("serializer");

			if (readPropertyValue == null)
				throw new ArgumentNullException("readPropertyValue");

			if (extends.TokenType == JsonToken.Null)
				return;

			if (extends.TokenType != JsonToken.StartObject)
				throw new FormatException(string.Format("Expected {0} got {1}", JsonToken.StartObject, extends.TokenType));

			while (extends.Read())
			{
				if (extends.TokenType == JsonToken.EndObject)
					break;

				// Get the property
				if (extends.TokenType != JsonToken.PropertyName)
					continue;
				string property = (string)extends.Value;

				// Read into the value
				extends.Read();

				readPropertyValue(property, extends, serializer);
			}
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
			where T : struct, IConvertible
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (extends.TokenType == JsonToken.String)
				return EnumUtils.Parse<T>(extends.GetValueAsString(), true);

			return (T)(object)extends.GetValueAsInt();
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
	}
}
