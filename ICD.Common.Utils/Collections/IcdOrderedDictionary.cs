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
	public sealed class IcdOrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private readonly List<TKey> m_OrderedKeys;
		private readonly List<TValue> m_ValuesOrderedByKey; 
		private readonly Dictionary<TKey, TValue> m_Dictionary;
		private readonly IComparer<TKey> m_Comparer; 

		#region Properties

		public int Count { get { return m_Dictionary.Count; } }

		public bool IsReadOnly { get { return false; } }

		[NotNull]
		public ICollection<TKey> Keys { get { return m_OrderedKeys; } }

		[NotNull]
		public ICollection<TValue> Values { get { return m_ValuesOrderedByKey; } }

		[CanBeNull]
		public TValue this[[NotNull] TKey key]
		{
			get { return m_Dictionary[key]; }
			set
			{
// ReSharper disable once CompareNonConstrainedGenericWithNull
				if (key == null)
					throw new ArgumentNullException("key");

				Remove(key);
				Add(key, value);
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
		public IcdOrderedDictionary([NotNull] IComparer<TKey> comparer)
			: this(comparer, EqualityComparer<TKey>.Default)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="comparer"></param>
		/// <param name="equalityComparer"></param>
		public IcdOrderedDictionary([NotNull] IComparer<TKey> comparer, [NotNull] IEqualityComparer<TKey> equalityComparer)
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

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dictionary"></param>
		public IcdOrderedDictionary([NotNull] IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
			: this()
		{
			if (dictionary == null)
				throw new ArgumentNullException("dictionary");

			foreach (KeyValuePair<TKey, TValue> kvp in dictionary)
				Add(kvp.Key, kvp.Value);
		}

		#region Methods

		[NotNull]
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return m_OrderedKeys.Select(k => new KeyValuePair<TKey, TValue>(k, m_Dictionary[k]))
			                    .GetEnumerator();
		}

		public void Add([NotNull] TKey key, [CanBeNull] TValue value)
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
			if (key == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("key");

			if (m_Dictionary.ContainsKey(key))
				throw new ArgumentOutOfRangeException("key", "An item with the same key has already been added.");

			int index = m_OrderedKeys.InsertSorted(key, m_Comparer);
			m_ValuesOrderedByKey.Insert(index, value);

			m_Dictionary[key] = value;
		}

		public void Clear()
		{
			m_OrderedKeys.Clear();
			m_ValuesOrderedByKey.Clear();
			m_Dictionary.Clear();
		}

		public bool ContainsKey([NotNull] TKey key)
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
			if (key == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("key");

			return m_Dictionary.ContainsKey(key);
		}

		public bool Remove([NotNull] TKey key)
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
			if (key == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("key");

			if (!m_Dictionary.Remove(key))
				return false;

			int index = m_OrderedKeys.BinarySearch(key, m_Comparer);

			m_OrderedKeys.RemoveAt(index);
			m_ValuesOrderedByKey.RemoveAt(index);

			return true;
		}

		public bool TryGetValue([NotNull] TKey key, out TValue value)
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
			if (key == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("key");

			return m_Dictionary.TryGetValue(key, out value);
		}

		#endregion

		#region Private Methods

		[NotNull]
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

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo([NotNull] KeyValuePair<TKey, TValue>[] array, int index)
		{
			if (array == null)
				throw new ArgumentNullException("array");

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
