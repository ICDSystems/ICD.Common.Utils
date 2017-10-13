using System;
using System.Collections.Generic;
using System.Linq;

namespace ICD.Common.Utils.Extensions
{
	/// <summary>
	/// Extension methods for working with ICollections.
	/// </summary>
	public static class CollectionExtensions
	{
		/// <summary>
		/// Removes all of the items from the other collection.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="other"></param>
		public static void RemoveAll<T>(this ICollection<T> extends, IEnumerable<T> other)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (other == null)
				throw new ArgumentNullException("other");

			extends.RemoveAll(i => other.Contains(i));
		}

		/// <summary>
		/// Removes items matching the predicate.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="predicate"></param>
		public static void RemoveAll<T>(this ICollection<T> extends, Func<T, bool> predicate)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (predicate == null)
				throw new ArgumentNullException("predicate");

			foreach (T item in extends.Where(predicate).ToArray())
				extends.Remove(item);
		}
	}
}
