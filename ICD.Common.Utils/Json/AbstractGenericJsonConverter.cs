#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

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

// ReSharper disable CompareNonConstrainedGenericWithNull
			if (value == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
			{
				writer.WriteNull();
				return;
			}

			WriteObject(writer, value, serializer);
		}

		/// <summary>
		/// Override to write the object value to the writer.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="value"></param>
		/// <param name="serializer"></param>
		protected virtual void WriteObject(JsonWriter writer, T value, JsonSerializer serializer)
		{
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

			// Casting null blows up struct casts
			T cast = (T)(existingValue ?? default(T));

			return ReadJson(reader, cast, serializer);
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

			if (reader.TokenType == JsonToken.Null)
				return existingValue;

			if (reader.TokenType != JsonToken.StartObject)
				throw new FormatException(string.Format("Expected {0} got {1}", JsonToken.StartObject, reader.TokenType));

			return ReadObject(reader, existingValue, serializer);
		}

		/// <summary>
		/// Override to handle deserialization of the current StartObject token.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="existingValue"></param>
		/// <param name="serializer"></param>
		/// <returns></returns>
		protected virtual T ReadObject(JsonReader reader, T existingValue, JsonSerializer serializer)
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
// ReSharper disable ConvertConditionalTernaryToNullCoalescing
			T output = existingValue == null ? Instantiate() : existingValue;
// ReSharper restore ConvertConditionalTernaryToNullCoalescing
// ReSharper restore CompareNonConstrainedGenericWithNull

			reader.ReadObject(serializer, (p, r, s) => ReadProperty(p, r, output, s));

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
