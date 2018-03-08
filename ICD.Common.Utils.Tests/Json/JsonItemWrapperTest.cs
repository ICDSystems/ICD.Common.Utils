using System;
using System.Collections.Generic;
using ICD.Common.Utils.Json;
using Newtonsoft.Json.Linq;
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
			Assert.AreEqual(expected, new JsonItemWrapper(item).ItemType);
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
			JsonItemWrapper wrapper = new JsonItemWrapper(new List<int> {1, 2, 3});
			string json = JsonUtils.Serialize(wrapper.Write);

			Assert.Inconclusive();
		}

		[Test]
		public void ReadTest()
		{
			const string json = "{\"t\":\"System.Collections.Generic.List`1[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]\",\"i\":\"[1,2,3]\"}";

			JObject jObject = JObject.Parse(json);
			List<int> wrappedObject = JsonItemWrapper.ReadToObject(jObject) as List<int>;

			Assert.NotNull(wrappedObject);
			Assert.AreEqual(3, wrappedObject.Count);
		}
	}
}
