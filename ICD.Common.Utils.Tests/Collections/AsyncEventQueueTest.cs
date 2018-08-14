using System.Collections.Generic;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Collections
{
	[TestFixture]
	public sealed class AsyncEventQueueTest
	{
		[Test]
		public void ItemDequeuedFeedbackTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void CountTest()
		{
			using (AsyncEventQueue<int> queue = new AsyncEventQueue<int>())
			{
				queue.OnItemDequeued +=
					(sender, args) =>
					{
						ThreadingUtils.Sleep(200);
					};

				Assert.AreEqual(0, queue.Count);

				for (int index = 0; index < 5; index++)
					queue.Enqueue(index);

				Assert.AreEqual(5, queue.Count);
			}
		}

		[Test]
		public void EnqueueTest()
		{
			using (AsyncEventQueue<int> queue = new AsyncEventQueue<int>())
			{
				List<GenericEventArgs<int>> eventArgs = new List<GenericEventArgs<int>>();
				queue.OnItemDequeued +=
					(sender, args) =>
					{
						eventArgs.Add(args);
					};

				Assert.AreEqual(0, eventArgs.Count);

				for (int index = 0; index < 5; index++)
					queue.Enqueue(index);

				ThreadingUtils.Sleep(500);

				Assert.AreEqual(5, eventArgs.Count);

				Assert.AreEqual(0, eventArgs[0].Data);
				Assert.AreEqual(1, eventArgs[1].Data);
				Assert.AreEqual(2, eventArgs[2].Data);
				Assert.AreEqual(3, eventArgs[3].Data);
				Assert.AreEqual(4, eventArgs[4].Data);
			}
		}

		[Test]
		public void ClearTest()
		{
			using (AsyncEventQueue<int> queue = new AsyncEventQueue<int>())
			{
				queue.OnItemDequeued +=
					(sender, args) =>
					{
						ThreadingUtils.Sleep(200);
					};

				Assert.AreEqual(0, queue.Count);

				for (int index = 0; index < 5; index++)
					queue.Enqueue(index);

				Assert.AreEqual(5, queue.Count);

				queue.Clear();

				Assert.AreEqual(0, queue.Count);
			}
		}
	}
}
