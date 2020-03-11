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
			DateTime time = IcdEnvironment.GetUtcTime();

			ThreadingUtils.SafeInvoke(() =>
			                          {
				                          ThreadingUtils.Sleep(100);
				                          complete = true;
			                          });

			Assert.IsTrue(ThreadingUtils.Wait(() => complete, 200));
			Assert.AreEqual(100, (IcdEnvironment.GetUtcTime() - time).TotalMilliseconds, 20);
		}

		[Test]
		public void Sleep()
		{
			DateTime now = IcdEnvironment.GetUtcTime();
			ThreadingUtils.Sleep(1000);
			DateTime now2 = IcdEnvironment.GetUtcTime();

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
			Assert.IsTrue(ThreadingUtils.Wait(() => result, 1000));
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
			Assert.IsTrue(ThreadingUtils.Wait(() => result, 1000));
		}
	}
}
