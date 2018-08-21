using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils.Collections
{
	public sealed class IcdOrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private readonly List<TKey> m_OrderedKeys;
		private readonly List<TValue> m_ValuesOrderedByKey; 
		private readonly Dictionary<TKey, TValue> m_Dictionary;
		private readonly IComparer<TKey> m_Comparer; 

		#region Properties

		public int Count { get { return m_Dictionary.Count; } }

		public bool IsReadOnly { get { return false; } }

		public ICollection<TKey> Keys { get { return m_OrderedKeys; } }

		public ICollection<TValue> Values { get { return m_ValuesOrderedByKey; } }

		public TValue this[TKey key]
		{
			get { return m_Dictionary[key]; }
			set
			{
// ReSharper disable once CompareNonConstrainedGenericWithNull
				if (key == null)
					throw new ArgumentNullException("key");

				if (!ContainsKey(key))
				{
					int index = m_OrderedKeys.AddSorted(key, m_Comparer);
					m_ValuesOrderedByKey.Insert(index, value);
				}

				m_Dictionary[key] = value;
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public IcdOrderedDictionary()
			: this(Comparer<TKey>.Default)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="comparer"></param>
		public IcdOrderedDictionary(IComparer<TKey> comparer)
			: this(comparer, EqualityComparer<TKey>.Default)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="comparer"></param>
		/// <param name="equalityComparer"></param>
		public IcdOrderedDictionary(IComparer<TKey> comparer, IEqualityComparer<TKey> equalityComparer)
		{
			if (comparer == null)
				throw new ArgumentNullException("comparer");

			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");

			m_Comparer = comparer;
			m_OrderedKeys = new List<TKey>();
			m_ValuesOrderedByKey = new List<TValue>();
			m_Dictionary = new Dictionary<TKey, TValue>(equalityComparer);
		}

		#region Methods

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return m_OrderedKeys.Select(k => new KeyValuePair<TKey, TValue>(k, m_Dictionary[k]))
			                    .GetEnumerator();
		}

		public void Add(TKey key, TValue value)
		{
// ReSharper disable once CompareNonConstrainedGenericWithNull
			if (key == null)
				throw new ArgumentNullException("key");

			if (m_Dictionary.ContainsKey(key))
				throw new ArgumentException("An item with the same key has already been added.", "key");

			this[key] = value;
		}

		public void Clear()
		{
			m_OrderedKeys.Clear();
			m_ValuesOrderedByKey.Clear();
			m_Dictionary.Clear();
		}

		public bool ContainsKey(TKey key)
		{
			return m_Dictionary.ContainsKey(key);
		}

		public bool Remove(TKey key)
		{
// ReSharper disable once CompareNonConstrainedGenericWithNull
			if (key == null)
				throw new ArgumentNullException("key");

			if (!m_Dictionary.Remove(key))
				return false;

			int index = m_OrderedKeys.BinarySearch(key, m_Comparer);

			m_OrderedKeys.RemoveAt(index);
			m_ValuesOrderedByKey.RemoveAt(index);

			return true;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return m_Dictionary.TryGetValue(key, out value);
		}

		#endregion

		#region Private Methods

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			TValue value;
			return TryGetValue(item.Key, out value) &&
			       EqualityComparer<TValue>.Default.Equals(value, item.Value);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
		{
			foreach (KeyValuePair<TKey, TValue> kvp in this)
			{
				array.SetValue(kvp, index);
				index++;
			}
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			return (this as ICollection<KeyValuePair<TKey, TValue>>).Contains(item) && Remove(item.Key);
		}

		#endregion
	}
}
