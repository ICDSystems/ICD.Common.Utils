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
		/// <summary>
		/// Adds the items into a sorted list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="items"></param>
		[PublicAPI]
		public static void AddSorted<T>(this IList<T> extends, IEnumerable<T> items)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (items == null)
				throw new ArgumentNullException("items");

			extends.AddSorted(items, Comparer<T>.Default);
		}

		/// <summary>
		/// Adds the items into a sorted list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="items"></param>
		/// <param name="comparer"></param>
		[PublicAPI]
		public static void AddSorted<T>(this IList<T> extends, IEnumerable<T> items, IComparer<T> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (items == null)
				throw new ArgumentNullException("items");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			items.ForEach(i => extends.AddSorted(i, comparer));
		}

		/// <summary>
		/// Adds the items into a sorted list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProp"></typeparam>
		/// <param name="extends"></param>
		/// <param name="items"></param>
		/// <param name="predicate"></param>
		[PublicAPI]
		public static void AddSorted<T, TProp>(this IList<T> extends, IEnumerable<T> items, Func<T, TProp> predicate)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (items == null)
				throw new ArgumentNullException("items");

			if (predicate == null)
				throw new ArgumentNullException("predicate");

			PredicateComparer<T, TProp> comparer = new PredicateComparer<T, TProp>(predicate);
			extends.AddSorted(items, comparer);
		}

		/// <summary>
		/// Adds the item into a sorted list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		[PublicAPI]
		public static int AddSorted<T>(this IList<T> extends, T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.AddSorted(item, Comparer<T>.Default);
		}

		/// <summary>
		/// Adds the item into a sorted list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <param name="comparer"></param>
		[PublicAPI]
		public static int AddSorted<T>(this IList<T> extends, T item, IComparer<T> comparer)
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

		/// <summary>
		/// Adds the item into a sorted list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProp"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <param name="predicate"></param>
		[PublicAPI]
		public static int AddSorted<T, TProp>(this IList<T> extends, T item, Func<T, TProp> predicate)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (predicate == null)
				throw new ArgumentNullException("predicate");

			PredicateComparer<T, TProp> comparer = new PredicateComparer<T, TProp>(predicate);
			return extends.AddSorted(item, comparer);
		}

		/// <summary>
		/// Returns the index of the item in the list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		[PublicAPI]
		public static int BinarySearch<T>(this IList<T> extends, T item, IComparer<T> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

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
	}
}
