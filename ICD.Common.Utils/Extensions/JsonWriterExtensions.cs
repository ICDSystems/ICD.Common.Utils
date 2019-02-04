using System;
using System.Collections.Generic;
using System.Text;
using ICD.Common.Properties;
using Newtonsoft.Json;

namespace ICD.Common.Utils.Extensions
{
	public static class JsonWriterExtensions
	{
		private static readonly Dictionary<Type, string> s_TypeToName;

		/// <summary>
		/// Static constructor.
		/// </summary>
		static JsonWriterExtensions()
		{
			s_TypeToName = new Dictionary<Type, string>();
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

			string name = GetTypeName(type);
			extends.WriteValue(name);
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
		/// Gets the string representation foe the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static string GetTypeName(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			string name;
			if (!s_TypeToName.TryGetValue(type, out name))
			{
				name = RemoveAssemblyDetails(type.AssemblyQualifiedName);
				s_TypeToName.Add(type, name);
			}

			return name;
		}

		/// <summary>
		/// Taken from Newtonsoft.Json.Utilities.ReflectionUtils
		/// Removes the assembly details from a type assembly qualified name.
		/// </summary>
		/// <param name="fullyQualifiedTypeName"></param>
		/// <returns></returns>
		private static string RemoveAssemblyDetails(string fullyQualifiedTypeName)
		{
			StringBuilder builder = new StringBuilder();

			// loop through the type name and filter out qualified assembly details from nested type names
			bool writingAssemblyName = false;
			bool skippingAssemblyDetails = false;
			for (int i = 0; i < fullyQualifiedTypeName.Length; i++)
			{
				char current = fullyQualifiedTypeName[i];
				switch (current)
				{
					case '[':
						writingAssemblyName = false;
						skippingAssemblyDetails = false;
						builder.Append(current);
						break;
					case ']':
						writingAssemblyName = false;
						skippingAssemblyDetails = false;
						builder.Append(current);
						break;
					case ',':
						if (!writingAssemblyName)
						{
							writingAssemblyName = true;
							builder.Append(current);
						}
						else
						{
							skippingAssemblyDetails = true;
						}
						break;
					default:
						if (!skippingAssemblyDetails)
						{
							builder.Append(current);
						}
						break;
				}
			}

			return builder.ToString();
		}
	}
}
