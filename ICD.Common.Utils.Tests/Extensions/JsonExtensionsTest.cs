using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.IO;
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
		public void ReadObjectTest()
		{
			const string json =
				"{\"name\":\"Test\",\"help\":\"Test test.\",\"type\":\"System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e\",\"value\":\"Test\"}";

			Dictionary<string, string> expected = new Dictionary<string, string>
			{
				{"name", "Test"},
				{"help", "Test test."},
				{"type", "System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"},
				{"value", "Test"}
			};

			Dictionary<string, string> deserialized = new Dictionary<string, string>();

			using (IcdStringReader textReader = new IcdStringReader(json))
			{
				using (JsonReader reader = new JsonTextReader(textReader.WrappedTextReader))
				{
					JsonSerializer serializer = new JsonSerializer();

					reader.Read();
					reader.ReadObject(serializer, (p, r, s) => deserialized.Add(p, (string)r.Value));
				}
			}

			Assert.IsTrue(deserialized.DictionaryEqual(expected));
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
	}
}
