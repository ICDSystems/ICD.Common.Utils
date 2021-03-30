using System;
using System.Collections;
using System.Collections.Generic;

namespace ICD.Common.Utils.Collections
{
	public sealed class IcdOrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
	{
		private readonly IcdOrderedDictionary m_PrivateDictionary;

		public int Count { get { return m_PrivateDictionary.Count; } }

		int ICollection.Count { get { return m_PrivateDictionary.Count; } }

		public bool IsReadOnly { get { return false; } }

		public TValue this[TKey key]
		{
			get
			{
				if (key == null)
					throw new ArgumentNullException("key");

				if (m_PrivateDictionary.Contains(key))
					return (TValue)m_PrivateDictionary[key];

				throw new KeyNotFoundException();
			}
			set
			{
				if (key == null)
					throw new ArgumentNullException("key");

				m_PrivateDictionary[key] = value;
			}
		}

		object IDictionary.this[object key]
		{
			get { return m_PrivateDictionary[key]; }
			set { m_PrivateDictionary[key] = value; }
		}

		public ICollection<TKey> Keys
		{
			get
			{
				List<TKey> keys = new List<TKey>(m_PrivateDictionary.Count);

				foreach (TKey key in m_PrivateDictionary.Keys)
				{
					keys.Add(key);
				}

				// Keys should be put in a ReadOnlyCollection,
				// but since this is an internal class, for performance reasons,
				// we choose to avoid creating yet another collection.

				return keys;
			}
		}

		ICollection IDictionary.Keys { get { return m_PrivateDictionary.Keys; } }

		public ICollection<TValue> Values
		{
			get
			{
				List<TValue> values = new List<TValue>(m_PrivateDictionary.Count);

				foreach (TValue value in m_PrivateDictionary.Values)
					values.Add(value);

				// Values should be put in a ReadOnlyCollection,
				// but since this is an internal class, for performance reasons,
				// we choose to avoid creating yet another collection.

				return values;
			}
		}

		ICollection IDictionary.Values { get { return m_PrivateDictionary.Values; } }

		bool IDictionary.IsFixedSize { get { return ((IDictionary)m_PrivateDictionary).IsFixedSize; } }

		bool IDictionary.IsReadOnly { get { return m_PrivateDictionary.IsReadOnly; } }

		bool ICollection.IsSynchronized { get { return ((ICollection)m_PrivateDictionary).IsSynchronized; } }

		object ICollection.SyncRoot { get { return m_PrivateDictionary; } }

		public IcdOrderedDictionary()
		{
			m_PrivateDictionary = new IcdOrderedDictionary();
		}

		public IcdOrderedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
		{
			if (dictionary == null)
				return;

			m_PrivateDictionary = new IcdOrderedDictionary();

			foreach (KeyValuePair<TKey, TValue> pair in dictionary)
			{
				m_PrivateDictionary.Add(pair.Key, pair.Value);
			}
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		public void Add(TKey key, TValue value)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			m_PrivateDictionary.Add(key, value);
		}

		public KeyValuePair<TKey, TValue> Get(int index)
		{
			DictionaryEntry entry = (DictionaryEntry)m_PrivateDictionary.ObjectsArray[index];
			return new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value);
		}

		public void Clear()
		{
			m_PrivateDictionary.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			if (item.Key == null || !m_PrivateDictionary.Contains(item.Key))
				return false;

			return m_PrivateDictionary[item.Key].Equals(item.Value);
		}

		public bool ContainsKey(TKey key)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			return m_PrivateDictionary.Contains(key);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}

			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}

			if (array.Rank > 1 || arrayIndex >= array.Length || array.Length - arrayIndex < m_PrivateDictionary.Count)
			{
				throw new ArgumentException("array");
			}

			int index = arrayIndex;
			foreach (DictionaryEntry entry in m_PrivateDictionary)
			{
				array[index] = new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value);
				index++;
			}
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			foreach (DictionaryEntry entry in m_PrivateDictionary)
				yield return new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			if (Contains(item))
			{
				m_PrivateDictionary.Remove(item.Key);
				return true;
			}
			return false;
		}

		public bool Remove(TKey key)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			if (m_PrivateDictionary.Contains(key))
			{
				m_PrivateDictionary.Remove(key);
				return true;
			}
			return false;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			bool keyExists = m_PrivateDictionary.Contains(key);
			value = keyExists ? (TValue)m_PrivateDictionary[key] : default(TValue);

			return keyExists;
		}

		void IDictionary.Add(object key, object value)
		{
			m_PrivateDictionary.Add(key, value);
		}

		void IDictionary.Clear()
		{
			m_PrivateDictionary.Clear();
		}

		bool IDictionary.Contains(object key)
		{
			return m_PrivateDictionary.Contains(key);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return m_PrivateDictionary.GetEnumerator();
		}

		void IDictionary.Remove(object key)
		{
			m_PrivateDictionary.Remove(key);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			m_PrivateDictionary.CopyTo(array, index);
		}
	}

	internal sealed class IcdOrderedDictionary : IDictionary
	{
		private ArrayList m_ObjectsArray;
		private Hashtable m_ObjectsTable;
		private readonly int m_InitialCapacity;
		private readonly IEqualityComparer m_Comparer;
		private readonly bool m_ReadOnly;

		/// <summary>
		/// Gets the size of the table.
		/// </summary>
		public int Count { get { return ObjectsArray.Count; } }

		/// <summary>
		/// Indicates that the collection can grow.
		/// </summary>
		bool IDictionary.IsFixedSize { get { return m_ReadOnly; } }

		/// <summary>
		/// Indicates that the collection is not read-only
		/// </summary>
		public bool IsReadOnly { get { return m_ReadOnly; } }

		/// <summary>
		/// Indicates that this class is not synchronized
		/// </summary>
		bool ICollection.IsSynchronized { get { return false; } }

		/// <summary>
		/// Gets the collection of keys in the table in order.
		/// </summary>
		public ICollection Keys { get { return new OrderedDictionaryKeyValueCollection(ObjectsArray, true); } }

		/// <summary>
		/// Returns an arrayList of the values in the table
		/// </summary>
		public ICollection Values { get { return new OrderedDictionaryKeyValueCollection(ObjectsArray, false); } }

		public ArrayList ObjectsArray
		{
			get { return m_ObjectsArray ?? (m_ObjectsArray = new ArrayList(m_InitialCapacity)); }
		}

		private Hashtable ObjectsTable
		{
			get { return m_ObjectsTable ?? (m_ObjectsTable = new Hashtable(m_InitialCapacity, m_Comparer)); }
		}

		/// <summary>
		/// The SyncRoot object.  Not used because IsSynchronized is false
		/// </summary>
		object ICollection.SyncRoot { get { return this; } }

		/// <summary>
		/// Gets or sets the object at the specified index
		/// </summary>
		public object this[int index]
		{
			get { return ((DictionaryEntry)ObjectsArray[index]).Value; }
			set
			{
				if (m_ReadOnly)
				{
					throw new NotSupportedException();
				}
				if (index < 0 || index >= ObjectsArray.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				object key = ((DictionaryEntry)ObjectsArray[index]).Key;
				ObjectsArray[index] = new DictionaryEntry(key, value);
				ObjectsTable[key] = value;
			}
		}

		/// <summary>
		/// Gets or sets the object with the specified key
		/// </summary>
		public object this[object key]
		{
			get { return ObjectsTable[key]; }
			set
			{
				if (m_ReadOnly)
				{
					throw new NotSupportedException();
				}
				if (ObjectsTable.Contains(key))
				{
					ObjectsTable[key] = value;
					ObjectsArray[IndexOfKey(key)] = new DictionaryEntry(key, value);
				}
				else
				{
					Add(key, value);
				}
			}
		}

		public IcdOrderedDictionary() : this(0)
		{
		}

		public IcdOrderedDictionary(int capacity) : this(capacity, null)
		{
		}

		public IcdOrderedDictionary(int capacity, IEqualityComparer comparer)
		{
			m_InitialCapacity = capacity;
			m_Comparer = comparer;
		}

		private IcdOrderedDictionary(IcdOrderedDictionary dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}

			m_ReadOnly = true;
			m_ObjectsArray = dictionary.m_ObjectsArray;
			m_ObjectsTable = dictionary.m_ObjectsTable;
			m_Comparer = dictionary.m_Comparer;
			m_InitialCapacity = dictionary.m_InitialCapacity;
		}

		/// <summary>
		/// Adds a new entry to the table with the lowest-available index.
		/// </summary>
		public void Add(object key, object value)
		{
			if (m_ReadOnly)
			{
				throw new NotSupportedException();
			}
			ObjectsTable.Add(key, value);
			ObjectsArray.Add(new DictionaryEntry(key, value));
		}

		/// <summary>
		/// Clears all elements in the table.
		/// </summary>
		public void Clear()
		{
			if (m_ReadOnly)
			{
				throw new NotSupportedException();
			}
			ObjectsTable.Clear();
			ObjectsArray.Clear();
		}

		/// <summary>
		/// Returns a readonly IcdOrderedDictionary for the given IcdOrderedDictionary.
		/// </summary>
		public IcdOrderedDictionary AsReadOnly()
		{
			return new IcdOrderedDictionary(this);
		}

		/// <summary>
		/// Returns true if the key exists in the table, false otherwise.
		/// </summary>
		public bool Contains(object key)
		{
			return ObjectsTable.Contains(key);
		}

		/// <summary>
		/// Copies the table to an array.  This will not preserve order.
		/// </summary>
		public void CopyTo(Array array, int index)
		{
			ObjectsTable.CopyTo(array, index);
		}

		private int IndexOfKey(object key)
		{
			for (int i = 0; i < ObjectsArray.Count; i++)
			{
				object o = ((DictionaryEntry)ObjectsArray[i]).Key;
				if (m_Comparer != null)
				{
					if (m_Comparer.Equals(o, key))
					{
						return i;
					}
				}
				else
				{
					if (o.Equals(key))
					{
						return i;
					}
				}
			}
			return -1;
		}

		/// <summary>
		/// Inserts a new object at the given index with the given key.
		/// </summary>
		public void Insert(int index, object key, object value)
		{
			if (m_ReadOnly)
			{
				throw new NotSupportedException();
			}
			if (index > Count || index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			ObjectsTable.Add(key, value);
			ObjectsArray.Insert(index, new DictionaryEntry(key, value));
		}

		/// <summary>
		/// Removes the entry at the given index.
		/// </summary>
		public void RemoveAt(int index)
		{
			if (m_ReadOnly)
			{
				throw new NotSupportedException();
			}
			if (index >= Count || index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			object key = ((DictionaryEntry)ObjectsArray[index]).Key;
			ObjectsArray.RemoveAt(index);
			ObjectsTable.Remove(key);
		}

		/// <summary>
		/// Removes the entry with the given key.
		/// </summary>
		public void Remove(object key)
		{
			if (m_ReadOnly)
			{
				throw new NotSupportedException();
			}
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}

			int index = IndexOfKey(key);
			if (index < 0)
			{
				return;
			}

			ObjectsTable.Remove(key);
			ObjectsArray.RemoveAt(index);
		}

		#region IDictionary implementation

		/// <internalonly/>
		public IDictionaryEnumerator GetEnumerator()
		{
			return new OrderedDictionaryEnumerator(ObjectsArray, OrderedDictionaryEnumerator.DICTIONARY_ENTRY);
		}

		#endregion

		#region IEnumerable implementation

		/// <internalonly/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new OrderedDictionaryEnumerator(ObjectsArray, OrderedDictionaryEnumerator.DICTIONARY_ENTRY);
		}

		#endregion

		/// <summary>
		/// OrderedDictionaryEnumerator works just like any other IDictionaryEnumerator, but it retrieves DictionaryEntries
		/// in the order by index.
		/// </summary>
		private sealed class OrderedDictionaryEnumerator : IDictionaryEnumerator
		{
			internal const int KEYS = 1;
			internal const int VALUES = 2;
			internal const int DICTIONARY_ENTRY = 3;

			private readonly int m_ObjectReturnType;
			private readonly IEnumerator m_ArrayEnumerator;

			internal OrderedDictionaryEnumerator(ArrayList array, int objectReturnType)
			{
				m_ArrayEnumerator = array.GetEnumerator();
				m_ObjectReturnType = objectReturnType;
			}

			/// <summary>
			/// Retrieves the current DictionaryEntry.  This is the same as Entry, but not strongly-typed.
			/// </summary>
			public object Current
			{
				get
				{
					if (m_ObjectReturnType == KEYS)
					{
						return ((DictionaryEntry)m_ArrayEnumerator.Current).Key;
					}
					if (m_ObjectReturnType == VALUES)
					{
						return ((DictionaryEntry)m_ArrayEnumerator.Current).Value;
					}
					return Entry;
				}
			}

			/// <summary>
			/// Retrieves the current DictionaryEntry
			/// </summary>
			public DictionaryEntry Entry
			{
				get
				{
					return new DictionaryEntry(((DictionaryEntry)m_ArrayEnumerator.Current).Key,
					                           ((DictionaryEntry)m_ArrayEnumerator.Current).Value);
				}
			}

			/// <summary>
			/// Retrieves the key of the current DictionaryEntry
			/// </summary>
			public object Key { get { return ((DictionaryEntry)m_ArrayEnumerator.Current).Key; } }

			/// <summary>
			/// Retrieves the value of the current DictionaryEntry
			/// </summary>
			public object Value { get { return ((DictionaryEntry)m_ArrayEnumerator.Current).Value; } }

			/// <summary>
			/// Moves the enumerator pointer to the next member
			/// </summary>
			public bool MoveNext()
			{
				return m_ArrayEnumerator.MoveNext();
			}

			/// <summary>
			/// Resets the enumerator pointer to the beginning.
			/// </summary>
			public void Reset()
			{
				m_ArrayEnumerator.Reset();
			}
		}

		/// <summary>
		/// OrderedDictionaryKeyValueCollection implements a collection for the Values and Keys properties
		/// that is "live"- it will reflect changes to the IcdOrderedDictionary on the collection made after the getter
		/// was called.
		/// </summary>
		private sealed class OrderedDictionaryKeyValueCollection : ICollection
		{
			private readonly ArrayList m_Objects;
			private readonly bool m_IsKeys;

			public OrderedDictionaryKeyValueCollection(ArrayList array, bool isKeys)
			{
				m_Objects = array;
				m_IsKeys = isKeys;
			}

			void ICollection.CopyTo(Array array, int index)
			{
				if (array == null)
					throw new ArgumentNullException("array");
				if (index < 0)
					throw new ArgumentOutOfRangeException("index");
				foreach (object o in m_Objects)
				{
					array.SetValue(m_IsKeys ? ((DictionaryEntry)o).Key : ((DictionaryEntry)o).Value, index);
					index++;
				}
			}

			int ICollection.Count { get { return m_Objects.Count; } }

			bool ICollection.IsSynchronized { get { return false; } }

			object ICollection.SyncRoot { get { return m_Objects.SyncRoot; } }

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new OrderedDictionaryEnumerator(m_Objects,
				                                       m_IsKeys
					                                       ? OrderedDictionaryEnumerator.KEYS
					                                       : OrderedDictionaryEnumerator.VALUES);
			}
		}
	}
}
