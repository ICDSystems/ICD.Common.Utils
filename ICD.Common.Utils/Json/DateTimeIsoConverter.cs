using System;
using System.Text.RegularExpressions;
using ICD.Common.Utils.Extensions;
using Newtonsoft.Json;

namespace ICD.Common.Utils.Json
{
	public sealed class DateTimeIsoConverter : AbstractGenericJsonConverter<DateTime>
	{
		/// <summary>
		/// Writes the JSON representation of the object.
		/// </summary>
		/// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.</param>
		/// <param name="value">The value.</param>
		/// <param name="serializer">The calling serializer.</param>
		public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToIso());
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
		public override DateTime ReadJson(JsonReader reader, DateTime existingValue, JsonSerializer serializer)
		{
			/*
			"\"\\/Date(1335205592410)\\/\""         .NET JavaScriptSerializer
			"\"\\/Date(1335205592410-0500)\\/\""    .NET DataContractJsonSerializer
			"2012-04-23T18:25:43.511Z"              JavaScript built-in JSON object
			"2012-04-21T18:25:43-05:00"             ISO 8601
			 */

			string serial = reader.GetValueAsString();

			Match match;
			if (RegexUtils.Matches(serial, @"Date\((?'date'\d+)(?'zone'(-|\+)\d+)?\)", out match))
			{
				long ms = long.Parse(match.Groups["date"].Value);
				DateTime dateTime = DateTimeUtils.FromEpochMilliseconds(ms);
				if (!match.Groups["zone"].Success)
					return dateTime;

				// No TimeZoneInfo in CF, so now things get gross
				dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
				string iso = dateTime.ToIso() + match.Groups["zone"].Value;
				return DateTime.Parse(iso);
			}

			return DateTime.Parse(serial);
		}
	}
}
