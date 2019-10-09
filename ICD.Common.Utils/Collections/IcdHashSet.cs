using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

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
			: this(Enumerable.Empty<T>())
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="items"></param>
		public IcdHashSet([NotNull] IEnumerable<T> items)
			: this(EqualityComparer<T>.Default, items)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="comparer"></param>
		public IcdHashSet([NotNull] IEqualityComparer<T> comparer)
			: this(comparer, Enumerable.Empty<T>())
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="comparer"></param>
		/// <param name="items"></param>
		public IcdHashSet([NotNull] IEqualityComparer<T> comparer, [NotNull] IEnumerable<T> items)
		{
			if (comparer == null)
				throw new ArgumentNullException("comparer");

			if (items == null)
				throw new ArgumentNullException("items");

			m_Dict = new Dictionary<T, object>(comparer);

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
		[NotNull]
		public IcdHashSet<T> Union([NotNull] IEnumerable<T> set)
		{
			if (set == null)
				throw new ArgumentNullException("set");

			IcdHashSet<T> unionSet = new IcdHashSet<T>(m_Dict.Comparer, this);
			unionSet.AddRange(set);

			return unionSet;
		}

		/// <summary>
		/// Returns a new set of this sets items exluding the items in the given set.
		/// </summary>
		/// <param name="set"></param>
		/// <returns></returns>
		[PublicAPI]
		[NotNull]
		public IcdHashSet<T> Subtract([NotNull] IEnumerable<T> set)
		{
			if (set == null)
				throw new ArgumentNullException("set");

			IcdHashSet<T> subtractSet = new IcdHashSet<T>(m_Dict.Comparer, this);

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
		[NotNull]
		public IcdHashSet<T> Intersection([NotNull] IEnumerable<T> set)
		{
			if (set == null)
				throw new ArgumentNullException("set");

			IcdHashSet<T> intersectionSet = new IcdHashSet<T>(m_Dict.Comparer);

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
		[NotNull]
		public IcdHashSet<T> NonIntersection([NotNull] IEnumerable<T> set)
		{
			if (set == null)
				throw new ArgumentNullException("set");

			IcdHashSet<T> output = new IcdHashSet<T>(m_Dict.Comparer, this);

			foreach (T item in set)
			{
				if (output.Contains(item))
					output.Remove(item);
				else
					output.Add(item);
			}

			return output;
		}

		/// <summary>
		/// Returns true if the given set contains all of the items in this set.
		/// </summary>
		/// <param name="set"></param>
		/// <returns></returns>
		[PublicAPI]
		public bool IsSubsetOf([NotNull] IcdHashSet<T> set)
		{
			if (set == null)
				throw new ArgumentNullException("set");

			return Count <= set.Count && this.All(set.Contains);
		}

		/// <summary>
		/// Returns true if the given set contains all of the items in this set, and the sets are not equal.
		/// </summary>
		/// <param name="set"></param>
		/// <returns></returns>
		[PublicAPI]
		public bool IsProperSubsetOf([NotNull] IcdHashSet<T> set)
		{
			if (set == null)
				throw new ArgumentNullException("set");

			return Count < set.Count && IsSubsetOf(set);
		}

		/// <summary>
		/// Returns true if this set contains all of the items in the given set.
		/// </summary>
		/// <param name="set"></param>
		/// <returns></returns>
		[PublicAPI]
		public bool IsSupersetOf([NotNull] IcdHashSet<T> set)
		{
			if (set == null)
				throw new ArgumentNullException("set");

			return set.IsSubsetOf(this);
		}

		/// <summary>
		/// Returns true if this set contains all of the items in the given set, and the sets are not equal.
		/// </summary>
		/// <param name="set"></param>
		/// <returns></returns>
		[PublicAPI]
		public bool IsProperSupersetOf([NotNull] IcdHashSet<T> set)
		{
			if (set == null)
				throw new ArgumentNullException("set");

			return set.IsProperSubsetOf(this);
		}

		/// <summary>
		/// Returns true if this set contains all of the items in the given set, and vice versa.
		/// </summary>
		/// <param name="set"></param>
		/// <returns></returns>
		[PublicAPI]
		public bool SetEquals([NotNull] IcdHashSet<T> set)
		{
			if (set == null)
				throw new ArgumentNullException("set");

			return Count == set.Count && set.All(Contains);
		}

		#endregion

		#region ICollection<T>

		/// <summary>
		/// Adds the item to the collection. Returns false if the item already exists.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Add([NotNull] T item)
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
		void ICollection<T>.Add([NotNull] T item)
		{
			Add(item);
		}

		/// <summary>
		/// Adds each of the items in the sequence to the collection.
		/// </summary>
		/// <param name="items"></param>
		public void AddRange([NotNull] IEnumerable<T> items)
		{
			if (items == null)
				throw new ArgumentNullException("items");

			foreach (T item in items)
				m_Dict[item] = null;
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
		public bool Contains([NotNull] T item)
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
			if (item == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("item");

			return m_Dict.ContainsKey(item);
		}

		/// <summary>
		/// Copies the items of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the items copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or-<paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.-or-The number of items in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type T cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
		public void CopyTo([NotNull] T[] array, int arrayIndex)
		{
			if (array == null)
				throw new ArgumentNullException("array");

			m_Dict.Keys.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <returns>
		/// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </returns>
		/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
		public bool Remove([NotNull] T item)
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
			if (item == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("item");

			return m_Dict.Remove(item);
		}

		/// <summary>
		/// Removes each of the items in the sequence from the collection.
		/// </summary>
		/// <param name="items"></param>
		public void RemoveRange([NotNull] IEnumerable<T> items)
		{
			if (items == null)
				throw new ArgumentNullException("items");

			m_Dict.RemoveAll(items);
		}

		#endregion

		#region IEnumerable<T>

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		[NotNull]
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
		[NotNull]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
