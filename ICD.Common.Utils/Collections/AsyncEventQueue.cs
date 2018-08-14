using System;
using System.Collections;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils.Collections
{
	/// <summary>
	/// Provides a buffer for storing items to be raised in order in a new thread.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class AsyncEventQueue<T> : IEnumerable<T>, ICollection, IDisposable
	{
		/// <summary>
		/// Raised to handle to the next item in the queue.
		/// </summary>
		public event EventHandler<GenericEventArgs<T>> OnItemDequeued;

		private readonly Queue<T> m_Queue;

		private readonly SafeCriticalSection m_QueueSection;
		private readonly SafeCriticalSection m_ProcessSection;

		#region Properties

		public int Count { get { return m_QueueSection.Execute(() => m_Queue.Count); } }

		bool ICollection.IsSynchronized { get { return true; } }

		object ICollection.SyncRoot { get { return this; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public AsyncEventQueue()
		{
			m_Queue = new Queue<T>();

			m_QueueSection = new SafeCriticalSection();
			m_ProcessSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			OnItemDequeued = null;

			Clear();
		}

		/// <summary>
		/// Enqueues the given item and begins processing the queue.
		/// </summary>
		/// <param name="item"></param>
		public void Enqueue(T item)
		{
			m_QueueSection.Execute(() => m_Queue.Enqueue(item));

			ThreadingUtils.SafeInvoke(ProcessQueue);
		}

		/// <summary>
		/// Clears the queued items.
		/// </summary>
		public void Clear()
		{
			m_QueueSection.Execute(() => m_Queue.Clear());
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Dequeues and raises each item in sequence.
		/// </summary>
		private void ProcessQueue()
		{
			if (!m_ProcessSection.TryEnter())
				return;

			try
			{
				T item;
				while (m_Queue.Dequeue(out item))
					OnItemDequeued.Raise(this, new GenericEventArgs<T>(item));
			}
			finally
			{
				m_ProcessSection.Leave();
			}
		}

		#endregion

		#region IEnumerable/ICollection

		public IEnumerator<T> GetEnumerator()
		{
			return m_QueueSection.Execute(() => m_Queue.ToList(m_Queue.Count).GetEnumerator());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			m_QueueSection.Enter();

			try
			{
				foreach (T item in this)
				{
					array.SetValue(item, index);
					index++;
				}
			}
			finally
			{
				m_QueueSection.Leave();
			}
		}

		#endregion
	}
}