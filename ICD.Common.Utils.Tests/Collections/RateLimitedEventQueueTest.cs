using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Collections
{
	[TestFixture]
	public sealed class RateLimitedEventQueueTest
	{
		[Test]
		public void ItemDequeuedFeedbackTest()
		{
			List<GenericEventArgs<int>> callbacks = new List<GenericEventArgs<int>>();

			using (RateLimitedEventQueue<int> queue = new RateLimitedEventQueue<int> { BetweenMilliseconds = 1000 })
			{
				queue.OnItemDequeued += (sender, args) => callbacks.Add(args);

				queue.Enqueue(10);
				queue.Enqueue(20);
				queue.Enqueue(30);

				ThreadingUtils.Sleep(100);

				Assert.AreEqual(1, callbacks.Count, "Initial enqueue did not trigger a dequeue");

				queue.OnItemDequeued += (sender, args) => { ThreadingUtils.Sleep(1000); };
				ThreadingUtils.Sleep(1000);

				Assert.AreEqual(2, callbacks.Count, "Second enqueue did not dequeue");

				ThreadingUtils.Sleep(1000);

				Assert.AreEqual(2, callbacks.Count, "Third enqueue did not wait for process to complete");

				ThreadingUtils.Sleep(1000);

				Assert.AreEqual(3, callbacks.Count);
			}
		}

		#region Properties

		[TestCase(1000)]
		public void BetweenMillisecondsTest(long milliseconds)
		{
			using (RateLimitedEventQueue<int> queue = new RateLimitedEventQueue<int> { BetweenMilliseconds = milliseconds })
				Assert.AreEqual(milliseconds, queue.BetweenMilliseconds);
		}

		[Test]
		public void CountTest()
		{
			using (RateLimitedEventQueue<int> queue = new RateLimitedEventQueue<int> { BetweenMilliseconds = 100 * 1000 })
			{
				queue.Enqueue(1);
				queue.Enqueue(1);
				queue.Enqueue(1);

				Assert.AreEqual(3, queue.Count);
			}
		}

		#endregion

		#region Methods

		[Test]
		public void EnqueueTest()
		{
			using (RateLimitedEventQueue<int> queue = new RateLimitedEventQueue<int> { BetweenMilliseconds = 100 * 1000 })
			{
				queue.Enqueue(10);
				queue.Enqueue(20);
				queue.Enqueue(30);

				Assert.True(queue.SequenceEqual(new[] { 10, 20, 30 }));
			}
		}

		[Test]
		public void ClearTest()
		{
			using (RateLimitedEventQueue<int> queue = new RateLimitedEventQueue<int> { BetweenMilliseconds = 100 * 1000 })
			{
				queue.Enqueue(1);
				queue.Enqueue(1);
				queue.Enqueue(1);

				queue.Clear();

				Assert.AreEqual(0, queue.Count);
			}
		}

		#endregion
	}
}
