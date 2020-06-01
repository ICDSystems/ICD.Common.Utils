using System;
using System.Text.RegularExpressions;
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
		public static T ReadAsObject<T>([NotNull] this JsonReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			JsonSerializer serializer =
#if SIMPLSHARP
				new JsonSerializer();
#else
				JsonSerializer.CreateDefault();
#endif
			return extends.ReadAsObject<T>(serializer);
		}

		/// <summary>
		/// Reads the current token in the reader and deserializes to the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="serializer"></param>
		/// <returns></returns>
		public static T ReadAsObject<T>([NotNull] this JsonReader extends, [NotNull] JsonSerializer serializer)
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
		public static void ReadObject([NotNull] this JsonReader extends, [NotNull] JsonSerializer serializer,
		                              [NotNull] Action<string, JsonReader, JsonSerializer> readPropertyValue)
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
				throw new FormatException(string.Format("Expected {0} got {1}", JsonToken.StartObject,
				                                        extends.TokenType));

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
		[CanBeNull]
		public static Type GetValueAsType([NotNull] this JsonReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (extends.TokenType == JsonToken.Null)
				return null;

			string value = extends.GetValueAsString();
			return Type.GetType(value);
		}

		/// <summary>
		/// Gets the current value as an unsigned integer.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static uint GetValueAsUInt([NotNull] this JsonReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (extends.TokenType == JsonToken.Integer)
				return (uint)(long)extends.Value;

			string message = string.Format("Token {0} {1} is not {2}", extends.TokenType, extends.Value,
			                               JsonToken.Integer);
			throw new InvalidCastException(message);
		}

		/// <summary>
		/// Gets the current value as an integer.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static int GetValueAsInt([NotNull] this JsonReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (extends.TokenType == JsonToken.Integer)
				return (int)(long)extends.Value;

			string message = string.Format("Token {0} {1} is not {2}", extends.TokenType, extends.Value,
			                               JsonToken.Integer);
			throw new InvalidCastException(message);
		}
		
		/// <summary>
		/// Gets the current value as a long.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static long GetValueAsLong([NotNull] this JsonReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (extends.TokenType == JsonToken.Integer)
				return (long)extends.Value;

			string message = string.Format("Token {0} {1} is not {2}", extends.TokenType, extends.Value,
										   JsonToken.Integer);
			throw new InvalidCastException(message);
		}

		/// <summary>
		/// Gets the current value as an unsigned long.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static ulong GetValueAsULong([NotNull] this JsonReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (extends.TokenType == JsonToken.Integer)
				return (ulong)extends.Value;

			string message = string.Format("Token {0} {1} is not {2}", extends.TokenType, extends.Value,
										   JsonToken.Integer);
			throw new InvalidCastException(message);
		}

		/// <summary>
		/// Gets the current value as a string.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string GetValueAsString([NotNull] this JsonReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

#if !SIMPLSHARP
			// Newer versions of NewtonSoft try to be helpful and interpret strings as DateTimes without any consideration for different DateTime formats.
			if (extends.TokenType == JsonToken.Date && extends.DateParseHandling != DateParseHandling.None)
				throw new InvalidOperationException("DateParseHandling needs to be set to None");
#endif

			if (!extends.TokenType.IsPrimitive())
				throw new FormatException("Expected primitive token type but got " + extends.TokenType);

			return extends.Value == null ? null : extends.Value.ToString();
		}

		/// <summary>
		/// Gets the current value as a bool.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool GetValueAsBool([NotNull] this JsonReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (extends.TokenType == JsonToken.Boolean)
				return (bool)extends.Value;

			string message = string.Format("Token {0} {1} is not {2}", extends.TokenType, extends.Value,
			                               JsonToken.Boolean);
			throw new InvalidCastException(message);
		}

		/// <summary>
		/// Gets the current value as an enum.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static T GetValueAsEnum<T>([NotNull] this JsonReader extends)
			where T : struct, IConvertible
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (extends.TokenType == JsonToken.String)
				return EnumUtils.Parse<T>(extends.GetValueAsString(), true);

			return (T)(object)extends.GetValueAsInt();
		}

		/// <summary>
		/// Gets the current value as a guid.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static Guid GetValueAsGuid([NotNull] this JsonReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			string stringValue = extends.GetValueAsString();
			return new Guid(stringValue);
		}

		/// <summary>
		/// Gets the current value as a date.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static DateTime GetValueAsDateTime([NotNull] this JsonReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

#if !SIMPLSHARP
			// Newer NewtonSoft tries to be helpful by assuming that anything that looks like a DateTime must be a date.
			if (extends.DateParseHandling != DateParseHandling.None)
				return (DateTime)extends.Value;
#endif
			/*
			"\"\\/Date(1335205592410)\\/\""         .NET JavaScriptSerializer
			"\"\\/Date(1335205592410-0500)\\/\""    .NET DataContractJsonSerializer
			"2012-04-23T18:25:43.511Z"              JavaScript built-in JSON object
			"2012-04-21T18:25:43-05:00"             ISO 8601
			 */

			string serial = extends.GetValueAsString();

			Match match;
			if (RegexUtils.Matches(serial, @"Date\((?'date'\d+)(?'zone'(-|\+)\d+)?\)", out match))
			{
				long ms = long.Parse(match.Groups["date"].Value);
				DateTime dateTime = DateTimeUtils.FromEpochMilliseconds(ms);
				if (!match.Groups["zone"].Success)
					return dateTime;

				// No TimeZoneInfo in CF, so now things get gross
				dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
				serial = dateTime.ToIso() + match.Groups["zone"].Value;
			}

			return DateTimeUtils.FromIso8601(serial);
		}
	}
}
