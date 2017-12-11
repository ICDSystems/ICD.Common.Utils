using ICD.Common.Utils.Timers;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Timers
{
	[TestFixture]
	public sealed class SafeTimerTest
	{
		[Test]
		public void DisposeTest()
		{
			bool called = false;
			SafeTimer timer = SafeTimer.Stopped(() => called = true);

			// Crestron timer tends to execute the callback on stop.
			timer.Dispose();
			Assert.IsFalse(called);
		}

		[Test]
		public void StoppedTest()
		{
			bool called = false;
			SafeTimer timer = SafeTimer.Stopped(() => called = true);

			Assert.IsFalse(called);

			timer.Dispose();
		}

		[Test]
		public void StopTest()
		{
			bool called = false;
			SafeTimer timer = SafeTimer.Stopped(() => called = true);
			timer.Reset(100);
			timer.Stop();

			ThreadingUtils.Sleep(200);

			Assert.IsFalse(called);

			timer.Dispose();
		}

		[Test]
		public void TriggerTest()
		{
			bool called = false;
			SafeTimer timer = SafeTimer.Stopped(() => called = true);

			timer.Trigger();

			Assert.IsTrue(called);

			timer.Dispose();
		}

		[Test]
		public void ResetTest()
		{
			int called = 0;
			SafeTimer timer = SafeTimer.Stopped(() => called++);

			timer.Reset(10);

			ThreadingUtils.Sleep(50);

			Assert.AreEqual(1, called);

			timer.Dispose();
		}

		[Test]
		public void ResetRepeatTest()
		{
			int called = 0;
			SafeTimer timer = SafeTimer.Stopped(() => called++);

			timer.Reset(100, 100);

			ThreadingUtils.Sleep(500);

			Assert.AreEqual(4, called, 2);

			timer.Dispose();
		}
	}
}
