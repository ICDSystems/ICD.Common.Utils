using ICD.Common.Utils.Timers;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Timers
{
	[TestFixture]
	public sealed class IcdStopwatchTest
	{
		[Test]
		public void ConstructorTest()
		{
			var stopwatch = new IcdStopwatch();
			ThreadingUtils.Sleep(100);
			Assert.AreEqual(0, stopwatch.ElapsedMilliseconds);
		}
	}
}
