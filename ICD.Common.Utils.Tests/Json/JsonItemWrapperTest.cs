using System;
using System.Collections.Generic;
using ICD.Common.Utils.Json;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Json
{
	[TestFixture]
	public sealed class JsonItemWrapperTest
	{
		[TestCase(null, null)]
		[TestCase("", typeof(string))]
		[TestCase(1, typeof(int))]
		public void ItemTypeTest(object item, Type expected)
		{
			Assert.AreEqual(expected, new JsonItemWrapper(item).Type);
		}

		[TestCase("")]
		[TestCase(1)]
		public void ItemTest(object item)
		{
			Assert.AreEqual(item, new JsonItemWrapper(item).Item);
		}

		[Test]
		public void WriteTest()
		{
			const string expected = @"{""t"":""System.Collections.Generic.List`1[[System.Int32]]"",""i"":[1,2,3]}";

			JsonItemWrapper wrapper = new JsonItemWrapper(new List<int> {1, 2, 3});
			string json = JsonConvert.SerializeObject(wrapper);

			Assert.AreEqual(expected, json);
		}

		[Test]
		public void ReadTest()
		{
			const string json = @"{""t"":""System.Collections.Generic.List`1[[System.Int32]]"",""i"":[1,2,3]}";

			JsonItemWrapper wrapper = JsonConvert.DeserializeObject<JsonItemWrapper>(json);
			List<int> wrappedObject = wrapper.Item as List<int>;

			Assert.NotNull(wrappedObject);
			Assert.AreEqual(3, wrappedObject.Count);
		}
	}
}
