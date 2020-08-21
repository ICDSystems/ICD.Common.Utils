using System;
using System.Collections;
using System.Collections.Generic;
using ICD.Common.Properties;

namespace ICD.Common.Utils.Collections
{
	/// <summary>
	///	ScrollQueue is a queue that never exceeds a predefined length. Old items
	///	are removed as new items are added.
	/// </summary>
	/// <typeparam name="TContents"></typeparam>
	public sealed class ScrollQueue<TContents> : IEnumerable<TContents>, ICollection
	{
		private readonly LinkedList<TContents> m_Collection;
		private int m_MaxSize;

		#region Properties

		/// <summary>
		/// Gets/sets the maximum size of the queue.
		/// </summary>
		[PublicAPI]
		public int MaxSize
		{
			get { return m_MaxSize; }
			set
			{
				if (value == m_MaxSize)
					return;

				m_MaxSize = value;

				TContents unused;
				Trim(out unused);
			}
		}

		/// <summary>
		/// Gets the number of items in the collection.
		/// </summary>
		public int Count { get { return m_Collection.Count; } }

		/// <summary>
		/// The IsSynchronized Boolean property returns True if the 
		/// collection is designed to be thread safe; otherwise, it returns False.
		/// </summary>
		public bool IsSynchronized { get { return false; } }

		/// <summary>
		/// The SyncRoot property returns an object, which is used for synchronizing 
		/// the collection. This returns the instance of the object or returns the 
		/// SyncRoot of other collections if the collection contains other collections.
		/// </summary>
		public object SyncRoot { get { return this; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="maxSize"></param>
		public ScrollQueue(int maxSize)
		{
			m_Collection = new LinkedList<TContents>();
			MaxSize = maxSize;
		}

		#region Queue Methods

		/// <summary>
		/// Clears all items from the queue.
		/// </summary>
		public void Clear()
		{
			m_Collection.Clear();
		}

		/// <summary>
		/// Appends the item to the queue, trims old items that exceed max length.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="removed"></param>
		/// <returns>Returns true if an item was dequeued.</returns>
		[PublicAPI]
		public bool Enqueue(TContents item, out TContents removed)
		{
			m_Collection.AddLast(item);
			return Trim(out removed);
		}

		/// <summary>
		/// Removes the oldest item from the queue.
		/// </summary>
		/// <returns></returns>
		[PublicAPI]
		public TContents Dequeue()
		{
			TContents output = Peek();
			m_Collection.RemoveFirst();
			return output;
		}

		/// <summary>
		/// Returns the oldest item in the queue.
		/// </summary>
		/// <returns></returns>
		[PublicAPI]
		public TContents Peek()
		{
			return m_Collection.First.Value;
		}

		#endregion

		#region Implemented Methods

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<TContents> GetEnumerator()
		{
			return m_Collection.GetEnumerator();
		}

		void ICollection.CopyTo(Array myArr, int index)
		{
			foreach (TContents item in m_Collection)
			{
				myArr.SetValue(item, index);
				index++;
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Removes items that fall outside of the max size.
		/// </summary>
		private bool Trim(out TContents removed)
		{
			removed = default(TContents);

			if (Count <= MaxSize)
				return false;

			removed = m_Collection.First.Value;
			m_Collection.RemoveFirst();

			return true;
		}

		#endregion
	}
}
