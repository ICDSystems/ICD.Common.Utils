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
			ScrollQueue<int> test = new ScrollQueue<int>(5);
			test.Enqueue(0);
			test.Enqueue(1);
			test.Enqueue(2);
			test.Enqueue(3);
			test.Enqueue(4);

			Assert.AreEqual(5, test.Count);

			test.MaxSize = 3;

			Assert.AreEqual(3, test.Count);
			Assert.AreEqual(2, test.Peek());

			test.Enqueue(0);

			Assert.AreEqual(3, test.Count);
			Assert.AreEqual(3, test.Peek());
		}

		[Test, UsedImplicitly]
		public void ClearTest()
		{
			ScrollQueue<int> test = new ScrollQueue<int>(5);
			test.Enqueue(0);
			test.Clear();

			Assert.AreEqual(0, test.Count);
		}

		[Test, UsedImplicitly]
		public void EnqueueTest()
		{
			ScrollQueue<int> test = new ScrollQueue<int>(5);
			test.Enqueue(0);
			test.Enqueue(1);

			Assert.AreEqual(2, test.Count);

			int[] array = test.ToArray();

			Assert.AreEqual(0, array[0]);
			Assert.AreEqual(1, array[1]);
		}

		[Test, UsedImplicitly]
		public void DequeueTest()
		{
			ScrollQueue<int> test = new ScrollQueue<int>(5);
			test.Enqueue(0);
			test.Enqueue(1);

			Assert.AreEqual(0, test.Dequeue());
			Assert.AreEqual(1, test.Count);
		}

		[Test, UsedImplicitly]
		public void PeekTest()
		{
			ScrollQueue<int> test = new ScrollQueue<int>(5);
			test.Enqueue(0);
			test.Enqueue(1);

			Assert.AreEqual(0, test.Peek());
		}
	}
}