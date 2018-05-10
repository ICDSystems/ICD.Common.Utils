using System.Collections.Generic;
using ICD.Common.Utils.Collections;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Collections
{
	[TestFixture]
	public sealed class PriorityQueueTest
	{
		#region Properties

		[Test]
		public void CountTest()
		{
			PriorityQueue<int> queue = new PriorityQueue<int>();

			Assert.AreEqual(0, queue.Count);

			queue.Enqueue(1);
			queue.Enqueue(2);
			queue.Enqueue(3);

			Assert.AreEqual(3, queue.Count);

			queue.Clear();

			Assert.AreEqual(0, queue.Count);
		}

		[Test]
		public void IsSynchronizedTest()
		{
			PriorityQueue<int> queue = new PriorityQueue<int>();

			Assert.IsFalse(queue.IsSynchronized);
		}

		[Test]
		public void SyncRootTest()
		{
			PriorityQueue<int> queue = new PriorityQueue<int>();

			Assert.AreEqual(queue, queue.SyncRoot);
		}

		#endregion

		#region Methods

		[Test]
		public void ClearTest()
		{
			PriorityQueue<int> queue = new PriorityQueue<int>();

			queue.Enqueue(1);
			queue.Enqueue(2);
			queue.Enqueue(3);

			Assert.AreEqual(3, queue.Count);

			queue.Clear();

			Assert.AreEqual(0, queue.Count);
		}

		[Test]
		public void EnqueueTest()
		{
			PriorityQueue<int> queue = new PriorityQueue<int>();

			queue.Enqueue(1);
			queue.Enqueue(2);
			queue.Enqueue(3);

			Assert.AreEqual(3, queue.Count);
		}

		[Test]
		public void EnqueuePriorityTest()
		{
			PriorityQueue<int> queue = new PriorityQueue<int>();

			queue.Enqueue(1, 3);
			queue.Enqueue(2, 2);
			queue.Enqueue(3, 1);

			Assert.AreEqual(3, queue.Count);

			List<int> dequeue = new List<int>
			{
				queue.Dequeue(),
				queue.Dequeue(),
				queue.Dequeue()
			};

			Assert.AreEqual(3, dequeue[0]);
			Assert.AreEqual(2, dequeue[1]);
			Assert.AreEqual(1, dequeue[2]);
		}

		[Test]
		public void EnqueueFirstTest()
		{
			PriorityQueue<int> queue = new PriorityQueue<int>();

			queue.Enqueue(1, int.MaxValue);
			queue.Enqueue(2, int.MinValue);
			queue.EnqueueFirst(3);

			Assert.AreEqual(3, queue.Count);

			Assert.AreEqual(3, queue.Dequeue());
		}

		[Test]
		public void EnqueueRemoveTest()
		{
			PriorityQueue<int> queue = new PriorityQueue<int>();

			queue.Enqueue(1);
			queue.Enqueue(2);
			queue.Enqueue(3);

			queue.EnqueueRemove(4, i => i == 2);

			Assert.AreEqual(3, queue.Count);

			List<int> dequeue = new List<int>
			{
				queue.Dequeue(),
				queue.Dequeue(),
				queue.Dequeue()
			};

			Assert.AreEqual(1, dequeue[0]);
			Assert.AreEqual(4, dequeue[1]);
			Assert.AreEqual(3, dequeue[2]);
		}

		[Test]
		public void EnqueueRemovePriorityTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void DequeueTest()
		{
			PriorityQueue<int> queue = new PriorityQueue<int>();

			queue.Enqueue(1, 3);
			queue.Enqueue(2, 2);
			queue.Enqueue(3, 1);

			Assert.AreEqual(3, queue.Count);

			List<int> dequeue = new List<int>
			{
				queue.Dequeue(),
				queue.Dequeue(),
				queue.Dequeue()
			};

			Assert.AreEqual(3, dequeue[0]);
			Assert.AreEqual(2, dequeue[1]);
			Assert.AreEqual(1, dequeue[2]);

			Assert.AreEqual(0, queue.Count);
		}

		[Test]
		public void GetEnumeratorTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void CopyToTest()
		{
			Assert.Inconclusive();
		}

		#endregion
	}
}
