using System;
using ICD.Common.Utils.Extensions;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Extensions
{
	[TestFixture]
	public sealed class TimeSpanExtensionsTest
	{
		[Test]
		public void ToReadableStringTest()
		{
			Assert.Inconclusive();
		}

		[TestCase(0, 0, 0)]
		[TestCase(12, 1, 13)]
		[TestCase(23, 1, 0)]
		[TestCase(6, -12, 18)]
		public void AddHoursAndWrapTest(int hours, int addHours, int expectedHours)
		{
			Assert.AreEqual(expectedHours, new TimeSpan(hours, 0, 0).AddHoursAndWrap(addHours).Hours);
		}

		[TestCase(0, 0, 0)]
		[TestCase(12, 1, 13)]
		[TestCase(23, 1, 12)]
		[TestCase(6, -12, 6)]
		public void AddHoursAndWrap12Hour(int hours, int addHours, int expectedHours)
		{
			Assert.AreEqual(expectedHours, new TimeSpan(hours, 0, 0).AddHoursAndWrap12Hour(addHours).Hours);
		}

		[TestCase(0, 0, 0)]
		[TestCase(30, 1, 31)]
		[TestCase(59, 1, 0)]
		[TestCase(30, -60, 30)]
		public void AddMinutesAndWrap(int minutes, int addMinutes, int expectedMinutes)
		{
			Assert.AreEqual(expectedMinutes, new TimeSpan(0, minutes, 0).AddMinutesAndWrap(addMinutes).Minutes);
		}
	}
}
