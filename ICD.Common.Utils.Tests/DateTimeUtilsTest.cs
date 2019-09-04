using NUnit.Framework;

namespace ICD.Common.Utils.Tests
{
	[TestFixture]
	public sealed class DateTimeUtilsTest
	{
		[TestCase(1, 1)]
		[TestCase(0, 12)]
		[TestCase(12, 12)]
		[TestCase(23, 11)]
		public void Get12Hour(int hour, int expected)
		{
			Assert.AreEqual(expected, DateTimeUtils.To12Hour(hour));
		}
	}
}
