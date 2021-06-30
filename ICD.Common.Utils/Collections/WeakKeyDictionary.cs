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
	public sealed class WeakKeyReference<T> : WeakReference
	{
		private readonly int m_HashCode;

		public new T Target { get { return (T)base.Target; } }

		public int HashCode { get { return m_HashCode; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="key"></param>
		public WeakKeyReference(T key)
			: this(key, EqualityComparer<T>.Default)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="comparer"></param>
		public WeakKeyReference(T key, IEqualityComparer<T> comparer)
			: base(key)
		{
			if (comparer == null)
				throw new ArgumentNullException("comparer");

			// Retain the object's hash code immediately so that even
			// if the target is GC'ed we will be able to find and
			// remove the dead weak reference.
			m_HashCode = comparer.GetHashCode(key);
		}
	}

	public sealed class WeakKeyComparer<T> : IEqualityComparer<WeakKeyReference<T>>
	{
		private readonly IEqualityComparer<T> m_Comparer;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="comparer"></param>
		public WeakKeyComparer(IEqualityComparer<T> comparer)
		{
			if (comparer == null)
				comparer = EqualityComparer<T>.Default;

			m_Comparer = comparer;
		}

		public int GetHashCode(WeakKeyReference<T> obj)
		{
			return obj == null ? 0 : obj.HashCode;
		}

		// Note: There are actually 9 cases to handle here.
		//
		//  Let Wa = Alive Weak Reference
		//  Let Wd = Dead Weak Reference
		//  Let S  = Strong Reference
		//  
		//  x  | y  | Equals(x,y)
		// -------------------------------------------------
		//  Wa | Wa | comparer.Equals(x.Target, y.Target) 
		//  Wa | Wd | false
		//  Wa | S  | comparer.Equals(x.Target, y)
		//  Wd | Wa | false
		//  Wd | Wd | x == y
		//  Wd | S  | false
		//  S  | Wa | comparer.Equals(x, y.Target)
		//  S  | Wd | false
		//  S  | S  | comparer.Equals(x, y)
		// -------------------------------------------------
		public bool Equals(WeakKeyReference<T> x, WeakKeyReference<T> y)
		{
			bool xIsDead;
			bool yIsDead;

			T first = GetTarget(x, out xIsDead);
			T second = GetTarget(y, out yIsDead);

			if (xIsDead)
				return yIsDead && x == y;

			return !yIsDead && m_Comparer.Equals(first, second);
		}

		private static T GetTarget(WeakKeyReference<T> obj, out bool isDead)
		{
			T target = obj.Target;
			isDead = !obj.IsAlive;
			return target;
		}
	}

	/// <summary>
	/// WeakDictionary keeps weak references to keys and drops key/value pairs once the key is garbage collected.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
#if !SIMPLSHARP
	[DebuggerDisplay("Count = {Count}")]
#endif
	public sealed class WeakKeyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private readonly Dictionary<WeakKeyReference<TKey>, TValue> m_Dictionary;
		private readonly IEqualityComparer<TKey> m_Comparer;

		#region Properties

		public int Count { get { return GetAliveKvps().Count(); } }

		public bool IsReadOnly { get { return false; } }

		public ICollection<TKey> Keys { get { return GetAliveKvps().Select(kvp => kvp.Key).ToArray(); } }

		public ICollection<TValue> Values { get { return GetAliveKvps().Select(kvp => kvp.Value).ToArray(); } }

		public TValue this[TKey key]
		{
			get
			{
				TValue output;
				if (TryGetValue(key, out output))
					return output;

				throw new KeyNotFoundException();
			}
			set
			{
// ReSharper disable once CompareNonConstrainedGenericWithNull
				if (key == null)
					throw new ArgumentNullException("key");

				WeakKeyReference<TKey> weakKey = new WeakKeyReference<TKey>(key, m_Comparer);
				m_Dictionary[weakKey] = value;
			}
		}

		#endregion

		public WeakKeyDictionary()
			: this(0)
		{
		}

		public WeakKeyDictionary(int capacity)
			: this(capacity, EqualityComparer<TKey>.Default)
		{
		}

		public WeakKeyDictionary(IEqualityComparer<TKey> comparer)
			: this(0, comparer)
		{
		}

		public WeakKeyDictionary(int capacity, IEqualityComparer<TKey> comparer)
		{
			m_Comparer = comparer;

			WeakKeyComparer<TKey> keyComparer = new WeakKeyComparer<TKey>(m_Comparer);
			m_Dictionary = new Dictionary<WeakKeyReference<TKey>, TValue>(capacity, keyComparer);
		}

		#region Methods

		public void Add(TKey key, TValue value)
		{
// ReSharper disable once CompareNonConstrainedGenericWithNull
			if (key == null)
				throw new ArgumentNullException("key");

			WeakKeyReference<TKey> weakKey = new WeakKeyReference<TKey>(key, m_Comparer);
			m_Dictionary.Add(weakKey, value);
		}

		public bool ContainsKey(TKey key)
		{
			TValue unused;
			return TryGetValue(key, out unused);
		}

		public bool Remove(TKey key)
		{
			return m_Dictionary.Remove(new WeakKeyReference<TKey>(key, m_Comparer));
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return m_Dictionary.TryGetValue(new WeakKeyReference<TKey>(key, m_Comparer), out value);
		}

		public void Clear()
		{
			m_Dictionary.Clear();
		}

		/// <summary>
		/// Removes the left-over weak references for entries in the dictionary
		/// whose key or value has already been reclaimed by the garbage
		/// collector. This will reduce the dictionary's Count by the number
		/// of dead key-value pairs that were eliminated.
		/// </summary>
		[PublicAPI]
		public void RemoveCollectedEntries()
		{
			IEnumerable<WeakKeyReference<TKey>> toRemove =
				m_Dictionary.Select(pair => pair.Key)
				            .Where(weakKey => !weakKey.IsAlive)
							.ToArray();

			m_Dictionary.RemoveAll(toRemove);
		}

		#region ICollection

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			return Remove(item.Key);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			return ContainsKey(item.Key);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			GetAliveKvps().CopyTo(array, arrayIndex);
		}

		private IEnumerable<KeyValuePair<TKey, TValue>> GetAliveKvps()
		{
			foreach (KeyValuePair<WeakKeyReference<TKey>, TValue> kvp in m_Dictionary)
			{
				WeakKeyReference<TKey> weakKey = kvp.Key;
				TKey key = weakKey.Target;
				if (weakKey.IsAlive)
					yield return new KeyValuePair<TKey, TValue>(key, kvp.Value);
			}
		}

		#endregion

		#region IEnumerator

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return GetAliveKvps().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#endregion
	}
}
