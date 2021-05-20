using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Collections;

namespace ICD.Common.Utils
{
	/// <summary>
	/// Utilizes a priority queue to store items
	/// Dequeues items in priority order and processes
	/// them in a worker thread one at a time
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class ThreadedWorkerQueue<T>
	{
		private readonly PriorityQueue<T> m_Queue;
		private readonly SafeCriticalSection m_QueueSection;
		private readonly SafeCriticalSection m_ProcessSection;
		private readonly Action<T> m_ProcessAction;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="processItemAction">Action to process the dequeued items</param>
		public ThreadedWorkerQueue([NotNull] Action<T> processItemAction)
		{
			if (processItemAction == null)
				throw new ArgumentNullException("processItemAction");

			m_Queue = new PriorityQueue<T>();
			m_QueueSection = new SafeCriticalSection();
			m_ProcessSection = new SafeCriticalSection();

			m_ProcessAction = processItemAction;
		}

		#region Queue Methods

		/// <summary>
		/// Clears the collection.
		/// </summary>
		[PublicAPI]
		public void Clear()
		{
			m_QueueSection.Execute(() => m_Queue.Clear());
		}

		/// <summary>
		/// Adds the item to the end of the queue.
		/// </summary>
		/// <param name="item"></param>
		[PublicAPI]
		public void Enqueue([CanBeNull] T item)
		{
			Enqueue(() => m_Queue.Enqueue(item));
		}

		/// <summary>
		/// Adds the item to the queue with the given priority.
		/// Lower values are dequeued first.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="priority"></param>
		[PublicAPI]
		public void Enqueue([CanBeNull] T item, int priority)
		{
			Enqueue(() => m_Queue.Enqueue(item, priority));
		}

		/// <summary>
		/// Adds the item to the queue with the given priority at the given index.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="priority"></param>
		/// <param name="position"></param>
		[PublicAPI]
		public void Enqueue([CanBeNull] T item, int priority, int position)
		{
			Enqueue(() => m_Queue.Enqueue(item, priority, position));
		}

		/// <summary>
		/// Enqueues the item at the beginning of the queue.
		/// </summary>
		/// <param name="item"></param>
		[PublicAPI]
		public void EnqueueFirst([CanBeNull] T item)
		{
			Enqueue(() => m_Queue.EnqueueFirst(item));
		}

		/// <summary>
		/// Removes any items in the queue matching the predicate.
		/// Appends the given item at the end of the given priority level.
		/// This is useful for reducing duplication, or replacing items with something more pertinent.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="remove"></param>
		[PublicAPI]
		public void EnqueueRemove([CanBeNull] T item, [NotNull] Func<T, bool> remove)
		{
			Enqueue(() => m_Queue.EnqueueRemove(item, remove));
		}

		/// <summary>
		/// Removes any items in the queue matching the predicate.
		/// Appends the given item at the end of the given priority level.
		/// This is useful for reducing duplication, or replacing items with something more pertinent.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="remove"></param>
		/// <param name="priority"></param>
		[PublicAPI]
		public void EnqueueRemove([CanBeNull] T item, [NotNull] Func<T, bool> remove, int priority)
		{
			Enqueue(() => m_Queue.EnqueueRemove(item, remove, priority));
		}

		/// <summary>
		/// Removes any items in the queue matching the predicate.
		/// Appends the given item at the end of the given priority level.
		/// This is useful for reducing duplication, or replacing items with something more pertinent.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="remove"></param>
		/// <param name="priority"></param>
		/// <param name="deDuplicateToEndOfQueue"></param>
		[PublicAPI]
		public void EnqueueRemove([CanBeNull] T item, [NotNull] Func<T, bool> remove, int priority, bool deDuplicateToEndOfQueue)
		{
			Enqueue(() => m_Queue.EnqueueRemove(item, remove, priority, deDuplicateToEndOfQueue));
		}

		#endregion

		#region Private Methods

		private void Enqueue(Action enqueueAction)
		{
			m_QueueSection.Execute(enqueueAction);
			ThreadingUtils.SafeInvoke(ProcessQueue);
		}

		private void ProcessQueue()
		{
			if (!m_ProcessSection.TryEnter())
				return;

			try
			{
				T item = default(T);
				while (m_QueueSection.Execute(() => m_Queue.TryDequeue(out item)))
					m_ProcessAction(item);
			}
			finally
			{
				m_ProcessSection.Leave();
			}
		}

		#endregion
	}
}