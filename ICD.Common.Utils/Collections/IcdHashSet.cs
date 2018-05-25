using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;

namespace ICD.Common.Utils.Collections
{
	/// <summary>
	/// A collection containing only unique items.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class IcdHashSet<T> : ICollection<T>
	{
		private readonly Dictionary<T, object> m_Dict;

		#region Properties

		/// <summary>
		/// Returns a new, empty hashset.
		/// </summary>
		[PublicAPI]
		public static IcdHashSet<T> NullSet { get { return new IcdHashSet<T>(); } }

		/// <summary>
		/// Gets the number of items contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <returns>
		/// The number of items contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </returns>
		public int Count { get { return m_Dict.Count; } }

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
		/// </summary>
		/// <returns>
		/// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
		/// </returns>
		public bool IsReadOnly { get { return false; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public IcdHashSet()
		{
			m_Dict = new Dictionary<T, object>();
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="items"></param>
		public IcdHashSet(IEnumerable<T> items)
			: this()
		{
			if (items == null)
				return;

			AddRange(items);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Returns a set containing all of this sets items plus all of the items in the given set.
		/// </summary>
		/// <param name="set"></param>
		/// <returns></returns>
		[PublicAPI]
		public IcdHashSet<T> Union(IEnumerable<T> set)
		{
			IcdHashSet<T> unionSet = new IcdHashSet<T>(this);

			if (set == null)
				return unionSet;

			unionSet.AddRange(set);

			return unionSet;
		}

		/// <summary>
		/// Returns a new set of this sets items exluding the items in the given set.
		/// </summary>
		/// <param name="set"></param>
		/// <returns></returns>
		[PublicAPI]
		public IcdHashSet<T> Subtract(IEnumerable<T> set)
		{
			IcdHashSet<T> subtractSet = new IcdHashSet<T>(this);

			if (set == null)
				return subtractSet;

			foreach (T item in set)
				subtractSet.Remove(item);

			return subtractSet;
		}

		/// <summary>
		/// Returns all of the items that are common between this set and the given set.
		/// </summary>
		/// <param name="set"></param>
		/// <returns></returns>
		[PublicAPI]
		public IcdHashSet<T> Intersection(IcdHashSet<T> set)
		{
			IcdHashSet<T> intersectionSet = NullSet;

			if (set == null)
				return intersectionSet;

			foreach (T item in this.Where(set.Contains))
				intersectionSet.Add(item);

			foreach (T item in set.Where(Contains))
				intersectionSet.Add(item);

			return intersectionSet;
		}

		/// <summary>
		/// Returns items that are not common between both sets.
		/// </summary>
		/// <param name="set"></param>
		/// <returns></returns>
		[PublicAPI]
		public IcdHashSet<T> NonIntersection(IcdHashSet<T> set)
		{
			IcdHashSet<T> setToCompare = set ?? NullSet;

			return Subtract(set).Union(setToCompare.Subtract(this));
		}

		/// <summary>
		/// Returns true if the given set contains all of the items in this set.
		/// </summary>
		/// <param name="set"></param>
		/// <returns></returns>
		[PublicAPI]
		public bool IsSubsetOf(IcdHashSet<T> set)
		{
			IcdHashSet<T> setToCompare = set ?? NullSet;

			return this.All(setToCompare.Contains);
		}

		/// <summary>
		/// Returns true if the given set contains all of the items in this set, and the sets are not equal.
		/// </summary>
		/// <param name="set"></param>
		/// <returns></returns>
		[PublicAPI]
		public bool IsProperSubsetOf(IcdHashSet<T> set)
		{
			IcdHashSet<T> setToCompare = set ?? NullSet;

			return IsSubsetOf(setToCompare) && !setToCompare.IsSubsetOf(this);
		}

		/// <summary>
		/// Returns true if this set contains all of the items in the given set.
		/// </summary>
		/// <param name="set"></param>
		/// <returns></returns>
		[PublicAPI]
		public bool IsSupersetOf(IcdHashSet<T> set)
		{
			IcdHashSet<T> setToCompare = set ?? NullSet;

			return setToCompare.IsSubsetOf(this);
		}

		/// <summary>
		/// Returns true if this set contains all of the items in the given set, and the sets are not equal.
		/// </summary>
		/// <param name="set"></param>
		/// <returns></returns>
		[PublicAPI]
		public bool IsProperSupersetOf(IcdHashSet<T> set)
		{
			IcdHashSet<T> setToCompare = set ?? NullSet;

			return IsSupersetOf(setToCompare) && !setToCompare.IsSupersetOf(this);
		}

		/// <summary>
		/// Returns true if this set contains all of the items in the given set, and vice versa.
		/// </summary>
		/// <param name="set"></param>
		/// <returns></returns>
		[PublicAPI]
		public bool SetEquals(IcdHashSet<T> set)
		{
			IcdHashSet<T> setToCompare = set ?? NullSet;

			return IsSupersetOf(setToCompare) && setToCompare.IsSupersetOf(this);
		}

		#endregion

		#region ICollection<T> Members

		/// <summary>
		/// Adds the item to the collection. Returns false if the item already exists.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Add(T item)
		{
			// ReSharper disable CompareNonConstrainedGenericWithNull
			if (item == null)
				// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("item");

			if (m_Dict.ContainsKey(item))
				return false;

			m_Dict[item] = null;
			return true;
		}

		/// <summary>
		/// Adds the item to the collection.
		/// </summary>
		/// <param name="item"></param>
		void ICollection<T>.Add(T item)
		{
			Add(item);
		}

		/// <summary>
		/// Adds each of the items in the sequence to the collection.
		/// </summary>
		/// <param name="items"></param>
		public void AddRange(IEnumerable<T> items)
		{
			if (items == null)
				throw new ArgumentNullException("items");

			foreach (T item in items)
			{
				// ReSharper disable CompareNonConstrainedGenericWithNull
				if (item == null)
					// ReSharper restore CompareNonConstrainedGenericWithNull
					throw new InvalidOperationException("item");

				if (!m_Dict.ContainsKey(item))
					m_Dict[item] = null;
			}
		}

		/// <summary>
		/// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
		public void Clear()
		{
			m_Dict.Clear();
		}

		/// <summary>
		/// Returns true if the IcdHashSet contains the given item.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(T item)
		{
			return m_Dict.ContainsKey(item);
		}

		/// <summary>
		/// Copies the items of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the items copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or-<paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.-or-The number of items in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type T cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
		public void CopyTo(T[] array, int arrayIndex)
		{
			m_Dict.Keys.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <returns>
		/// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </returns>
		/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
		public bool Remove(T item)
		{
			return m_Dict.Remove(item);
		}

		#endregion

		#region Implementation of IEnumerable

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<T> GetEnumerator()
		{
			return m_Dict.Keys.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
