using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests
{
	[TestFixture]
	public sealed class ThreadedWorkerQueueTest
	{
		[Test]
		public void BetweenTimeTest()
		{
			List<int> callbacks = new List<int>();

			using (ThreadedWorkerQueue<int> queue = new ThreadedWorkerQueue<int>((d) => callbacks.Add(d), true, 1000))
			{

				queue.Enqueue(10);
				queue.Enqueue(20);
				queue.Enqueue(30);

				ThreadingUtils.Sleep(100);

				Assert.AreEqual(1, callbacks.Count, "Initial enqueue did not trigger a dequeue");

				ThreadingUtils.Sleep(1000);

				Assert.AreEqual(2, callbacks.Count, "Second enqueue did not dequeue");

				ThreadingUtils.Sleep(100);

				Assert.AreEqual(2, callbacks.Count, "Third enqueue did not wait for process to complete");

				ThreadingUtils.Sleep(1000);

				Assert.AreEqual(3, callbacks.Count);
			}
		}

		#region Properties

		[TestCase(1000)]
		[TestCase(0)]
		[TestCase(long.MaxValue)]
		public void BetweenMillisecondsTest(long milliseconds)
		{
			using (var queue = new ThreadedWorkerQueue<int>((d) => { }, true, milliseconds))
				Assert.AreEqual(milliseconds, queue.BetweenTime);
		}

		[TestCase(5)]
		[TestCase(0)]
		[TestCase(30)]
		public void CountTest(int count)
		{
			using (var queue = new ThreadedWorkerQueue<int>(d => { }, false))
			{
				for (int i = 0; i < count; i++)
					queue.Enqueue(1);

				Assert.AreEqual(count, queue.Count);
			}
		}

		[Test]
		public void ProcessBetweenTimeTest()
		{
			var processed = new List<int>();

			using (var queue = new ThreadedWorkerQueue<int>(d => processed.Add(d), false, 1000))
			{
				queue.Enqueue(1);
				queue.Enqueue(2);
				queue.Enqueue(3);
				
				ThreadingUtils.Sleep(100);
				Assert.AreEqual(0, processed.Count, "Queue processed item early");

				queue.SetRunProcess(true);
				
				ThreadingUtils.Sleep(100);

				Assert.AreEqual(1, processed.Count, "First item not processed");

				ThreadingUtils.Sleep(1000);

				Assert.AreEqual(2, processed.Count, "Second item not processed");

				queue.SetRunProcess(false);

				ThreadingUtils.Sleep(2000);

				Assert.AreEqual(2, processed.Count, "Item processed after stopping run process");
				Assert.AreEqual(1, queue.Count, "Incorrect number of items in queue");

				// Queue lower priority item
				queue.Enqueue(5, 1);
				queue.SetRunProcess(true);

				ThreadingUtils.Sleep(100);

				Assert.AreEqual(3, processed.Count, "Third item not processed");
				Assert.AreEqual(5, processed[2], "Dequeued incorrect priority item");

				ThreadingUtils.Sleep(1000);

				Assert.AreEqual(4, processed.Count, "Didn't process all items");
				Assert.True(processed.SequenceEqual(new[] { 1, 2, 5, 3 }), "Processed sequence incorrect");
			}
		}

		[Test]
		public void FlushQueueBetweenTimeTest()
		{
			var processed = new List<int>();

			using (var queue = new ThreadedWorkerQueue<int>(d => processed.Add(d), false, 1000))
			{
				Assert.True(queue.WaitForFlush(1), "WaitForFlush on empty queue failed");

				queue.Enqueue(11);
				queue.Enqueue(21);
				queue.Enqueue(31);
				queue.Enqueue(41);
				queue.Enqueue(51);
				queue.Enqueue(61);
				queue.Enqueue(71);
				queue.Enqueue(81);
				queue.Enqueue(91);
				queue.Enqueue(101);

				queue.SetRunProcess(true);

				Assert.False(queue.WaitForFlush(1250), "WaitForFlush didn't time out");
				Assert.AreEqual(2, processed.Count, "Didn't process correct number of items in time frame");

				Assert.True(queue.WaitForFlush(), "WaitForFlush failed");
				Assert.AreEqual(10, processed.Count, "Not all items processed");
				Assert.AreEqual(0, queue.Count, "Queue not empty");
			}
		}

		#endregion

		#region Methods

		[TestCase(false)]
		[TestCase(true)]
		public void SetRunProcessTest(bool runProcess)
		{
			using (var queue = new ThreadedWorkerQueue<int>(d => { }, !runProcess))
			{
				Assert.AreEqual(!runProcess, queue.RunProcess, "Initial state wrong");
				queue.SetRunProcess(runProcess);
				Assert.AreEqual(runProcess, queue.RunProcess, "Didn't set to correct state 1st time");
				queue.SetRunProcess(!runProcess);
				Assert.AreEqual(!runProcess, queue.RunProcess, "Didn't set to correct state 2nd time");
			}
		}

		[Test]
		public void EnqueueTest()
		{
			var processed = new List<int>();

			using (var queue = new ThreadedWorkerQueue<int>(d => processed.Add(d), false))
			{
				queue.Enqueue(10);
				queue.Enqueue(20);
				queue.Enqueue(30);

				Assert.AreEqual(3, queue.Count, "First queue count wrong");

				queue.Enqueue(40);
				queue.Enqueue(50);
				queue.Enqueue(60);

				Assert.AreEqual(6, queue.Count, "Second queue count wrong");

				queue.SetRunProcess(true);
				Assert.True(queue.WaitForFlush(),"Queue didn't flush after processing");


				Assert.True(processed.SequenceEqual(new[] { 10, 20, 30, 40, 50, 60 }), "Processed sequence wrong");
			}
		}

		[Test]
		public void ClearTest()
		{
			using (var queue = new ThreadedWorkerQueue<int>(d => { }, false))
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
