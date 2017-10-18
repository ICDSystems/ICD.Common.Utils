using System;
using ICD.Common.Utils.Json;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Json
{
	[TestFixture]
	public sealed class JsonItemWrapperTest
	{
		[TestCase(null, null)]
		public void ItemTypeStringTest(object item, string expected)
		{
			Assert.AreEqual(expected, new JsonItemWrapper(item).ItemTypeString);
		}

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
			Assert.Inconclusive();
		}

		[Test]
		public void ReadTest()
		{
			Assert.Inconclusive();
		}
	}
}
