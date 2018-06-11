using System;
using System.Collections;
using System.Collections.Generic;

namespace ICD.Common.Utils.Collections
{
	/// <summary>
	/// Provides a 1-to-1 map of Keys to Values with O(1) Value->Key lookup time.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public sealed class BiDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private readonly Dictionary<TKey, TValue> m_KeyToValue;
		private readonly Dictionary<TValue, TKey> m_ValueToKey;

		#region Properties

		public int Count { get { return m_KeyToValue.Count; } }

		public bool IsReadOnly { get { return false; } }

		public ICollection<TKey> Keys { get { return m_KeyToValue.Keys; } }

		public ICollection<TValue> Values { get { return m_ValueToKey.Keys; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public BiDictionary()
		{
			m_KeyToValue = new Dictionary<TKey, TValue>();
			m_ValueToKey = new Dictionary<TValue, TKey>();
		}

		#region Methods

		public void Clear()
		{
			m_KeyToValue.Clear();
			m_ValueToKey.Clear();
		}

		public bool ContainsKey(TKey key)
		{
			return m_KeyToValue.ContainsKey(key);
		}

		public bool ContainsValue(TValue value)
		{
			return m_ValueToKey.ContainsKey(value);
		}

		public void Add(TKey key, TValue value)
		{
// ReSharper disable once CompareNonConstrainedGenericWithNull
			if (key == null)
				throw new ArgumentNullException("key");

// ReSharper disable once CompareNonConstrainedGenericWithNull
			if (value == null)
				throw new ArgumentNullException("value");

			if (ContainsKey(key))
				throw new ArgumentException("Key is already present in the collection", "key");

			if (ContainsValue(value))
				throw new ArgumentException("Value is already present in the collection", "value");

			m_KeyToValue.Add(key, value);
			m_ValueToKey.Add(value, key);
		}

		public void Set(TKey key, TValue value)
		{
// ReSharper disable once CompareNonConstrainedGenericWithNull
			if (key == null)
				throw new ArgumentNullException("key");

// ReSharper disable once CompareNonConstrainedGenericWithNull
			if (value == null)
				throw new ArgumentNullException("value");

			// Prevent building a 2-to-1 mapping
			if (ContainsKey(key) ^ ContainsValue(value))
				throw new InvalidOperationException(
					"Can not set key and value when either key or value are already present in the collection");

			m_KeyToValue[key] = value;
			m_ValueToKey[value] = key;
		}

		public TKey GetKey(TValue value)
		{
			return m_ValueToKey[value];
		}

		public TValue GetValue(TKey key)
		{
			return m_KeyToValue[key];
		}

		public bool RemoveKey(TKey key)
		{
			if (!ContainsKey(key))
				return false;

			TValue value = m_KeyToValue[key];

			m_KeyToValue.Remove(key);
			m_ValueToKey.Remove(value);

			return true;
		}

		public bool RemoveValue(TValue value)
		{
			if (!ContainsValue(value))
				return false;

			TKey key = m_ValueToKey[value];

			return RemoveKey(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return m_KeyToValue.TryGetValue(key, out value);
		}

		public bool TryGetKey(TValue value, out TKey key)
		{
			return m_ValueToKey.TryGetValue(value, out key);
		}

		#endregion

		#region IDictionary

		TValue IDictionary<TKey, TValue>.this[TKey key] { get { return GetValue(key); } set { Set(key, value); } }

		bool IDictionary<TKey, TValue>.Remove(TKey key)
		{
			return RemoveKey(key);
		}

		#endregion

		#region ICollection

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			return (m_KeyToValue as IDictionary<TKey, TValue>).Contains(item);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			return RemoveKey(item.Key);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			(m_KeyToValue as IDictionary<TKey, TValue>).CopyTo(array, arrayIndex);
		}

		#endregion

		#region IEnumerable

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return m_KeyToValue.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
