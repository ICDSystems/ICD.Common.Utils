using System;
using ICD.Common.Properties;
using Newtonsoft.Json;

namespace ICD.Common.Utils.Extensions
{
	public static class JsonWriterExtensions
	{
		/// <summary>
		/// Writes the DateTime as an ISO-8601 string.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="dateTime"></param>
		public static void WriteDateTime([NotNull] this JsonWriter extends, DateTime dateTime)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			string iso = dateTime.ToIso();

			// Remove redundant ms
			iso = iso.Replace(".0000000", "");

			extends.WriteValue(iso);
		}

		/// <summary>
		/// Writes the type value.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="type"></param>
		[PublicAPI]
		public static void WriteType([NotNull] this JsonWriter extends, [CanBeNull] Type type)
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
		public static void WriteProperty([NotNull]this JsonWriter extends, string propertyName, object value)
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
		public static void WriteProperty([NotNull]this JsonWriter extends, string propertyName, string value)
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
		public static void WriteProperty([NotNull]this JsonWriter extends, string propertyName, DateTime value)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			extends.WritePropertyName(propertyName);
			extends.WriteDateTime(value);
		}

		/// <summary>
		/// Writes the property name and value to the writer.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		public static void WriteProperty([NotNull]this JsonWriter extends, string propertyName, bool value)
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
		public static void WriteProperty([NotNull]this JsonWriter extends, string propertyName, int value)
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
		public static void WriteProperty([NotNull]this JsonWriter extends, string propertyName, Guid value)
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
		public static void WriteProperty([NotNull]this JsonWriter extends, string propertyName, Type value)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			extends.WritePropertyName(propertyName);
			extends.WriteType(value);
		}
	}
}
