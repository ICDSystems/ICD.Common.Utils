using System;
using System.Collections;
using System.Collections.Generic;
using ICD.Common.Properties;

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

		[NotNull]
		public ICollection<TKey> Keys { get { return m_KeyToValue.Keys; } }

		[NotNull]
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

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dict"></param>
		public BiDictionary([NotNull] Dictionary<TKey, TValue> dict)
			: this()
		{
			if (dict == null)
				throw new ArgumentNullException("dict");

			foreach (KeyValuePair<TKey, TValue> kvp in dict)
				Add(kvp.Key, kvp.Value);
		}

		#region Methods

		public void Clear()
		{
			m_KeyToValue.Clear();
			m_ValueToKey.Clear();
		}

		public bool ContainsKey([NotNull] TKey key)
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
			if (key == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("key");

			return m_KeyToValue.ContainsKey(key);
		}

		public bool ContainsValue([NotNull] TValue value)
		{
			return m_ValueToKey.ContainsKey(value);
		}

		public void Add([NotNull] TKey key, [NotNull] TValue value)
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
			if (key == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("key");

// ReSharper disable CompareNonConstrainedGenericWithNull
			if (value == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("value");

			if (ContainsKey(key))
				throw new ArgumentException("Key is already present in the collection", "key");

			if (ContainsValue(value))
				throw new ArgumentException("Value is already present in the collection", "value");

			m_KeyToValue.Add(key, value);
			m_ValueToKey.Add(value, key);
		}

		public void Set([NotNull] TKey key, [NotNull] TValue value)
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
			if (key == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("key");

// ReSharper disable CompareNonConstrainedGenericWithNull
			if (value == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("value");

			RemoveKey(key);
			RemoveValue(value);

			Add(key, value);
		}

		[NotNull]
		public TKey GetKey([NotNull] TValue value)
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
			if (value == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("value");

			return m_ValueToKey[value];
		}

		[NotNull]
		public TValue GetValue([NotNull] TKey key)
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
			if (key == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("key");

			return m_KeyToValue[key];
		}

		public bool RemoveKey([NotNull] TKey key)
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
			if (key == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("key");

			if (!ContainsKey(key))
				return false;

			TValue value = m_KeyToValue[key];

			m_KeyToValue.Remove(key);
			m_ValueToKey.Remove(value);

			return true;
		}

		public bool RemoveValue([NotNull] TValue value)
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
			if (value == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("value");

			if (!ContainsValue(value))
				return false;

			TKey key = m_ValueToKey[value];

			return RemoveKey(key);
		}

		public bool TryGetValue([NotNull] TKey key, out TValue value)
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
			if (key == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("key");

			return m_KeyToValue.TryGetValue(key, out value);
		}

		public bool TryGetKey([NotNull] TValue value, out TKey key)
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
			if (value == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("value");

			return m_ValueToKey.TryGetValue(value, out key);
		}

		#endregion

		#region IDictionary

		[NotNull]
		TValue IDictionary<TKey, TValue>.this[[NotNull] TKey key] { get { return GetValue(key); } set { Set(key, value); } }

		bool IDictionary<TKey, TValue>.Remove([NotNull] TKey key)
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

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo([NotNull] KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			(m_KeyToValue as IDictionary<TKey, TValue>).CopyTo(array, arrayIndex);
		}

		#endregion

		#region IEnumerable

		[NotNull]
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return m_KeyToValue.GetEnumerator();
		}

		[NotNull]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
