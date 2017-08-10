using NUnit.Framework;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils.Tests.Extensions
{
    [TestFixture]
    public sealed class QueueExtensionsTest
    {
        [Test]
        public void EnqueueRangeTest()
        {
            Queue<int> queue = new Queue<int>();

            queue.EnqueueRange(new int[] { 1, 2, 3 });

            Assert.AreEqual(3, queue.Count);
            Assert.AreEqual(1, queue.Dequeue());
            Assert.AreEqual(2, queue.Dequeue());
            Assert.AreEqual(3, queue.Dequeue());
        }

        [Test]
        public void DequeueTest()
        {
            Queue<int> queue = new Queue<int>();
            queue.EnqueueRange(new int[] { 1, 2, 3 });

            int output;

            Assert.AreEqual(3, queue.Count);
            Assert.AreEqual(true, queue.Dequeue(out output));
            Assert.AreEqual(1, output);

            Assert.AreEqual(2, queue.Count);
            Assert.AreEqual(true, queue.Dequeue(out output));
            Assert.AreEqual(2, output);

            Assert.AreEqual(1, queue.Count);
            Assert.AreEqual(true, queue.Dequeue(out output));
            Assert.AreEqual(3, output);

            Assert.AreEqual(0, queue.Count);
            Assert.AreEqual(false, queue.Dequeue(out output));
            Assert.AreEqual(0, output);
        }
    }
}
