using System;
using ICD.Common.Utils.Extensions;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Extensions
{
	[TestFixture]
	public sealed class DateTimeExtensionsTest
	{
		[TestCase(1, 1)]
		[TestCase(0, 12)]
		[TestCase(12, 12)]
		[TestCase(23, 11)]
		public void Get12Hour(int hour, int expected)
		{
			Assert.AreEqual(expected, new DateTime(2019, 1, 1, hour, 0, 0).Get12Hour());
		}

		[Test]
		public void ToShortTimeStringTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void ToLongTimeStringWithMillisecondsTest()
		{
			Assert.Inconclusive();
		}
	}
}
