using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.IO;
using ICD.Common.Utils.Json;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Json
{
	[TestFixture]
	public sealed class JsonUtilsTest
	{
		[Test]
		public void FormatTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void SerializeTest()
		{
			TestSerializable serializable = new TestSerializable {Test = 100};
			string json = JsonUtils.Serialize(w => serializable.Serialize(w));

			using (JsonTextReader reader = new JsonTextReader(new IcdStringReader(json).WrappedStringReader))
			{
				int test = 0;

				while (reader.Read())
				{
					if (reader.TokenType == JsonToken.EndObject)
					{
						reader.Read();
						break;
					}

					if (reader.TokenType != JsonToken.PropertyName)
						continue;

					string property = reader.Value as string;

					// Read to the value
					reader.Read();

					switch (property)
					{
						case "Test":
							test = reader.GetValueAsInt();
							break;
					}
				}

				Assert.AreEqual(100, test);
			}
		}

		[Test]
		public void DeserializeTest()
		{
			const string json = "{\"Test\":100}";
			TestSerializable serializable = JsonUtils.Deserialize(r => TestSerializable.Deserialize(r), json);

			Assert.AreEqual(100, serializable.Test);
		}

		[Test]
		public void SerializeMessageTest()
		{
			TestSerializable serializable = new TestSerializable {Test = 100};
			string json = JsonUtils.SerializeMessage(w => serializable.Serialize(w), "test message");

			using (JsonTextReader reader = new JsonTextReader(new IcdStringReader(json).WrappedStringReader))
			{
				int test = 0;
				string messageName = null;

				while (reader.Read())
				{
					if (reader.TokenType == JsonToken.EndObject)
					{
						reader.Read();
						break;
					}

					if (reader.TokenType != JsonToken.PropertyName)
						continue;

					string property = reader.Value as string;

					// Read to the value
					reader.Read();

					switch (property)
					{
						case "Test":
							test = reader.GetValueAsInt();
							break;

						case "m":
							messageName = reader.GetValueAsString();
							break;
					}
				}

				Assert.AreEqual(100, test);
				Assert.AreEqual("test message", messageName);
			}
		}

		[Test]
		public void DeserializeMessageTest()
		{
			const string json = "{\"m\":\"test message\",\"d\":{\"Test\":100}}";
			string messageName = null;
			TestSerializable serializable = JsonUtils.DeserializeMessage((r, m) =>
			                                                             {
				                                                             messageName = m;
				                                                             return TestSerializable.Deserialize(r);
			                                                             },
			                                                             json);

			Assert.AreEqual(100, serializable.Test);
			Assert.AreEqual("test message", messageName);
		}

		public sealed class TestSerializable
		{
			private const string PROPERTY_NAME = "Test";

			public int Test { get; set; }

			public void Serialize(JsonWriter writer)
			{
				writer.WriteStartObject();
				{
					writer.WritePropertyName(PROPERTY_NAME);
					writer.WriteValue(Test);
				}
				writer.WriteEndObject();
			}

			public static TestSerializable Deserialize(JsonReader reader)
			{
				int test = 0;

				while (reader.Read())
				{
					if (reader.TokenType == JsonToken.EndObject)
					{
						reader.Read();
						break;
					}

					if (reader.TokenType != JsonToken.PropertyName)
						continue;

					string property = reader.Value as string;

					// Read to the value
					reader.Read();

					switch (property)
					{
						case PROPERTY_NAME:
							test = reader.GetValueAsInt();
							break;
					}
				}

				return new TestSerializable {Test = test};
			}
		}
	}
}
