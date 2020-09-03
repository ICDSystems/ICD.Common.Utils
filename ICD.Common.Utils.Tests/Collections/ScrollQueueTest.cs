using System.Linq;
using ICD.Common.Properties;
using NUnit.Framework;
using ICD.Common.Utils.Collections;

namespace ICD.Common.Utils.Tests.Collections
{
	[TestFixture]
	public sealed class ScrollQueueTest
	{
		[Test, UsedImplicitly]
		public void MaxSizeTest()
		{
			int unused;

			ScrollQueue<int> test = new ScrollQueue<int>(5);
			test.Enqueue(0, out unused);
			test.Enqueue(1, out unused);
			test.Enqueue(2, out unused);
			test.Enqueue(3, out unused);
			test.Enqueue(4, out unused);

			Assert.AreEqual(5, test.Count);

			test.MaxSize = 3;

			Assert.AreEqual(3, test.Count);
			Assert.AreEqual(2, test.Peek());

			test.Enqueue(0, out unused);

			Assert.AreEqual(3, test.Count);
			Assert.AreEqual(3, test.Peek());
		}

		[Test, UsedImplicitly]
		public void ClearTest()
		{
			int unused;

			ScrollQueue<int> test = new ScrollQueue<int>(5);
			test.Enqueue(0, out unused);
			test.Clear();

			Assert.AreEqual(0, test.Count);
		}

		[Test, UsedImplicitly]
		public void EnqueueTest()
		{
			int unused;

			ScrollQueue<int> test = new ScrollQueue<int>(5);
			test.Enqueue(0, out unused);
			test.Enqueue(1, out unused);

			Assert.AreEqual(2, test.Count);

			int[] array = test.ToArray();

			Assert.AreEqual(0, array[0]);
			Assert.AreEqual(1, array[1]);
		}

		[Test, UsedImplicitly]
		public void DequeueTest()
		{
			int unused;

			ScrollQueue<int> test = new ScrollQueue<int>(5);
			test.Enqueue(0, out unused);
			test.Enqueue(1, out unused);

			Assert.AreEqual(0, test.Dequeue());
			Assert.AreEqual(1, test.Count);
		}

		[Test, UsedImplicitly]
		public void PeekTest()
		{
			int unused;

			ScrollQueue<int> test = new ScrollQueue<int>(5);
			test.Enqueue(0, out unused);
			test.Enqueue(1, out unused);

			Assert.AreEqual(0, test.Peek());
		}
	}
}