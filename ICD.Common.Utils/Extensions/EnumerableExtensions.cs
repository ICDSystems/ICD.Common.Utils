﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Comparers;

namespace ICD.Common.Utils.Extensions
{
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Returns the first item in the sequence. Returns the provided default item if there
		/// are no elements in the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="defaultItem"></param>
		/// <returns></returns>
		public static T FirstOrDefault<T>([NotNull] this IEnumerable<T> extends, T defaultItem)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.FirstOrDefault(i => true, defaultItem);
		}

		/// <summary>
		/// Returns the first element in the sequence matching the predicate. Returns the provided
		/// default item if there are no elements matching the predicate in the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="predicate"></param>
		/// <param name="defaultItem"></param>
		/// <returns></returns>
		public static T FirstOrDefault<T>([NotNull] this IEnumerable<T> extends, [NotNull] Func<T, bool> predicate,
		                                  T defaultItem)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (predicate == null)
				throw new ArgumentNullException("predicate");

			T output;
			return extends.TryFirst(predicate, out output) ? output : defaultItem;
		}

		/// <summary>
		/// Returns true if there is at least 1 item in the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item">Outputs the first item in the sequence.</param>
		/// <returns></returns>
		public static bool TryFirst<T>([NotNull] this IEnumerable<T> extends, out T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			IList<T> list = extends as IList<T>;
			if (list == null)
				return extends.TryFirst(i => true, out item);

			item = default(T);

			if (list.Count <= 0)
				return false;

			item = list[0];

			return true;
		}

		/// <summary>
		/// Returns true if there is at least 1 item in the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="predicate"></param>
		/// <param name="item">Outputs the first item in the sequence.</param>
		/// <returns></returns>
		public static bool TryFirst<T>([NotNull] this IEnumerable<T> extends, [NotNull] Func<T, bool> predicate,
		                               out T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (predicate == null)
				throw new ArgumentNullException("predicate");

			item = default(T);

			using (IEnumerator<T> iterator = extends.GetEnumerator())
			{
				while (iterator.MoveNext())
				{
					if (!predicate(iterator.Current))
						continue;

					item = iterator.Current;
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Returns true if there is at least 1 item in the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item">Outputs the last item in the sequence.</param>
		/// <returns></returns>
		public static bool TryLast<T>([NotNull] this IEnumerable<T> extends, out T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			IList<T> list = extends as IList<T>;
			if (list == null)
				return extends.TryLast(i => true, out item);

			item = default(T);

			if (list.Count <= 0)
				return false;

			item = list[list.Count - 1];

			return true;
		}

		/// <summary>
		/// Returns true if there is at least 1 item in the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="predicate"></param>
		/// <param name="item">Outputs the last item in the sequence.</param>
		/// <returns></returns>
		public static bool TryLast<T>([NotNull] this IEnumerable<T> extends, [NotNull] Func<T, bool> predicate,
		                              out T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (predicate == null)
				throw new ArgumentNullException("predicate");

			item = default(T);
			bool output = false;

			using (IEnumerator<T> iterator = extends.GetEnumerator())
			{
				while (iterator.MoveNext())
				{
					if (!predicate(iterator.Current))
						continue;

					item = iterator.Current;
					output = true;
				}
			}

			return output;
		}

		/// <summary>
		/// Returns the true if an element with the given index is in the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="index"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public static bool TryElementAt<T>([NotNull] this IEnumerable<T> extends, int index, out T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (index < 0)
				throw new ArgumentOutOfRangeException("index");

			item = default(T);

			IList<T> list = extends as IList<T>;
			if (list != null)
			{
				if (index >= list.Count)
					return false;
				item = list[index];
				return true;
			}

			int current = 0;
			foreach (T value in extends)
			{
				if (current == index)
				{
					item = value;
					return true;
				}

				current++;
			}

			return false;
		}

		/// <summary>
		/// Gets the element at the given index. Returns the specified default value if the index does not exist.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="index"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static T ElementAtOrDefault<T>([NotNull] this IEnumerable<T> extends, int index, T defaultValue)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			T output;
			return extends.TryElementAt(index, out output) ? output : defaultValue;
		}

		/// <summary>
		/// Compares the two sequences for identical values.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="other"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static bool SequenceEqual<T>([NotNull] this IEnumerable<T> extends, [NotNull] IEnumerable<T> other,
		                                    [NotNull] Func<T, T, bool> predicate)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (other == null)
				throw new ArgumentNullException("other");

			if (predicate == null)
				throw new ArgumentNullException("predicate");

			// Simple count check
			ICollection<T> a = extends as ICollection<T>;
			ICollection<T> b = other as ICollection<T>;
			if (a != null && b != null && a.Count != b.Count)
				return false;

			using (IEnumerator<T> firstPos = extends.GetEnumerator())
			{
				using (IEnumerator<T> secondPos = other.GetEnumerator())
				{
					bool hasFirst = firstPos.MoveNext();
					bool hasSecond = secondPos.MoveNext();

					while (hasFirst && hasSecond)
					{
						if (!predicate(firstPos.Current, secondPos.Current))
							return false;

						hasFirst = firstPos.MoveNext();
						hasSecond = secondPos.MoveNext();
					}

					return !hasFirst && !hasSecond;
				}
			}
		}

		/// <summary>
		/// Compares two sequences for identical values, ignoring order.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool ScrambledEquals<T>([NotNull] this IEnumerable<T> extends, [NotNull] IEnumerable<T> other)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (other == null)
				throw new ArgumentNullException("other");

			return extends.ScrambledEquals(other, EqualityComparer<T>.Default);
		}

		/// <summary>
		/// Compares two sequences for identical values, ignoring order.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="other"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static bool ScrambledEquals<T>([NotNull] this IEnumerable<T> extends, [NotNull] IEnumerable<T> other,
		                                      [NotNull] IEqualityComparer<T> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (other == null)
				throw new ArgumentNullException("other");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			// Simple count check
			ICollection<T> a = extends as ICollection<T>;
			ICollection<T> b = other as ICollection<T>;
			if (a != null && b != null && a.Count != b.Count)
				return false;

			Dictionary<T, int> count = new Dictionary<T, int>(comparer);

			foreach (T item in extends)
				count[item] = count.GetDefault(item, 0) + 1;

			foreach (T item in other)
				count[item] = count.GetDefault(item, 0) - 1;

			return count.Values.All(c => c == 0);
		}

		/// <summary>
		/// Returns the index that matches the predicate.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="match"></param>
		/// <returns></returns>
		public static int FindIndex<T>([NotNull] this IEnumerable<T> extends, [NotNull] Predicate<T> match)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (match == null)
				throw new ArgumentNullException("match");

			return extends.FindIndices(match).FirstOrDefault(-1);
		}

		/// <summary>
		/// Returns the indices that match the predicate.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="match"></param>
		/// <returns></returns>
		public static IEnumerable<int> FindIndices<T>([NotNull] this IEnumerable<T> extends,
		                                              [NotNull] Predicate<T> match)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (match == null)
				throw new ArgumentNullException("match");

			return FindIndicesIterator(extends, match);
		}

		/// <summary>
		/// Returns the indices that match the predicate.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sequence"></param>
		/// <param name="match"></param>
		/// <returns></returns>
		private static IEnumerable<int> FindIndicesIterator<T>([NotNull] IEnumerable<T> sequence,
		                                                       [NotNull] Predicate<T> match)
		{
			int index = 0;

			foreach (T item in sequence)
			{
				if (match(item))
					yield return index;
				index++;
			}
		}

		/// <summary>
		/// Allows for selection of multiple results for each item in the sequence.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="extends"></param>
		/// <param name="selectors"></param>
		/// <returns></returns>
		public static IEnumerable<TResult> SelectMulti<TSource, TResult>([NotNull] this IEnumerable<TSource> extends,
		                                                                 [NotNull]
		                                                                 params Func<TSource, TResult>[] selectors)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (selectors == null)
				throw new ArgumentNullException("selectors");

			return extends.SelectMany(source => selectors, (source, selector) => selector(source));
		}

		/// <summary>
		/// Enumerates each item in the sequence.
		/// Useful for processing a LINQ query without creating a new collection.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		[PublicAPI]
		public static void Execute<T>([NotNull] this IEnumerable<T> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

// ReSharper disable UnusedVariable
			foreach (T item in extends)
// ReSharper restore UnusedVariable
			{
			}
		}

		/// <summary>
		/// Performs the action for each item in the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="action"></param>
		[PublicAPI]
		public static void ForEach<T>([NotNull] this IEnumerable<T> extends, [NotNull] Action<T> action)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (action == null)
				throw new ArgumentNullException("action");

			extends.ForEach((item, index) => action(item));
		}

		/// <summary>
		/// Performs the action for each item in the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="action"></param>
		[PublicAPI]
		public static void ForEach<T>([NotNull] this IEnumerable<T> extends, [NotNull] Action<T, int> action)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (action == null)
				throw new ArgumentNullException("action");

			int index = 0;
			foreach (T item in extends)
				action(item, index++);
		}

#if SIMPLSHARP
		/// <summary>
		/// Prepends the item to the start of the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public static IEnumerable<T> Prepend<T>([NotNull]this IEnumerable<T> extends, T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return PrependIterator(extends, item);
		}

		/// <summary>
		/// Prepends the item to the start of the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sequence"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		private static IEnumerable<T> PrependIterator<T>([NotNull]IEnumerable<T> sequence, T item)
		{
			yield return item;

			foreach (T next in sequence)
				yield return next;
		}
#endif

		/// <summary>
		/// Prepends the items to the start of the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="items"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<T> PrependMany<T>([NotNull] this IEnumerable<T> extends, [NotNull] params T[] items)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (items == null)
				throw new ArgumentNullException("items");

			return PrependManyIterator(extends, items);
		}

		/// <summary>
		/// Prepends the items to the start of the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sequence"></param>
		/// <param name="items"></param>
		/// <returns></returns>
		private static IEnumerable<T> PrependManyIterator<T>([NotNull] IEnumerable<T> sequence,
		                                                     [NotNull] params T[] items)
		{
			foreach (T item in items)
				yield return item;

			foreach (T each in sequence)
				yield return each;
		}

#if SIMPLSHARP
		/// <summary>
		/// Appends the item to the end of the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public static IEnumerable<T> Append<T>([NotNull]this IEnumerable<T> extends, T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return AppendIterator(extends, item);
		}

		/// <summary>
		/// Appends the item to the end of the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sequence"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		private static IEnumerable<T> AppendIterator<T>([NotNull]IEnumerable<T> sequence, T item)
		{
			foreach (T first in sequence)
				yield return first;

			yield return item;
		}
#endif

		/// <summary>
		/// Appends the items to the end of the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="items"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<T> AppendMany<T>([NotNull] this IEnumerable<T> extends, [NotNull] params T[] items)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (items == null)
				throw new ArgumentNullException("items");

			return AppendManyIterator(extends, items);
		}

		/// <summary>
		/// Appends the items to the end of the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sequence"></param>
		/// <param name="items"></param>
		/// <returns></returns>
		private static IEnumerable<T> AppendManyIterator<T>([NotNull] IEnumerable<T> sequence,
		                                                    [NotNull] params T[] items)
		{
			foreach (T each in sequence)
				yield return each;

			foreach (T item in items)
				yield return item;
		}

		/// <summary>
		/// Pads the given sequence to the given count size with default items.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<T> PadRight<T>([NotNull] this IEnumerable<T> extends, int count)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return PadRightIterator(extends, count);
		}

		/// <summary>
		/// Pads the given sequence to the given count size with default items.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sequence"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		private static IEnumerable<T> PadRightIterator<T>([NotNull] IEnumerable<T> sequence, int count)
		{
			int index = 0;

			foreach (T item in sequence)
			{
				yield return item;
				index++;
			}

			for (; index < count; index++)
				yield return default(T);
		}

		/// <summary>
		/// Returns true if the given sequence is ordered.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static bool AreOrdered<T>([NotNull] this IEnumerable<T> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.AreOrdered(Comparer<T>.Default);
		}

		/// <summary>
		/// Returns true if the given sequence is ordered.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static bool AreOrdered<T>([NotNull] this IEnumerable<T> extends, [NotNull] IComparer<T> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			bool first = true;
			T previous = default(T);

			foreach (T item in extends)
			{
				if (!first && comparer.Compare(item, previous) < 0)
					return false;

				first = false;
				previous = item;
			}

			return true;
		}

		/// <summary>
		/// Default ordering for the items in the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static IOrderedEnumerable<T> Order<T>([NotNull] this IEnumerable<T> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.OrderBy(i => i);
		}

		/// <summary>
		/// Shorthand for ordering the given sequence with the given comparer.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static IOrderedEnumerable<T> Order<T>([NotNull] this IEnumerable<T> extends,
		                                             [NotNull] IComparer<T> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			return extends.OrderBy(i => i, comparer);
		}

		/// <summary>
		/// Shorthand for ordering the given sequence with the given comparer.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static IOrderedEnumerable<T> OrderDescending<T>([NotNull] this IEnumerable<T> extends,
		                                                       [NotNull] IComparer<T> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			return extends.OrderByDescending(i => i, comparer);
		}

		/// <summary>
		/// Returns every item in the sequence except the given item.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public static IEnumerable<T> Except<T>([NotNull] this IEnumerable<T> extends, T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.Except(item, EqualityComparer<T>.Default);
		}

		/// <summary>
		/// Returns every item in the sequence except the given item.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static IEnumerable<T> Except<T>([NotNull] this IEnumerable<T> extends, T item,
		                                       [NotNull] IEqualityComparer<T> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			return extends.Where(i => !comparer.Equals(item, i));
		}

		/// <summary>
		/// Removes any null elements from an enumerable of nullable value types
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<T> ExceptNulls<T>([NotNull] this IEnumerable<T?> extends)
			where T : struct
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.OfType<T>();
		}

		/// <summary>
		/// Copies all the elements of the current one-dimensional array to the specified one-dimensional array
		/// starting at the specified destination array index. The index is specified as a 32-bit integer.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public static void CopyTo<T>([NotNull] this IEnumerable<T> extends, [NotNull] T[] array, int index)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (array == null)
				throw new ArgumentNullException("array");

			ICollection<T> collection = extends as ICollection<T>;
			if (collection != null)
			{
				collection.CopyTo(array, index);
				return;
			}

			int current = 0;
			foreach (T item in extends)
			{
				array[index + current] = item;
				current++;
			}
		}

		/// <summary>
		/// Creates a new collection and fills it with the items of the enumerable.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TCollection"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static TCollection ToCollection<T, TCollection>([NotNull] this IEnumerable<T> extends)
			where TCollection : ICollection<T>
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			TCollection collection = ReflectionUtils.CreateInstance<TCollection>();
			foreach (T item in extends)
				collection.Add(item);

			return collection;
		}

		/// <summary>
		/// Returns the sequence as a IcdHashSet.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static IcdHashSet<T> ToIcdHashSet<T>([NotNull] this IEnumerable<T> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.ToIcdHashSet(EqualityComparer<T>.Default);
		}

		/// <summary>
		/// Returns the sequence as a IcdHashSet.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static IcdHashSet<T> ToIcdHashSet<T>([NotNull] this IEnumerable<T> extends,
		                                            [NotNull] IEqualityComparer<T> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			return new IcdHashSet<T>(comparer, extends);
		}

		/// <summary>
		/// Optimized ToArray implementation with fewer allocations.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static T[] ToArray<T>([NotNull] this IEnumerable<T> extends, int count)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (count < 0)
				throw new ArgumentOutOfRangeException("count");

			// Source is already an array
			T[] arrayCast = extends as T[];
			if (arrayCast != null)
			{
				T[] output = new T[count];
				Array.Copy(arrayCast, output, count);
				return output;
			}

			// Dumb sequence case
			T[] array = new T[count];

			int i = 0;
			foreach (T item in extends)
			{
				array[i++] = item;
				if (i >= count)
					break;
			}

			if (i != count)
				throw new ArgumentOutOfRangeException("count");

			return array;
		}

		/// <summary>
		/// Creates a generic List of the given item type.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="itemType"></param>
		/// <returns></returns>
		public static IList ToList<T>([NotNull] this IEnumerable<T> extends, [NotNull] Type itemType)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (itemType == null)
				throw new ArgumentNullException("itemType");

			Type genericListType = typeof(List<>);
			Type specificListType = genericListType.MakeGenericType(itemType);
			IList list = (IList)ReflectionUtils.CreateInstance(specificListType);

			foreach (object item in extends)
				list.Add(item);

			return list;
		}

		/// <summary>
		/// Optimized ToList implementation with fewer allocations.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static List<T> ToList<T>([NotNull] this IEnumerable<T> extends, int count)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (count < 0)
				throw new ArgumentOutOfRangeException("count");

			List<T> list = new List<T>(count);
			list.AddRange(extends);
			return list;
		}

		/// <summary>
		/// Returns the sequence as an index:value dictionary.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static Dictionary<int, T> ToIndexedDictionary<T>([NotNull] this IEnumerable<T> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			Dictionary<int, T> output = new Dictionary<int, T>();
			extends.ForEach((item, index) => output.Add(index, item));
			return output;
		}

		/// <summary>
		/// Returns the sequence as an index:value dictionary.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static Dictionary<uint, T> ToIndexedDictionaryUInt<T>([NotNull] this IEnumerable<T> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			Dictionary<uint, T> output = new Dictionary<uint, T>();
			extends.ForEach((item, index) => output.Add((uint)index, item));
			return output;
		}

		/// <summary>
		/// Returns true if all of the items in the sequence are equal, or the sequence is empty.
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static bool IsDistinct<TItem>([NotNull] this IEnumerable<TItem> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			IEqualityComparer<TItem> comparer = EqualityComparer<TItem>.Default;
			return extends.IsDistinct(comparer);
		}

		/// <summary>
		/// Returns true if all of the items in the sequence are equal, or the sequence is empty.
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <param name="extends"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static bool IsDistinct<TItem>([NotNull] this IEnumerable<TItem> extends, [NotNull] IEqualityComparer<TItem> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			TItem other = default(TItem);
			bool first = true;

			foreach (TItem item in extends)
			{
				if (first)
				{
					other = item;
					first = false;
					continue;
				}

				if (!comparer.Equals(item, other))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Gets distinct elements from the sequence based on given property.
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="extends"></param>
		/// <param name="getProperty"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<TItem> Distinct<TItem, TProperty>([NotNull] this IEnumerable<TItem> extends,
		                                                            [NotNull] Func<TItem, TProperty> getProperty)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (getProperty == null)
				throw new ArgumentNullException("getProperty");

			IEqualityComparer<TProperty> comparer = EqualityComparer<TProperty>.Default;
			return extends.Distinct(getProperty, comparer);
		}

		/// <summary>
		/// Gets distinct elements from the sequence based on given property.
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="extends"></param>
		/// <param name="getProperty"></param>
		/// <param name="propertyComparer"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<TItem> Distinct<TItem, TProperty>([NotNull] this IEnumerable<TItem> extends,
		                                                            Func<TItem, TProperty> getProperty,
		                                                            [NotNull]
		                                                            IEqualityComparer<TProperty> propertyComparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (getProperty == null)
				throw new ArgumentNullException("getProperty");

			if (propertyComparer == null)
				throw new ArgumentNullException("propertyComparer");

			return extends.Distinct(new PredicateEqualityComparer<TItem, TProperty>(propertyComparer, getProperty));
		}

		/// <summary>
		/// Returns a random item from the given sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static T Random<T>([NotNull] this IEnumerable<T> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			Random random = new Random(Guid.NewGuid().GetHashCode());
			return extends.Random(random);
		}

		/// <summary>
		/// Returns a random item from the given sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="random"></param>
		/// <returns></returns>
		public static T Random<T>([NotNull] this IEnumerable<T> extends, [NotNull] Random random)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			IList<T> sequence = extends as IList<T> ?? extends.ToArray();

			if (sequence.Count == 0)
				throw new InvalidOperationException("Sequence is empty.");

			int index = random.Next(0, sequence.Count);

			return sequence[index];
		}

		/// <summary>
		/// Returns other if the sequence is empty.
		/// Returns other if the sequence is non-empty and there are two different elements.
		/// Returns the element of the sequence if it is non-empty and all elements are the same.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		[PublicAPI]
		public static T Unanimous<T>([NotNull] this IEnumerable<T> extends, T other)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			T item;
			return extends.Unanimous(out item) ? item : other;
		}

		/// <summary>
		/// Returns false if the sequence is empty.
		/// Returns false if the sequence is non-empty and there are two different elements.
		/// Returns true if the sequence is non-empty and all elements are the same.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool Unanimous<T>([NotNull] this IEnumerable<T> extends, out T result)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.Unanimous(EqualityComparer<T>.Default, out result);
		}

		/// <summary>
		/// Returns false if the sequence is empty.
		/// Returns false if the sequence is non-empty and there are two different elements.
		/// Returns true if the sequence is non-empty and all elements are the same.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="comparer"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool Unanimous<T>([NotNull] this IEnumerable<T> extends, [NotNull] IEqualityComparer<T> comparer,
		                                out T result)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			result = default(T);
			T output = default(T);
			bool empty = true;

			foreach (T entry in extends)
			{
				if (empty)
				{
					empty = false;
					output = entry;
					continue;
				}

				if (!comparer.Equals(entry, output))
					return false;
			}

			if (empty)
				return false;

			result = output;
			return true;
		}

		/// <summary>
		/// Partitions a sequence into sequences of the given length.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="partitionSize"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<IEnumerable<T>> Partition<T>([NotNull] this IEnumerable<T> extends, int partitionSize)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return PartitionIterator(extends, partitionSize);
		}

		/// <summary>
		/// Partitions a sequence into sequences of the given length.
		/// </summary>
		/// <param name="sequence"></param>
		/// <param name="partitionSize"></param>
		/// <returns></returns>
		private static IEnumerable<IEnumerable<T>> PartitionIterator<T>([NotNull] IEnumerable<T> sequence,
		                                                                int partitionSize)
		{
			using (IEnumerator<T> enumerator = sequence.GetEnumerator())
			{
				while (enumerator.MoveNext())
					yield return YieldBatchElements(enumerator, partitionSize - 1);
			}
		}

		private static IEnumerable<T> YieldBatchElements<T>([NotNull] IEnumerator<T> source, int partitionSize)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			// Important to enumerate through partitionSize items before returning
			// Otherwise enumerable.Partition(3).Skip(1) will do unwanted things.
			List<T> output = new List<T> {source.Current};

			for (int i = 0; i < partitionSize && source.MoveNext(); i++)
				output.Add(source.Current);

			return output;
		}

		/// <summary>
		/// Wraps this object instance into an IEnumerable consisting of a single item.
		/// </summary>
		/// <typeparam name="T">Type of the object.</typeparam>
		/// <param name="item">The instance that will be wrapped.</param>
		/// <returns>An IEnumerable&lt;T&gt; consisting of a single item.</returns>
		public static IEnumerable<T> Yield<T>([CanBeNull] this T item)
		{
			yield return item;
		}

		/// <summary>
		/// Given a sequence [A, B, C] returns a sequence [[A, B], [B, C]]
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static IEnumerable<T[]> GetAdjacentPairs<T>([NotNull] this IEnumerable<T> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return GetAdjacentPairsIterator(extends);
		}

		/// <summary>
		/// Given a sequence [A, B, C] returns a sequence [[A, B], [B, C]]
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		private static IEnumerable<T[]> GetAdjacentPairsIterator<T>([NotNull] IEnumerable<T> extends)
		{
			T previous = default(T);
			bool first = true;

			foreach (T item in extends)
			{
				if (!first)
					yield return new[] {previous, item};

				first = false;
				previous = item;
			}
		}

		/// <summary>
		/// Gets the item from the sequence with the smallest calculated delta.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="getDelta"></param>
		/// <returns></returns>
		public static T GetClosest<T>([NotNull] this IEnumerable<T> extends, [NotNull] Func<T, int> getDelta)
		{
			return extends.MinBy(n => Math.Abs(getDelta(n)));
		}

		/// <summary>
		/// Returns the minimal element of the given sequence, based on
		/// the given projection.
		/// </summary>
		/// <remarks>
		/// If more than one element has the minimal projected value, the first
		/// one encountered will be returned. This overload uses the default comparer
		/// for the projected type. This operator uses immediate execution, but
		/// only buffers a single result (the current minimal element).
		/// </remarks>
		/// <typeparam name="TSource">Type of the source sequence</typeparam>
		/// <typeparam name="TKey">Type of the projected element</typeparam>
		/// <param name="source">Source sequence</param>
		/// <param name="selector">Selector to use to pick the results to compare</param>
		/// <returns>The minimal element, according to the projection.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null</exception>
		/// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
		[PublicAPI]
		public static TSource MinBy<TSource, TKey>([NotNull] this IEnumerable<TSource> source,
		                                           [NotNull] Func<TSource, TKey> selector)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (selector == null)
				throw new ArgumentNullException("selector");

			return source.MinBy(selector, Comparer<TKey>.Default);
		}

		/// <summary>
		/// Returns the minimal element of the given sequence, based on
		/// the given projection and the specified comparer for projected values.
		/// </summary>
		/// <remarks>
		/// If more than one element has the minimal projected value, the first
		/// one encountered will be returned. This operator uses immediate execution, but
		/// only buffers a single result (the current minimal element).
		/// </remarks>
		/// <typeparam name="TSource">Type of the source sequence</typeparam>
		/// <typeparam name="TKey">Type of the projected element</typeparam>
		/// <param name="source">Source sequence</param>
		/// <param name="selector">Selector to use to pick the results to compare</param>
		/// <param name="comparer">Comparer to use to compare projected values</param>
		/// <returns>The minimal element, according to the projection.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="selector"/> 
		/// or <paramref name="comparer"/> is null</exception>
		/// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
		[PublicAPI]
		public static TSource MinBy<TSource, TKey>([NotNull] this IEnumerable<TSource> source,
		                                           [NotNull] Func<TSource, TKey> selector,
		                                           [NotNull] IComparer<TKey> comparer)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (selector == null)
				throw new ArgumentNullException("selector");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
			{
				if (!sourceIterator.MoveNext())
					throw new InvalidOperationException("Sequence contains no elements");

				TSource min = sourceIterator.Current;
				TKey minKey = selector(min);

				while (sourceIterator.MoveNext())
				{
					TSource candidate = sourceIterator.Current;
					TKey candidateProjected = selector(candidate);

					if (comparer.Compare(candidateProjected, minKey) >= 0)
						continue;

					min = candidate;
					minKey = candidateProjected;
				}

				return min;
			}
		}

		/// <summary>
		/// Returns the minimum value from the sequence, otherwise the default value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static T MinOrDefault<T>([NotNull] this IEnumerable<T> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.MinOrDefault(v => v);
		}

		/// <summary>
		/// Returns the minimum value from the sequence, otherwise the default value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="selector"></param>
		/// <returns></returns>
		public static TValue MinOrDefault<T, TValue>([NotNull] this IEnumerable<T> extends, [NotNull] Func<T, TValue> selector)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (selector == null)
				throw new ArgumentNullException("selector");

			return extends.Select(selector)
			              .AggregateOrDefault((a, b) => Comparer<TValue>.Default.Compare(a, b) < 0 ? a : b);
		}

		/// <summary>
		/// Returns the maximum value from the sequence, otherwise the default value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static T MaxOrDefault<T>([NotNull] this IEnumerable<T> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.MaxOrDefault(v => v);
		}

		/// <summary>
		/// Returns the maximum value from the sequence, otherwise the default value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="selector"></param>
		/// <returns></returns>
		public static TValue MaxOrDefault<T, TValue>([NotNull] this IEnumerable<T> extends, [NotNull] Func<T, TValue> selector)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (selector == null)
				throw new ArgumentNullException("selector");

			return extends.Select(selector)
			              .AggregateOrDefault((a, b) => Comparer<TValue>.Default.Compare(a, b) > 0 ? a : b);
		}

		/// <summary>
		/// Applies an accumulator function over a sequence.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="extends"></param>
		/// <param name="func"></param>
		/// <returns>The final accumulator value.</returns>
		[CanBeNull]
		public static TSource AggregateOrDefault<TSource>([NotNull] this IEnumerable<TSource> extends,
		                                                  [NotNull] Func<TSource, TSource, TSource> func)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (func == null)
				throw new ArgumentNullException("func");

			return extends.AggregateOrDefault(func, default(TSource));
		}

		/// <summary>
		/// Applies an accumulator function over a sequence.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="extends"></param>
		/// <param name="func"></param>
		/// <param name="defaultValue"></param>
		/// <returns>The final accumulator value.</returns>
		[CanBeNull]
		public static TSource AggregateOrDefault<TSource>([NotNull] this IEnumerable<TSource> extends,
		                                                  [NotNull] Func<TSource, TSource, TSource> func,
		                                                  TSource defaultValue)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (func == null)
				throw new ArgumentNullException("func");

			using (IEnumerator<TSource> enumerator = extends.GetEnumerator())
			{
				if (!enumerator.MoveNext())
					return defaultValue;

				TSource result = enumerator.Current;

				while (enumerator.MoveNext())
					result = func(result, enumerator.Current);

				return result;
			}
		}

		/// <summary>
		/// Computes the sum of a sequence of byte values.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static byte Sum([NotNull] this IEnumerable<byte> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return (byte)extends.Select(i => (int)i).Sum();
		}

		/// <summary>
		/// Skips duplicate, consecutive items.
		/// E.g.
		///		[1, 2, 2, 3, 1, 1]
		/// Becomes
		///		[1, 2, 3, 1]
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<T> Consolidate<T>([NotNull] this IEnumerable<T> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return Consolidate(extends, EqualityComparer<T>.Default);
		}

		/// <summary>
		/// Skips duplicate, consecutive items.
		/// E.g.
		///		[1, 2, 2, 3, 1, 1]
		/// Becomes
		///		[1, 2, 3, 1]
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<T> Consolidate<T>([NotNull] this IEnumerable<T> extends,
													[NotNull] IEqualityComparer<T> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			return extends.Consolidate(e => e, comparer);
		}

		/// <summary>
		/// Skips duplicate, consecutive items.
		/// E.g.
		///		[1, 2, 2, 3, 1, 1]
		/// Becomes
		///		[1, 2, 3, 1]
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<TKey> Consolidate<TKey, TValue>([NotNull] this IEnumerable<TKey> extends,
																  [NotNull] Func<TKey, TValue> predicate)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (predicate == null)
				throw new ArgumentNullException("predicate");

			return extends.Consolidate(predicate, EqualityComparer<TValue>.Default);
		}

		/// <summary>
		/// Skips duplicate, consecutive items.
		/// E.g.
		///		[1, 2, 2, 3, 1, 1]
		/// Becomes
		///		[1, 2, 3, 1]
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="predicate"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<TKey> Consolidate<TKey, TValue>([NotNull] this IEnumerable<TKey> extends,
																  [NotNull] Func<TKey, TValue> predicate,
																  [NotNull] IEqualityComparer<TValue> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (predicate == null)
				throw new ArgumentNullException("predicate");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			bool first = true;
			TValue last = default(TValue);

			foreach (TKey item in extends)
			{
				TValue value = predicate(item);
				if (!first && comparer.Equals(last, value))
					continue;

				first = false;
				last = value;

				yield return item;
			}
		}

		/// <summary>
		/// Returns true if all items in the sequence match the predicate.
		/// Returns false if the sequence is empty.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool AnyAndAll<T>([NotNull] this IEnumerable<T> extends, [NotNull] Func<T, bool> predicate)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (predicate == null)
				throw new ArgumentNullException("predicate");

			bool any = false;
			foreach (T item in extends)
			{
				any = true;
				if (!predicate(item))
					return false;
			}

			return any;
		}

		/// <summary>
		/// Combines the corresponding elements of two sequences, producing a sequence of KeyValuePairs.
		/// </summary>
		/// <typeparam name="TFirst"></typeparam>
		/// <typeparam name="TSecond"></typeparam>
		/// <param name="extends"></param>
		/// <param name="second"></param>
		public static IEnumerable<KeyValuePair<TFirst, TSecond>> Zip<TFirst, TSecond>(
			[NotNull] this IEnumerable<TFirst> extends,
			[NotNull] IEnumerable<TSecond> second)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (second == null)
				throw new ArgumentNullException("second");

			return extends.Zip(second, (f, s) => new KeyValuePair<TFirst, TSecond>(f, s));
		}

#if SIMPLSHARP
		/// <summary>
		/// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
		/// </summary>
		/// <typeparam name="TFirst"></typeparam>
		/// <typeparam name="TSecond"></typeparam>
		/// <param name="extends"></param>
		/// <param name="second"></param>
		/// <param name="callback"></param>
		public static void Zip<TFirst, TSecond>([NotNull]this IEnumerable<TFirst> extends,
		                                        [NotNull]IEnumerable<TSecond> second,
		                                        [NotNull]Action<TFirst, TSecond> callback)
		{
			if (extends == null)
				throw new ArgumentNullException("first");

			if (second == null)
				throw new ArgumentNullException("second");

			if (callback == null)
				throw new ArgumentNullException("callback");

			using (IEnumerator<TFirst> enumerator1 = extends.GetEnumerator())
			{
				using (IEnumerator<TSecond> enumerator2 = second.GetEnumerator())
				{
					while (enumerator1.MoveNext() && enumerator2.MoveNext())
						callback(enumerator1.Current, enumerator2.Current);
				}
			}
		}

		/// <summary>
		/// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
		/// </summary>
		/// <typeparam name="TFirst"></typeparam>
		/// <typeparam name="TSecond"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="extends"></param>
		/// <param name="other"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>([NotNull]this IEnumerable<TFirst> extends,
		                                                                 [NotNull]IEnumerable<TSecond> other,
		                                                                 [NotNull]Func<TFirst, TSecond, TResult> callback)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (other == null)
				throw new ArgumentNullException("other");

			if (callback == null)
				throw new ArgumentNullException("callback");

			return ZipIterator(extends, other, callback);
		}

		/// <summary>
		/// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
		/// </summary>
		/// <typeparam name="TFirst"></typeparam>
		/// <typeparam name="TSecond"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		private static IEnumerable<TResult> ZipIterator<TFirst, TSecond, TResult>([NotNull]IEnumerable<TFirst> first,
		                                                                          [NotNull]IEnumerable<TSecond> second,
		                                                                          [NotNull]Func<TFirst, TSecond, TResult> callback)
		{
			using (IEnumerator<TFirst> enumerator1 = first.GetEnumerator())
			{
				using (IEnumerator<TSecond> enumerator2 = second.GetEnumerator())
				{
					while (enumerator1.MoveNext() && enumerator2.MoveNext())
						yield return callback(enumerator1.Current, enumerator2.Current);
				}
			}
		}
#endif
	}
}
