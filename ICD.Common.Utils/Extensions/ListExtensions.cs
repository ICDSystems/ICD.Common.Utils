using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils.Comparers;

namespace ICD.Common.Utils.Extensions
{
	/// <summary>
	/// Extension methods for working with Lists.
	/// </summary>
	public static class ListExtensions
	{
		#region Add Sorted

		/// <summary>
		/// Attempts to add the item to the sorted list.
		/// Returns false if the item already exists in the list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool AddSorted<T>([NotNull] this IList<T> extends, T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.AddSorted(item, Comparer<T>.Default);
		}

		/// <summary>
		/// Attempts to add the item to the sorted list.
		/// Returns false if the item already exists in the list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProp"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <param name="predicate"></param>
		[PublicAPI]
		public static bool AddSorted<T, TProp>([NotNull] this IList<T> extends, T item,
		                                       [NotNull] Func<T, TProp> predicate)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (predicate == null)
				throw new ArgumentNullException("predicate");

			PredicateComparer<T, TProp> comparer = new PredicateComparer<T, TProp>(predicate);
			return extends.AddSorted(item, comparer);
		}

		/// <summary>
		/// Attempts to add the item to the sorted list.
		/// Returns false if the item already exists in the list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool AddSorted<T>([NotNull] this IList<T> extends, T item, [NotNull] IComparer<T> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			int index = extends.BinarySearch(item, comparer);
			if (index >= 0)
				return false;

			index = ~index;
			extends.Insert(index, item);

			return true;
		}

		#endregion

		#region Remove Sorted

		/// <summary>
		/// Attempts to remove the item from the sorted list.
		/// Returns false if the item does not exist in the list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool RemoveSorted<T>([NotNull] this IList<T> extends, T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.RemoveSorted(item, Comparer<T>.Default);
		}

		/// <summary>
		/// Attempts to remove the item from the sorted list.
		/// Returns false if the item does not exist in the list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProp"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <param name="predicate"></param>
		[PublicAPI]
		public static bool RemoveSorted<T, TProp>([NotNull] this IList<T> extends, T item,
		                                          [NotNull] Func<T, TProp> predicate)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (predicate == null)
				throw new ArgumentNullException("predicate");

			PredicateComparer<T, TProp> comparer = new PredicateComparer<T, TProp>(predicate);
			return extends.RemoveSorted(item, comparer);
		}

		/// <summary>
		/// Attempts to remove the item from the sorted list.
		/// Returns false if the item does not exist in the list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool RemoveSorted<T>([NotNull] this IList<T> extends, T item, [NotNull] IComparer<T> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			int index = extends.BinarySearch(item, comparer);
			if (index < 0)
				return false;

			extends.RemoveAt(index);

			return true;
		}

		#endregion

		#region Insert Sorted

		/// <summary>
		/// Inserts the items into a sorted list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="items"></param>
		[PublicAPI]
		public static void InsertSorted<T>([NotNull] this IList<T> extends, [NotNull] IEnumerable<T> items)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (items == null)
				throw new ArgumentNullException("items");

			extends.InsertSorted(items, Comparer<T>.Default);
		}

		/// <summary>
		/// Inserts the items into a sorted list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="items"></param>
		/// <param name="comparer"></param>
		[PublicAPI]
		public static void InsertSorted<T>([NotNull] this IList<T> extends, [NotNull] IEnumerable<T> items,
		                                   [NotNull] IComparer<T> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (items == null)
				throw new ArgumentNullException("items");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			items.ForEach(i => extends.InsertSorted(i, comparer));
		}

		/// <summary>
		/// Inserts the items into a sorted list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProp"></typeparam>
		/// <param name="extends"></param>
		/// <param name="items"></param>
		/// <param name="predicate"></param>
		[PublicAPI]
		public static void InsertSorted<T, TProp>([NotNull] this IList<T> extends, [NotNull] IEnumerable<T> items,
		                                          [NotNull] Func<T, TProp> predicate)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (items == null)
				throw new ArgumentNullException("items");

			if (predicate == null)
				throw new ArgumentNullException("predicate");

			PredicateComparer<T, TProp> comparer = new PredicateComparer<T, TProp>(predicate);
			extends.InsertSorted(items, comparer);
		}

		/// <summary>
		/// Inserts the item into a sorted list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		[PublicAPI]
		public static int InsertSorted<T>([NotNull] this IList<T> extends, T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.InsertSorted(item, Comparer<T>.Default);
		}

		/// <summary>
		/// Inserts the item into a sorted list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProp"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <param name="predicate"></param>
		[PublicAPI]
		public static int InsertSorted<T, TProp>([NotNull] this IList<T> extends, T item,
		                                         [NotNull] Func<T, TProp> predicate)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (predicate == null)
				throw new ArgumentNullException("predicate");

			PredicateComparer<T, TProp> comparer = new PredicateComparer<T, TProp>(predicate);
			return extends.InsertSorted(item, comparer);
		}

		/// <summary>
		/// Inserts the item into a sorted list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <param name="comparer"></param>
		[PublicAPI]
		public static int InsertSorted<T>([NotNull] this IList<T> extends, T item, [NotNull] IComparer<T> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			int index = extends.BinarySearch(item, comparer);
			if (index < 0)
				index = ~index;

			extends.Insert(index, item);

			return index;
		}

		#endregion

		#region Contains Sorted

		/// <summary>
		/// Returns true if the sorted list contains the given item.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool ContainsSorted<T>([NotNull] this IList<T> extends, T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.ContainsSorted(item, Comparer<T>.Default);
		}

		/// <summary>
		/// Returns true if the sorted list contains the given item.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool ContainsSorted<T>([NotNull] this IList<T> extends, T item, [NotNull] IComparer<T> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			return extends.BinarySearch(item, comparer) >= 0;
		}

		#endregion

		#region Binary Search

		/// <summary>
		/// Returns the index of the item in the list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		[PublicAPI]
		public static int BinarySearch<T>([NotNull] this IList<T> extends, T item, [NotNull] IComparer<T> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			// Array
			T[] array = extends as T[];
			if (array != null)
				return Array.BinarySearch(array, 0, array.Length, item, comparer);

			// List
			List<T> list = extends as List<T>;
			if (list != null)
				return list.BinarySearch(item, comparer);

			// IList
			int lo = 0;
			int hi = extends.Count - 1;

			while (lo <= hi)
			{
				int i = lo + ((hi - lo) >> 1);
				int order = comparer.Compare(extends[i], item);

				if (order == 0)
					return i;

				if (order < 0)
					lo = i + 1;
				else
					hi = i - 1;
			}

			return ~lo;
		}

		#endregion
	}
}
