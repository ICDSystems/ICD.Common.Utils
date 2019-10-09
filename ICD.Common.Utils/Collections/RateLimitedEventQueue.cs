using System;
using System.Collections;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;

namespace ICD.Common.Utils.Collections
{
	/// <summary>
	/// RateLimitedEventQueue provides features for enqueing items to be raised via an event at a controlled interval.
	/// </summary>
	public sealed class RateLimitedEventQueue<T> : IEnumerable<T>, ICollection, IDisposable
	{
		/// <summary>
		/// Raised to handle to the next item in the queue.
		/// </summary>
		public event EventHandler<GenericEventArgs<T>> OnItemDequeued;

		private readonly SafeTimer m_DequeueTimer;
		private readonly Queue<T> m_Queue;
		private readonly SafeCriticalSection m_QueueSection;

		#region Properties

		/// <summary>
		/// Gets/sets the time between dequeues in milliseconds.
		/// </summary>
		public long BetweenMilliseconds { get; set; }

		public int Count { get { return m_QueueSection.Execute(() => m_Queue.Count); } }

		bool ICollection.IsSynchronized { get { return true; } }

		[NotNull]
		object ICollection.SyncRoot { get { return this; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public RateLimitedEventQueue()
		{
			m_Queue = new Queue<T>();
			m_QueueSection = new SafeCriticalSection();

			m_DequeueTimer = SafeTimer.Stopped(DequeueTimerCallback);
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			OnItemDequeued = null;

			m_QueueSection.Enter();

			try
			{
				m_DequeueTimer.Dispose();
			}
			finally
			{
				m_QueueSection.Leave();
			}
		}

		/// <summary>
		/// Enqueues the given item.
		/// </summary>
		/// <param name="item"></param>
		public void Enqueue([CanBeNull] T item)
		{
			m_QueueSection.Enter();

			try
			{
				m_Queue.Enqueue(item);

				if (m_Queue.Count == 1)
					SendNext();
			}
			finally
			{
				m_QueueSection.Leave();
			}
		}

		/// <summary>
		/// Clears the queued items.
		/// </summary>
		public void Clear()
		{
			m_QueueSection.Enter();

			try
			{
				m_DequeueTimer.Stop();
				m_Queue.Clear();
			}
			finally
			{
				m_QueueSection.Leave();
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Sends the next pulse in the queue.
		/// </summary>
		private void SendNext()
		{
			if (!m_QueueSection.TryEnter())
				return;

			try
			{
				if (m_Queue.Count == 0)
					return;

				T item = m_Queue.Peek();

				OnItemDequeued.Raise(this, new GenericEventArgs<T>(item));

				m_DequeueTimer.Reset(BetweenMilliseconds);
			}
			finally
			{
				m_QueueSection.Leave();
			}
		}

		/// <summary>
		/// Called when the dequeue timer elapses.
		/// </summary>
		private void DequeueTimerCallback()
		{
			m_QueueSection.Enter();

			try
			{
				m_Queue.Dequeue();
				SendNext();
			}
			finally
			{
				m_QueueSection.Leave();
			}
		}

		#endregion

		#region IEnumerable/ICollection

		[NotNull]
		public IEnumerator<T> GetEnumerator()
		{
			return m_QueueSection.Execute(() => m_Queue.ToList(m_Queue.Count).GetEnumerator());
		}

		[NotNull]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		void ICollection.CopyTo([NotNull] Array array, int index)
		{
			if (array == null)
				throw new ArgumentNullException("array");

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
