using System;
using ICD.Common.Properties;
using Newtonsoft.Json;

namespace ICD.Common.Utils.Json
{
	public abstract class AbstractGenericJsonConverter<T> : JsonConverter
	{
		/// <summary>
		/// Creates a new instance of T.
		/// </summary>
		/// <returns></returns>
		protected virtual T Instantiate()
		{
			return ReflectionUtils.CreateInstance<T>();
		}

		/// <summary>
		/// Writes the JSON representation of the object.
		/// </summary>
		/// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.</param>
		/// <param name="value">The value.</param>
		/// <param name="serializer">The calling serializer.</param>
		public sealed override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			if (serializer == null)
				throw new ArgumentNullException("serializer");

			if (value == null)
			{
				writer.WriteNull();
				return;
			}

			WriteJson(writer, (T)value, serializer);
		}

		/// <summary>
		/// Writes the JSON representation of the object.
		/// </summary>
		/// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.</param>
		/// <param name="value">The value.</param>
		/// <param name="serializer">The calling serializer.</param>
		[PublicAPI]
		public virtual void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			if (serializer == null)
				throw new ArgumentNullException("serializer");

			if (value == null)
			{
				writer.WriteNull();
				return;
			}

			writer.WriteStartObject();
			{
				WriteProperties(writer, value, serializer);
			}
			writer.WriteEndObject();
		}

		/// <summary>
		/// Override to write properties to the writer.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="value"></param>
		/// <param name="serializer"></param>
		protected virtual void WriteProperties(JsonWriter writer, T value, JsonSerializer serializer)
		{
		}

		/// <summary>
		/// Reads the JSON representation of the object.
		/// </summary>
		/// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from.</param>
		/// <param name="objectType">Type of the object.</param>
		/// <param name="existingValue">The existing value of object being read.</param>
		/// <param name="serializer">The calling serializer.</param>
		/// <returns>
		/// The object value.
		/// </returns>
		public sealed override object ReadJson(JsonReader reader, Type objectType, object existingValue,
		                                       JsonSerializer serializer)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			if (serializer == null)
				throw new ArgumentNullException("serializer");

			return ReadJson(reader, (T)existingValue, serializer);
		}

		/// <summary>
		/// Reads the JSON representation of the object.
		/// </summary>
		/// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from.</param>
		/// <param name="existingValue">The existing value of object being read.</param>
		/// <param name="serializer">The calling serializer.</param>
		/// <returns>
		/// The object value.
		/// </returns>
		[PublicAPI]
		public virtual T ReadJson(JsonReader reader, T existingValue, JsonSerializer serializer)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			if (serializer == null)
				throw new ArgumentNullException("serializer");

			T output = default(T);
			bool instantiated = false;

			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.Null || reader.TokenType == JsonToken.EndObject)
					break;

				if (!instantiated)
				{
					instantiated = true;
					output = Instantiate();
				}

				// Get the property
				if (reader.TokenType != JsonToken.PropertyName)
					continue;
				string property = (string)reader.Value;

				// Read into the value
				reader.Read();

				ReadProperty(property, reader, output, serializer);
			}

			return output;
		}

		/// <summary>
		/// Override to handle the current property value with the given name.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="reader"></param>
		/// <param name="instance"></param>
		/// <param name="serializer"></param>
		protected virtual void ReadProperty(string property, JsonReader reader, T instance, JsonSerializer serializer)
		{
			reader.Skip();
		}

		/// <summary>
		/// Determines whether this instance can convert the specified object type.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <returns>
		/// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(T);
		}
	}
}
