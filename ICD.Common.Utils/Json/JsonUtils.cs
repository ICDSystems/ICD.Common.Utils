using System;
using System.Linq;
using System.Text;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.IO;
using Newtonsoft.Json;

namespace ICD.Common.Utils.Json
{
	/// <summary>
	/// Utility methods for working with JSON.
	/// </summary>
	[PublicAPI]
	public static class JsonUtils
	{
		private const string MESSAGE_NAME_PROPERTY = "m";
		private const string MESSAGE_DATA_PROPERTY = "d";

		private static JsonSerializerSettings s_CommonSettings;

		/// <summary>
		/// Gets the common JSON serializer settings for cross-platform support.
		/// </summary>
		public static JsonSerializerSettings CommonSettings
		{
			get
			{
				if (s_CommonSettings != null)
					return s_CommonSettings;

				s_CommonSettings = new JsonSerializerSettings
				{
#if !SIMPLSHARP
					// Turn off the ridiculous new behaviour of DateTiming anything vaguely resembling a date
					DateParseHandling = DateParseHandling.None,
#endif
				};

				// Serialize DateTimes to ISO
				s_CommonSettings.Converters.Add(new DateTimeIsoConverter());

				// Minify Type serialization
				s_CommonSettings.Converters.Add(new MinimalTypeConverter());

				return s_CommonSettings;
			}
		}

		/// <summary>
		/// Serializes the given item and formats the JSON into a human-readable form.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string Format(object value)
		{
			string serial = JsonConvert.SerializeObject(value, Formatting.None, CommonSettings);
			return Format(serial);
		}

		/// <summary>
		/// Formats the JSON into a human-readable form.
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string Format(string json)
		{
			if (json == null)
				throw new ArgumentNullException("json");

			int indent = 0;
			bool quoted = false;
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < json.Length; i++)
			{
				char ch = json[i];
				switch (ch)
				{
					case '{':
					case '[':
						sb.Append(ch);
						if (!quoted)
						{
							sb.Append(IcdEnvironment.NewLine);
							Enumerable.Range(0, ++indent).ForEach(item => sb.Append('\t'));
						}
						break;
					case '}':
					case ']':
						if (!quoted)
						{
							sb.Append(IcdEnvironment.NewLine);
							Enumerable.Range(0, --indent).ForEach(item => sb.Append('\t'));
						}
						sb.Append(ch);
						break;
					case '"':
						sb.Append(ch);
						bool escaped = false;
						int index = i;
						while (index > 0 && json[--index] == '\\')
							escaped = !escaped;
						if (!escaped)
							quoted = !quoted;
						break;
					case ',':
						sb.Append(ch);
						if (!quoted)
						{
							sb.Append(IcdEnvironment.NewLine);
							Enumerable.Range(0, indent).ForEach(item => sb.Append('\t'));
						}
						break;
					case ':':
						sb.Append(ch);
						if (!quoted)
							sb.Append(" ");
						break;
					default:
						sb.Append(ch);
						break;
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Shorthand for serializing an instance to a json string.
		/// </summary>
		/// <param name="serializeMethod"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string Serialize(Action<JsonWriter> serializeMethod)
		{
			if (serializeMethod == null)
				throw new ArgumentNullException("serializeMethod");

			StringBuilder builder = new StringBuilder();

			using (IcdStringWriter stringWriter = new IcdStringWriter(builder))
			{
				using (JsonTextWriter writer = new JsonTextWriter(stringWriter.WrappedStringWriter))
					serializeMethod(writer);
			}

			return builder.ToString();
		}

		/// <summary>
		/// Shorthand for deserializing a json string to the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="deserializeMethod"></param>
		/// <param name="json"></param>
		/// <returns></returns>
		[PublicAPI]
		public static T Deserialize<T>(Func<JsonReader, T> deserializeMethod, string json)
		{
			if (deserializeMethod == null)
				throw new ArgumentNullException("deserializeMethod");

			using (IcdStringReader stringReader = new IcdStringReader(json))
			{
				using (JsonTextReader reader = new JsonTextReader(stringReader.WrappedStringReader))
					return deserializeMethod(reader);
			}
		}

		/// <summary>
		/// Serializes to json, wrapping the object with a message property to differentiate between messages.
		/// E.g.
		/// { a = 1 }
		/// Becomes
		/// { m = "Test", d = { a = 1 } }
		/// </summary>
		/// <param name="value"></param>
		/// <param name="messageName"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string SerializeMessage(object value, string messageName)
		{
			return SerializeMessage(w => new JsonSerializer().Serialize(w, value), messageName);
		}

		/// <summary>
		/// Serializes to json, wrapping the object with a message property to differentiate between messages.
		/// E.g.
		/// { a = 1 }
		/// Becomes
		/// { m = "Test", d = { a = 1 } }
		/// </summary>
		/// <param name="serializeMethod"></param>
		/// <param name="messageName"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string SerializeMessage(Action<JsonWriter> serializeMethod, string messageName)
		{
			if (serializeMethod == null)
				throw new ArgumentNullException("serializeMethod");

			return Serialize(w =>
			                 {
				                 w.WriteStartObject();
				                 {
					                 w.WritePropertyName(MESSAGE_NAME_PROPERTY);
					                 w.WriteValue(messageName);

					                 w.WritePropertyName(MESSAGE_DATA_PROPERTY);
					                 serializeMethod(w);
				                 }
				                 w.WriteEndObject();
			                 });
		}

		/// <summary>
		/// Deserializes a json object wrapped in a json message structure.
		/// E.g.
		/// { a = 1 }
		/// Becomes
		/// { m = "Test", d = { a = 1 } }
		/// </summary>
		/// <param name="deserializeMethod"></param>
		/// <param name="json"></param>
		/// <returns></returns>
		[PublicAPI]
		public static T DeserializeMessage<T>(Func<JsonReader, string, T> deserializeMethod, string json)
		{
			if (deserializeMethod == null)
				throw new ArgumentNullException("deserializeMethod");

			return Deserialize(r =>
			                   {
				                   T output = default(T);
				                   string messageName = null;

				                   while (r.Read())
				                   {
					                   if (r.TokenType == JsonToken.EndObject)
					                   {
						                   r.Read();
						                   break;
					                   }

					                   if (r.TokenType != JsonToken.PropertyName)
						                   continue;

					                   string property = r.Value as string;

					                   // Read to the value
					                   r.Read();

					                   switch (property)
					                   {
						                   case MESSAGE_NAME_PROPERTY:
							                   messageName = r.GetValueAsString();
							                   break;

						                   case MESSAGE_DATA_PROPERTY:
							                   output = deserializeMethod(r, messageName);
							                   break;
					                   }
				                   }

				                   return output;
			                   },
			                   json);
		}
	}
}
