using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICD.Common.Utils.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Extensions
{
	[TestFixture]
	public sealed class JsonExtensionsTest
	{
		[Test]
		public void WriteObjectTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void GetValueAsIntTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void GetValueAsStringTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void GetValueAsBoolTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void GetValueAsEnumTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void SerializeArrayTest()
		{
			JsonSerializer serializer = new JsonSerializer();
			StringBuilder stringBuilder = new StringBuilder();

			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				using (JsonWriter writer = new JsonTextWriter(stringWriter))
				{
					serializer.SerializeArray(writer, new[] {1, 2, 3, 4});
				}
			}

			string json = stringBuilder.ToString();
			Assert.AreEqual("[1,2,3,4]", json);
		}

		[Test]
		public void DeserializeArrayTest()
		{
			const string json = "[1,2,3,4]";

			JsonSerializer serializer = new JsonSerializer();
			int[] deserialized;

			using (StringReader stringReader = new StringReader(json))
			{
				using (JsonReader reader = new JsonTextReader(stringReader))
				{
					reader.Read();
					deserialized = serializer.DeserializeArray<int>(reader).ToArray();
				}
			}

			Assert.IsTrue(deserialized.SequenceEqual(new[] {1, 2, 3, 4}));
		}

		[Test]
		public void SerializeDictionaryTest()
		{
			Dictionary<int, string> dict = new Dictionary<int, string>
			{
				{1, "Item 1"},
				{10, "Item 2"},
				{15, "Item 3"}
			};

			JsonSerializer serializer = new JsonSerializer();
			StringBuilder stringBuilder = new StringBuilder();

			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				using (JsonWriter writer = new JsonTextWriter(stringWriter))
				{
					serializer.SerializeDictionary(writer, dict);
				}
			}

			string json = stringBuilder.ToString();
			Assert.AreEqual("{\"1\":\"Item 1\",\"10\":\"Item 2\",\"15\":\"Item 3\"}", json);
		}

		[Test]
		public void DeserializeDictionaryTest()
		{
			const string json = "{\"1\":\"Item 1\",\"10\":\"Item 2\",\"15\":\"Item 3\"}";

			JsonSerializer serializer = new JsonSerializer();
			Dictionary<int, string> deserialized;

			using (StringReader stringReader = new StringReader(json))
			{
				using (JsonReader reader = new JsonTextReader(stringReader))
				{
					reader.Read();
					deserialized = serializer.DeserializeDictionary<int, string>(reader).ToDictionary();
				}
			}

			Assert.AreEqual(3, deserialized.Count);
			Assert.AreEqual("Item 1", deserialized[1]);
			Assert.AreEqual("Item 2", deserialized[10]);
			Assert.AreEqual("Item 3", deserialized[15]);
		}
	}
}
