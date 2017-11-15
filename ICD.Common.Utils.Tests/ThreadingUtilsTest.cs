using System;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests
{
	[TestFixture]
	public sealed class ThreadingUtilsTest
	{
		[Test]
		public static void WaitTest()
		{
			Assert.IsFalse(ThreadingUtils.Wait(() => false, 100));
			Assert.IsTrue(ThreadingUtils.Wait(() => true, 100));

			bool complete = false;

			ThreadingUtils.SafeInvoke(() =>
			                          {
				                          ThreadingUtils.Sleep(50);
				                          complete = true;
			                          });

			Assert.IsTrue(ThreadingUtils.Wait(() => complete, 100));
		}

		[Test]
		public void Sleep()
		{
			DateTime now = IcdEnvironment.GetLocalTime();
			ThreadingUtils.Sleep(1000);
			DateTime now2 = IcdEnvironment.GetLocalTime();

			Assert.AreEqual(1000, (now2 - now).TotalMilliseconds, 100);
		}

		[Test]
		public void SafeInvokeTest()
		{
			bool result = false;
			ThreadingUtils.SafeInvoke(() =>
			                          {
				                          ThreadingUtils.Sleep(100);
				                          result = true;
			                          });

			Assert.IsFalse(result);
			ThreadingUtils.Sleep(1000);
			Assert.IsTrue(result);
		}

		[Test]
		public void SafeInvokeParamTest()
		{
			bool result = false;
			ThreadingUtils.SafeInvoke(p =>
			                          {
				                          ThreadingUtils.Sleep(100);
				                          result = p;
			                          }, true);

			Assert.IsFalse(result);
			ThreadingUtils.Sleep(1000);
			Assert.IsTrue(result);
		}
	}
}
