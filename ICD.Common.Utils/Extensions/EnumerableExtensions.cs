using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Collections;

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
		public static T FirstOrDefault<T>(this IEnumerable<T> extends, T defaultItem)
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
		public static T FirstOrDefault<T>(this IEnumerable<T> extends, Func<T, bool> predicate, T defaultItem)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (predicate == null)
				throw new ArgumentNullException("predicate");

			foreach (T item in extends.Where(predicate))
				return item;
			return defaultItem;
		}

		/// <summary>
		/// Returns true if there is at least 1 item in the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item">Outputs the first item in the sequence.</param>
		/// <returns></returns>
		public static bool TryFirst<T>(this IEnumerable<T> extends, out T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.TryFirst(i => true, out item);
		}

		/// <summary>
		/// Returns true if there is at least 1 item in the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="predicate"></param>
		/// <param name="item">Outputs the first item in the sequence.</param>
		/// <returns></returns>
		public static bool TryFirst<T>(this IEnumerable<T> extends, Func<T, bool> predicate, out T item)
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
		public static bool TryLast<T>(this IEnumerable<T> extends, out T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.TryLast(i => true, out item);
		}

		/// <summary>
		/// Returns true if there is at least 1 item in the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="predicate"></param>
		/// <param name="item">Outputs the last item in the sequence.</param>
		/// <returns></returns>
		public static bool TryLast<T>(this IEnumerable<T> extends, Func<T, bool> predicate, out T item)
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
		public static bool TryElementAt<T>(this IEnumerable<T> extends, int index, out T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (index < 0)
				throw new ArgumentException("Index must be greater or equal to 0", "index");

			item = default(T);
			int eachIndex = 0;

			foreach (T each in extends)
			{
				if (eachIndex == index)
				{
					item = each;
					return true;
				}

				eachIndex++;
			}

			return false;
		}

		/// <summary>
		/// Compares the two sequences for identical values.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="other"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static bool SequenceEqual<T>(this IEnumerable<T> extends, IEnumerable<T> other, Func<T, T, bool> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (extends == null)
				throw new ArgumentNullException("other");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			using (IEnumerator<T> firstPos = extends.GetEnumerator())
			{
				using (IEnumerator<T> secondPos = other.GetEnumerator())
				{
					bool hasFirst = firstPos.MoveNext();
					bool hasSecond = secondPos.MoveNext();

					while (hasFirst && hasSecond)
					{
						if (!comparer(firstPos.Current, secondPos.Current))
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
		public static bool ScrambledEquals<T>(this IEnumerable<T> extends, IEnumerable<T> other)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (extends == null)
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
		public static bool ScrambledEquals<T>(this IEnumerable<T> extends, IEnumerable<T> other, IEqualityComparer<T> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (extends == null)
				throw new ArgumentNullException("other");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			Dictionary<T, int> count = new Dictionary<T, int>(comparer);

			foreach (T item in extends)
			{
				if (count.ContainsKey(item))
					count[item]++;
				else
					count.Add(item, 1);
			}

			foreach (T item in other)
			{
				if (count.ContainsKey(item))
					count[item]--;
				else
					return false;
			}

			return count.Values.All(c => c == 0);
		}

		/// <summary>
		/// Returns the index that matches the predicate.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="match"></param>
		/// <returns></returns>
		public static int FindIndex<T>(this IEnumerable<T> extends, Predicate<T> match)
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
		public static IEnumerable<int> FindIndices<T>(this IEnumerable<T> extends, Predicate<T> match)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (match == null)
				throw new ArgumentNullException("match");

			int index = 0;

			foreach (T item in extends)
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
		public static IEnumerable<TResult> SelectMulti<TSource, TResult>(this IEnumerable<TSource> extends,
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
		public static void Execute<T>(this IEnumerable<T> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			extends.ForEach(item => { });
		}

		/// <summary>
		/// Performs the action for each item in the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="action"></param>
		[PublicAPI]
		public static void ForEach<T>(this IEnumerable<T> extends, Action<T> action)
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
		public static void ForEach<T>(this IEnumerable<T> extends, Action<T, int> action)
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
		public static IEnumerable<T> Prepend<T>(this IEnumerable<T> extends, T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			yield return item;
			foreach (T next in extends)
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
		public static IEnumerable<T> PrependMany<T>(this IEnumerable<T> extends, params T[] items)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (items == null)
				throw new ArgumentNullException("items");

			foreach (T item in items)
				yield return item;
			foreach (T each in extends)
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
		public static IEnumerable<T> Append<T>(this IEnumerable<T> extends, T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			foreach (T first in extends)
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
		public static IEnumerable<T> AppendMany<T>(this IEnumerable<T> extends, params T[] items)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (items == null)
				throw new ArgumentNullException("items");

			foreach (T each in extends)
				yield return each;
			foreach (T item in items)
				yield return item;
		}

		/// <summary>
		/// Default ordering for the items in the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static IEnumerable<T> Order<T>(this IEnumerable<T> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.OrderBy(i => i);
		}

		/// <summary>
		/// Returns every item in the sequence except the given item.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public static IEnumerable<T> Except<T>(this IEnumerable<T> extends, T item)
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
		public static IEnumerable<T> Except<T>(this IEnumerable<T> extends, T item, IEqualityComparer<T> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			return extends.Where(i => !comparer.Equals(item, i));
		}

		/// <summary>
		/// Returns the sequence as a IcdHashSet.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static IcdHashSet<T> ToIcdHashSet<T>(this IEnumerable<T> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return new IcdHashSet<T>(extends);
		}

		/// <summary>
		/// Optimized ToArray implementation with fewer allocations.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static T[] ToArray<T>(this IEnumerable<T> extends, int count)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (count < 0)
				throw new ArgumentOutOfRangeException("count");

			T[] array = new T[count];
			int i = 0;

			foreach (T item in extends)
				array[i++] = item;
			return array;
		}

		/// <summary>
		/// Optimized ToList implementation with fewer allocations.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static List<T> ToList<T>(this IEnumerable<T> extends, int count)
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
		public static Dictionary<int, T> ToDictionary<T>(this IEnumerable<T> extends)
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
		public static Dictionary<uint, T> ToDictionaryUInt<T>(this IEnumerable<T> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			Dictionary<uint, T> output = new Dictionary<uint, T>();
			extends.ForEach((item, index) => output.Add((uint)index, item));
			return output;
		}

		/// <summary>
		/// Turns an enumerable of KeyValuePairs back into a dictionary
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.ToDictionary(x => x.Key, x => x.Value);
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
		public static T Unanimous<T>(this IEnumerable<T> extends, T other)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			T[] array = extends.Distinct().ToArray();
			return array.Length == 1 ? array[0] : other;
		}

		/// <summary>
		/// Returns false if the sequence is empty.
		/// Returns false if the sequence is non-empty and there are two different elements.
		/// Returns true if the sequence is non-empty and all elements are the same.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool Unanimous<T>(this IEnumerable<T> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.Distinct().Count() == 1;
		}

		/// <summary>
		/// Partitions a sequence into sequences of the given length.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="partitionSize"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> extends, int partitionSize)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			using (IEnumerator<T> enumerator = extends.GetEnumerator())
			{
				while (enumerator.MoveNext())
					yield return YieldBatchElements(enumerator, partitionSize - 1);
			}
		}

		private static IEnumerable<T> YieldBatchElements<T>(IEnumerator<T> source, int partitionSize)
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
		/// Given a sequence [A, B, C] returns a sequence [[A, B], [B, C]]
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static IEnumerable<T[]> GetAdjacentPairs<T>(this IEnumerable<T> extends)
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
		public static T GetClosest<T>(this IEnumerable<T> extends, Func<T, int> getDelta)
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
		public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
		                                           Func<TSource, TKey> selector)
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
		public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector,
		                                           IComparer<TKey> comparer)
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
		/// Removes any null elements from an enumerable of nullable value types
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<T> ExceptNulls<T>(this IEnumerable<T?> extends)
			where T : struct
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.OfType<T>();
		}

		/// <summary>
		/// Computes the sum of a sequence of byte values.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static byte Sum(this IEnumerable<byte> extends)
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
		public static IEnumerable<T> Consolidate<T>(this IEnumerable<T> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return Consolidate(extends, Comparer<T>.Default);
		}

		///  <summary>
		///  Skips duplicate, consecutive items.
		///  E.g.
		/// 		[1, 2, 2, 3, 1, 1]
		///  Becomes
		/// 		[1, 2, 3, 1]
		///  </summary>
		///  <typeparam name="T"></typeparam>
		///  <param name="extends"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<T> Consolidate<T>(this IEnumerable<T> extends, IComparer<T> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			bool first = true;
			T last = default(T);

			foreach (T item in extends)
			{
				if (!first && comparer.Compare(last, item) == 0)
					continue;

				first = false;
				last = item;
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
		public static bool AnyAndAll<T>(this IEnumerable<T> extends, Func<T, bool> predicate)
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
	}
}
