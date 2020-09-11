using System;
using ICD.Common.Utils.Comparers;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Comparers
{
	[TestFixture]
	public sealed class UndefinedVersionEqualityComparerTest
	{
		[Test]
		public void Equals()
		{
			Assert.IsTrue(UndefinedVersionEqualityComparer.Instance.Equals(new Version(0, 0), new Version(0, 0, 0, 0)));
			Assert.IsFalse(UndefinedVersionEqualityComparer.Instance.Equals(new Version(0, 0), new Version(1, 0, 0, 0)));
		}
	}
}
