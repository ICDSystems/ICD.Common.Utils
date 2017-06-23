﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICD.Common.Utils.Json
{
	/// <summary>
	/// Simple wrapper for serialization of an object and its type.
	/// </summary>
	public sealed class JsonItemWrapper
	{
		private const string TYPE_TOKEN = "t";
		private const string ITEM_TOKEN = "i";

		private readonly string m_ItemTypeString;
		private readonly object m_Item;

		/// <summary>
		/// Gets the string representation of the item type. Returns null if the item is null.
		/// </summary>
		public string ItemTypeString { get { return m_ItemTypeString; } }

		/// <summary>
		/// Gets the Type of the item. Returns null if the item is null.
		/// </summary>
		public Type ItemType
		{
			get { return string.IsNullOrEmpty(m_ItemTypeString) ? null : Type.GetType(m_ItemTypeString); }
		}

		/// <summary>
		/// Gets the wrapped item.
		/// </summary>
		public object Item { get { return string.IsNullOrEmpty(m_ItemTypeString) ? null : m_Item; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="item"></param>
		public JsonItemWrapper(object item)
		{
			m_ItemTypeString = item == null ? null : item.GetType().FullName;
			m_Item = item;
		}

		/// <summary>
		/// Writes the JsonItemWrapper as a JObject.
		/// </summary>
		/// <param name="writer"></param>
		public void Write(JsonWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			writer.WriteStartObject();

			writer.WritePropertyName(TYPE_TOKEN);
			writer.WriteValue(m_ItemTypeString);

			writer.WritePropertyName(ITEM_TOKEN);
			writer.WriteValue(JsonConvert.SerializeObject(m_Item));

			writer.WriteEndObject();
		}

		/// <summary>
		/// Reads the JToken back to the wrapped object.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public static object ReadToObject(JToken token)
		{
			if (token == null)
				throw new ArgumentNullException("token");

			string typeString = (string)token.SelectToken(TYPE_TOKEN);
			if (string.IsNullOrEmpty(typeString))
				return null;

			string itemString = (string)token.SelectToken(ITEM_TOKEN);
			Type type = Type.GetType(typeString);

			return JsonConvert.DeserializeObject(itemString, type);
		}
	}
}
