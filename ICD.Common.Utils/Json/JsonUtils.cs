using System;
using System.Globalization;
using System.Linq;
using System.Text;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICD.Common.Utils.Json
{
	/// <summary>
	/// Utility methods for working with JSON.
	/// </summary>
	[PublicAPI]
	public static class JsonUtils
	{
		// 2016-02-26T19:24:59
		private const string DATE_FORMAT = @"yyyy-MM-dd\THH:mm:ss";

		private const string MESSAGE_NAME_PROPERTY = "m";
		private const string MESSAGE_DATA_PROPERTY = "d";

		/// <summary>
		/// Forces Newtonsoft to cache the given type for faster subsequent usage.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public static void CacheType<T>()
			where T : new()
		{
			string serialized = JsonConvert.SerializeObject(ReflectionUtils.CreateInstance<T>());
			JsonConvert.DeserializeObject<T>(serialized);
		}

		/// <summary>
		/// Gets the token as a DateTime value.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		[PublicAPI]
		public static DateTime ParseDateTime(JToken token)
		{
			if (token == null)
				throw new ArgumentNullException("token");

#if SIMPLSHARP
			return DateTime.ParseExact((string)token, DATE_FORMAT, CultureInfo.CurrentCulture);
#else
			return (DateTime)token;
#endif
		}

		/// <summary>
		/// Gets the token as a DateTime value.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool TryParseDateTime(JToken token, out DateTime output)
		{
			if (token == null)
				throw new ArgumentNullException("token");

			output = default(DateTime);

			try
			{
				output = ParseDateTime(token);
				return true;
			}
			catch (InvalidCastException)
			{
				return false;
			}
		}

		/// <summary>
		/// Pretty-prints the JSON document.
		/// </summary>
		/// <param name="json"></param>
		[PublicAPI]
		public static void Print(string json)
		{
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

			IcdConsole.PrintLine(sb.ToString());
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

			using (JsonTextWriter writer = new JsonTextWriter(new IcdStringWriter(builder).WrappedStringWriter))
				serializeMethod(writer);

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

			using (JsonTextReader reader = new JsonTextReader(new IcdStringReader(json).WrappedStringReader))
				return deserializeMethod(reader);
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
	}
}
