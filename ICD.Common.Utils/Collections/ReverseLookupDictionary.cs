using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
#if !SIMPLSHARP
using System.Diagnostics;
#endif

namespace ICD.Common.Utils.Collections
{
#if !SIMPLSHARP
	[DebuggerDisplay("Count = {Count}")]
#endif
    public class ReverseLookupDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> m_KeyToValue;
		private readonly Dictionary<TValue, IcdHashSet<TKey>> m_ValueToKeys;

		#region Properties

		public int Count { get { return m_KeyToValue.Count; } }

		public bool IsReadOnly { get { return false; } }

		[NotNull]
		public ICollection<TKey> Keys { get { return m_KeyToValue.Keys; } }

		[NotNull]
		public ICollection<TValue> Values { get { return m_ValueToKeys.Keys; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ReverseLookupDictionary() :
			this(EqualityComparer<TKey>.Default, EqualityComparer<TValue>.Default)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="keyComparer"></param>
		/// <param name="valueComparer"></param>
		public ReverseLookupDictionary(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
		{
			m_KeyToValue = new Dictionary<TKey, TValue>(keyComparer);
			m_ValueToKeys = new Dictionary<TValue, IcdHashSet<TKey>>(valueComparer);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dict"></param>
		/// <param name="keyComparer"></param>
		/// <param name="valueComparer"></param>
		public ReverseLookupDictionary([NotNull] Dictionary<TKey, TValue> dict, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
			: this(keyComparer, valueComparer)
		{
			if (dict == null)
				throw new ArgumentNullException("dict");

			foreach (KeyValuePair<TKey, TValue> kvp in dict)
				Add(kvp.Key, kvp.Value);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dict"></param>
		public ReverseLookupDictionary([NotNull] Dictionary<TKey, TValue> dict)
			: this(dict, EqualityComparer<TKey>.Default, EqualityComparer<TValue>.Default)
		{
		}

		#region Methods

		public void Clear()
		{
			m_KeyToValue.Clear();
			m_ValueToKeys.Clear();
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
			return m_ValueToKeys.ContainsKey(value);
		}

		public IEnumerable<KeyValuePair<TValue, IEnumerable<TKey>>> GetReverseDictionary()
		{
			// Cast stuff a layer deep cause weird
		    return m_ValueToKeys.Select(kvp => new KeyValuePair<TValue, IEnumerable<TKey>>(kvp.Key, kvp.Value.ToArray(kvp.Value.Count)));
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


			m_KeyToValue.Add(key, value);
			m_ValueToKeys.GetOrAddNew(value, () => new IcdHashSet<TKey>()).Add(key);
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

			Add(key, value);
		}

		[NotNull]
		public IEnumerable<TKey> GetKeys([NotNull] TValue value)
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
			if (value == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("value");

			return m_ValueToKeys[value];
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
			
			IcdHashSet<TKey> keys = m_ValueToKeys[value];
			keys.Remove(key);

			if (keys.Count == 0)
				m_ValueToKeys.Remove(value);

			return true;
		}

		/// <summary>
		/// Removes the value from the collection, and any keys that were using it
		/// </summary>
		/// <param name="value"></param>
		/// <returns>true if items were removed, false if not</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public bool RemoveValue([NotNull] TValue value)
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
			if (value == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("value");

			if (!ContainsValue(value))
				return false;

			IcdHashSet<TKey> keys = m_ValueToKeys[value];

			m_ValueToKeys.Remove(value);

			foreach (TKey key in keys)
			{
				m_KeyToValue.Remove(key);
			}

			return true;
		}

		public bool TryGetValue([NotNull] TKey key, out TValue value)
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
			if (key == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("key");

			return m_KeyToValue.TryGetValue(key, out value);
		}

		public bool TryGetKeys([NotNull] TValue value, out IEnumerable<TKey> keys)
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
			if (value == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("value");
			
			IcdHashSet<TKey> keysInternal;
			if (m_ValueToKeys.TryGetValue(value, out keysInternal))
			{
				keys = keysInternal;
				return true;
			}
			keys = Enumerable.Empty<TKey>();
			return false;
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