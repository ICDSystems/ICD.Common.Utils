using System;
using System.Text.RegularExpressions;
using ICD.Common.Utils.Extensions;
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

		private readonly object m_Item;

		/// <summary>
		/// Gets the Type of the item. Returns null if the item is null.
		/// </summary>
		public Type ItemType { get { return m_Item == null ? null : m_Item.GetType(); } }

		/// <summary>
		/// Gets the wrapped item.
		/// </summary>
		public object Item { get { return m_Item; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="item"></param>
		public JsonItemWrapper(object item)
		{
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
			{
				writer.WritePropertyName(TYPE_TOKEN);
				writer.WriteType(ItemType);

				writer.WritePropertyName(ITEM_TOKEN);
				writer.WriteValue(JsonConvert.SerializeObject(m_Item));
			}
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

			if (type == null)
			{
				typeString = typeString.Replace("_SimplSharp", "").Replace("_NetStandard", "");
				type = Type.GetType(typeString);
			}

			if (type == null)
			{
				typeString = AddSimplSharpSuffix(typeString);
				type = Type.GetType(typeString);
			}

			return JsonConvert.DeserializeObject(itemString, type);
		}

		private static string AddSimplSharpSuffix(string typeString)
		{
			return Regex.Replace(typeString,
				"(?'prefix'[^,]+, )(?'assembly'[^,]*)(?'suffix', .*)",
				"${prefix}${assembly}_SimplSharp${suffix}");
		}
	}
}
