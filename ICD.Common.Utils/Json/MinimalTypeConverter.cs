using System;
using ICD.Common.Utils.Extensions;
using Newtonsoft.Json;

namespace ICD.Common.Utils.Json
{
	public sealed class MinimalTypeConverter : AbstractGenericJsonConverter<Type>
	{
		/// <summary>
		/// Writes the JSON representation of the object.
		/// </summary>
		/// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.</param>
		/// <param name="value">The value.</param>
		/// <param name="serializer">The calling serializer.</param>
		public override void WriteJson(JsonWriter writer, Type value, JsonSerializer serializer)
		{
			writer.WriteType(value);
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
		public override Type ReadJson(JsonReader reader, Type existingValue, JsonSerializer serializer)
		{
			return reader.GetValueAsType();
		}
	}
}
