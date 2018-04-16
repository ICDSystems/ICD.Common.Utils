using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;

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
		public static void AddSorted<T>(this List<T> extends, IEnumerable<T> items)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (items == null)
				throw new ArgumentNullException("items");

			extends.AddSorted(items, Comparer<T>.Default);
		}

		/// <summary>
		/// Adds the item into a sorted list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="items"></param>
		/// <param name="comparer"></param>
		[PublicAPI]
		public static void AddSorted<T>(this List<T> extends, IEnumerable<T> items, IComparer<T> comparer)
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
		/// Adds the item into a sorted list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		[PublicAPI]
		public static void AddSorted<T>(this List<T> extends, T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			extends.AddSorted(item, Comparer<T>.Default);
		}

		/// <summary>
		/// Adds the item into a sorted list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <param name="comparer"></param>
		[PublicAPI]
		public static void AddSorted<T>(this List<T> extends, T item, IComparer<T> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			if (extends.Count == 0)
			{
				extends.Add(item);
				return;
			}

			if (comparer.Compare(extends[extends.Count - 1], item) <= 0)
			{
				extends.Add(item);
				return;
			}

			if (comparer.Compare(extends[0], item) >= 0)
			{
				extends.Insert(0, item);
				return;
			}

			int index = extends.BinarySearch(item, comparer);
			if (index < 0)
				index = ~index;

			extends.Insert(index, item);
		}

		/// <summary>
		/// Pads the list to the given total length.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="totalLength"></param>
		[PublicAPI]
		public static void PadRight<T>(this List<T> extends, int totalLength)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (totalLength < 0)
				throw new ArgumentOutOfRangeException("totalLength", "totalLength must be greater or equal to 0");

			extends.PadRight(totalLength, default(T));
		}

		/// <summary>
		/// Pads the list to the given total length with the given item.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="totalLength"></param>
		/// <param name="item"></param>
		[PublicAPI]
		public static void PadRight<T>(this List<T> extends, int totalLength, T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (totalLength < 0)
				throw new ArgumentOutOfRangeException("totalLength", "totalLength must be greater or equal to 0");

			int pad = totalLength - extends.Count;
			if (pad <= 0)
				return;

			extends.AddRange(Enumerable.Repeat(item, pad));
		}
	}
}
