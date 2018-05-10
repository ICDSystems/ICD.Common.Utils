using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils.Collections
{
	/// <summary>
	/// Provides a first-in first-out collection with enhanced insertion features.
	/// </summary>
	public sealed class PriorityQueue<T> : IEnumerable<T>, ICollection
	{
		private readonly IcdOrderedDictionary<int, List<T>> m_PriorityToQueue;
		private int m_Count;

		#region Properties

		/// <summary>
		/// Gets the number of items in the collection.
		/// </summary>
		public int Count { get { return m_Count; } }

		/// <summary>
		/// Is the collection thread safe?
		/// </summary>
		public bool IsSynchronized { get { return false; } }

		/// <summary>
		/// Gets a reference for locking.
		/// </summary>
		public object SyncRoot { get { return this; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public PriorityQueue()
		{
			m_PriorityToQueue = new IcdOrderedDictionary<int, List<T>>();
		}

		#region Methods

		/// <summary>
		/// Clears the collection.
		/// </summary>
		[PublicAPI]
		public void Clear()
		{
			m_PriorityToQueue.Clear();
			m_Count = 0;
		}

		/// <summary>
		/// Adds the item to the end of the queue.
		/// </summary>
		/// <param name="item"></param>
		[PublicAPI]
		public void Enqueue(T item)
		{
			Enqueue(item, int.MaxValue);
		}

		/// <summary>
		/// Adds the item to the queue with the given priority.
		/// Lower values are dequeued first.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="priority"></param>
		[PublicAPI]
		public void Enqueue(T item, int priority)
		{
			if (!m_PriorityToQueue.ContainsKey(priority))
				m_PriorityToQueue.Add(priority, new List<T>());

			m_PriorityToQueue[priority].Add(item);
			m_Count++;
		}

		/// <summary>
		/// Dequeues the first item with the lowest priority value.
		/// </summary>
		/// <returns></returns>
		[PublicAPI]
		public T Dequeue()
		{
			KeyValuePair<int, List<T>> kvp;
			if (!m_PriorityToQueue.TryFirst(out kvp))
				throw new InvalidOperationException("The queue is empty.");

			int priority = kvp.Key;
			List<T> queue = kvp.Value;

			T output = queue[0];
			queue.RemoveAt(0);

			if (queue.Count == 0)
				m_PriorityToQueue.Remove(priority);

			m_Count--;

			return output;
		}

		/// <summary>
		/// Gets an enumerator for the items.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<T> GetEnumerator()
		{
			return m_PriorityToQueue.Values
			                        .SelectMany(v => v)
			                        .GetEnumerator();
		}

		/// <summary>
		/// Copies the collection to the target array.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(Array array, int index)
		{
			foreach (T item in this)
			{
				array.SetValue(item, index);
				index++;
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets an enumerator for the items.
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
