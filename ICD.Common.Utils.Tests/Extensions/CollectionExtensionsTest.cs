using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Extensions
{
	[TestFixture]
	public sealed class CollectionExtensionsTest
	{
		[Test]
		public void RemoveAllPredicateTest()
		{
			List<int> a = new List<int> {1, 2, 2, 3};
			List<int> b = new List<int> {2, 3};

			((ICollection<int>)a).RemoveAll(i => b.Contains(i));

			Assert.AreEqual(1, a.Count);
			Assert.AreEqual(1, a[0]);
		}

		[Test]
		public void RemoveAllOtherTest()
		{
			List<int> a = new List<int> {1, 2, 2, 3};
			List<int> b = new List<int> {2, 3};

			a.RemoveAll(b);

			Assert.AreEqual(1, a.Count);
			Assert.AreEqual(1, a[0]);
		}
	}
}
