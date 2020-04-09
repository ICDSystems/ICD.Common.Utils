using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using Newtonsoft.Json;

namespace ICD.Common.Utils.Json
{
	/// <summary>
	/// Simple wrapper for serialization of an object and its type.
	/// </summary>
	[JsonConverter(typeof(JsonItemWrapperConverter))]
	public sealed class JsonItemWrapper
	{
		/// <summary>
		/// Gets the Type of the item. Returns null if the item is null.
		/// </summary>
		[CanBeNull]
		public Type Type { get; set; }

		/// <summary>
		/// Gets the wrapped item.
		/// </summary>
		[CanBeNull]
		public object Item { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		public JsonItemWrapper()
			: this(null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="item"></param>
		public JsonItemWrapper([CanBeNull] object item)
		{
			Item = item;
			Type = item == null ? null : item.GetType();
		}
	}

	public sealed class JsonItemWrapperConverter : AbstractGenericJsonConverter<JsonItemWrapper>
	{
		private const string TYPE_TOKEN = "t";
		private const string ITEM_TOKEN = "i";

		/// <summary>
		/// Override to write properties to the writer.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="value"></param>
		/// <param name="serializer"></param>
		protected override void WriteProperties(JsonWriter writer, JsonItemWrapper value, JsonSerializer serializer)
		{
			base.WriteProperties(writer, value, serializer);

			if (value.Type != null)
				writer.WriteProperty(TYPE_TOKEN, value.Type.GetMinimalName());

			if (value.Item != null)
			{
				writer.WritePropertyName(ITEM_TOKEN);
				serializer.Serialize(writer, value.Item);
			}
		}

		/// <summary>
		/// Override to handle the current property value with the given name.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="reader"></param>
		/// <param name="instance"></param>
		/// <param name="serializer"></param>
		protected override void ReadProperty(string property, JsonReader reader, JsonItemWrapper instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case TYPE_TOKEN:
					instance.Type = reader.TokenType == JsonToken.Null ? null : reader.GetValueAsType();
					break;

				case ITEM_TOKEN:
					if (instance.Type == null && reader.TokenType != JsonToken.Null)
						throw new FormatException("No Type for associated Item");
					instance.Item = serializer.Deserialize(reader, instance.Type);
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
