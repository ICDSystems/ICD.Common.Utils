using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests
{
	[TestFixture]
	class IcdManualResetEventTest
	{
		[TestCase(true)]
		[TestCase(false)]
		public void InitialStateTest(bool initialState)
		{
			using (var waitHandleEvent = new IcdManualResetEvent(initialState))
			{
				Assert.AreEqual(initialState, waitHandleEvent.WaitOne(1), "Initial state incorrect");
			}
		}

		[Test]
		public void DefaultInitialStateTest()
		{
			using (var waitHandleEvent = new IcdManualResetEvent())
			{
				Assert.False(waitHandleEvent.WaitOne(1), "Initial state incorrect");
			}
		}

		[TestCase(1)]
		[TestCase(5)]
		[TestCase(15)]
		public void MultipleThreadSetTest(int count)
		{
			int releasedCount = 0;

			SafeCriticalSection countSection = new SafeCriticalSection();

			
			IcdManualResetEvent waitHandleEvent = new IcdManualResetEvent(false);
			
			for (int i = 0; i < count; i++)
				ThreadingUtils.SafeInvoke(() =>
				                          {
					                          waitHandleEvent.WaitOne();
					                          countSection.Execute(() => releasedCount++);
					                          ;
				                          });

			Assert.AreEqual(0, countSection.Execute(() => releasedCount), "Threads released early");

			waitHandleEvent.Set();

			ThreadingUtils.Sleep(500);

			Assert.AreEqual(count, countSection.Execute(() => releasedCount), "Incorrect number of threads released");

			waitHandleEvent.Dispose();
		}

		[TestCase(1)]
		[TestCase(5)]
		[TestCase(15)]
		public void MultipleThreadResetTest(int count)
		{
			int releasedCount = 0;

			SafeCriticalSection countSection = new SafeCriticalSection();


			IcdManualResetEvent waitHandleEvent = new IcdManualResetEvent(true);

			Assert.True(waitHandleEvent.WaitOne(1), "Initial State Wrong");

			waitHandleEvent.Reset();

			for (int i = 0; i < count; i++)
				ThreadingUtils.SafeInvoke(() =>
				                          {
					                          if (waitHandleEvent.WaitOne(100))
												countSection.Execute(() => releasedCount++);
				                          });

			ThreadingUtils.Sleep(2000);

			Assert.AreEqual(0, countSection.Execute(() => releasedCount), "Incorrect number of threads released");

			waitHandleEvent.Set();

			Assert.AreEqual(0, countSection.Execute(() => releasedCount), "Threads released after timeout");

			waitHandleEvent.Dispose();
		}

		[TestCase(1)]
		[TestCase(200)]
		[TestCase(5000)]
		public void WaitOneTest(int waitTime)
		{
			bool released = false;

			IcdManualResetEvent waitHandleEvent = new IcdManualResetEvent(false);
			
			ThreadingUtils.SafeInvoke(() =>
				                          {
					                          waitHandleEvent.WaitOne();
					                          released = true;
				                          });

			ThreadingUtils.Sleep(waitTime);

			Assert.False(released, "Thread released when it shouldn't have");

			waitHandleEvent.Set();
			
			ThreadingUtils.Sleep(100);

			Assert.True(released, "Thread didn't release after set event");

			waitHandleEvent.Dispose();
		}

		[TestCase(200)]
		[TestCase(500)]
		[TestCase(5000)]
		public void WaitOneTimeoutTest(int waitTime)
		{
			bool released = false;

			IcdManualResetEvent waitHandleEvent = new IcdManualResetEvent(false);

			ThreadingUtils.SafeInvoke(() =>
			                          {
				                          waitHandleEvent.WaitOne(waitTime * 2);
				                          released = true;
			                          });

			ThreadingUtils.Sleep(waitTime);

			Assert.False(released, "Thread released when it shouldn't have");

			waitHandleEvent.Set();

			ThreadingUtils.Sleep(100);

			Assert.True(released, "Thread didn't release after set event");

			waitHandleEvent.Dispose();
		}

		[TestCase(200)]
		[TestCase(500)]
		[TestCase(5000)]
		public void WaitOneTimedOutTest(int waitTime)
		{
			bool released = false;
			bool? returned = null;

			IcdManualResetEvent waitHandleEvent = new IcdManualResetEvent(false);

			ThreadingUtils.SafeInvoke(() =>
			                          {
				                          returned = waitHandleEvent.WaitOne(waitTime);
				                          released = true;

			                          });

			ThreadingUtils.Sleep(100);

			Assert.True(returned == null, "WaitOne returned when it shouldn't have");
			Assert.False(released, "Thread released when it shouldn't have");

			ThreadingUtils.Sleep(waitTime * 2);

			Assert.True(released, "Thread didn't release after timeout");
			Assert.True(returned.HasValue && returned.Value == false, "WaitOne timeout didn't return false");

			waitHandleEvent.Set();

			waitHandleEvent.Dispose();
		}
	}
}
