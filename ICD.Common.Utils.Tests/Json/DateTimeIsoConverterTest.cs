using System;
using ICD.Common.Utils.Json;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Json
{
	[TestFixture]
	public sealed class DateTimeIsoConverterTest : AbstractGenericJsonConverterTest
	{
		[Test]
		public override void WriteJsonTest()
		{
			JsonSerializerSettings settings = new JsonSerializerSettings
			{
				DateParseHandling = DateParseHandling.None
			};
			settings.Converters.Add(new DateTimeIsoConverter());

			DateTime dateTime = new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc);
			string serial = JsonConvert.SerializeObject(dateTime, settings);

			Assert.AreEqual("\"2020-01-02T03:04:05Z\"", serial);
		}

		[Test]
		public override void ReadJsonTest()
		{
			JsonSerializerSettings settings = new JsonSerializerSettings
			{
				DateParseHandling = DateParseHandling.None
			};
			settings.Converters.Add(new DateTimeIsoConverter());

			string serial = "\"2020-01-02T03:04:05Z\"";
			DateTime dateTime = JsonConvert.DeserializeObject<DateTime>(serial, settings);

			Assert.AreEqual(new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc), dateTime);
		}
	}
}
