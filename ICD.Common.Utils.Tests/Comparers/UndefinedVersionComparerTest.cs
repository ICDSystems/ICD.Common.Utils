using System;
using ICD.Common.Utils.Comparers;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Comparers
{
	[TestFixture]
	public sealed class UndefinedVersionComparerTest
	{
		[Test]
		public void CompareToTest()
		{
			Assert.AreEqual(0, UndefinedVersionComparer.Instance.Compare(new Version(0, 0), new Version(0, 0, 0, 0)));
			Assert.AreEqual(-1, UndefinedVersionComparer.Instance.Compare(new Version(0, 0), new Version(1, 0, 0, 0)));
			Assert.AreEqual(1, UndefinedVersionComparer.Instance.Compare(new Version(1, 0), new Version(0, 0, 0, 0)));
		}
	}
}
