using System;
using System.Collections.Generic;
using ICD.Common.Properties;

namespace ICD.Common.Utils.Extensions
{
	public static class CollectionExtensions
	{
		/// <summary>
		/// Clears the collection and adds the given range of items.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="range"></param>
		public static void SetRange<T>([NotNull] this ICollection<T> extends, [NotNull] IEnumerable<T> range)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (range == null)
				throw new ArgumentNullException("range");

			extends.Clear();
			extends.AddRange(range);
		}

		/// <summary>
		/// Adds the given range of items to the collection.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="range"></param>
		public static void AddRange<T>([NotNull] this ICollection<T> extends, [NotNull] IEnumerable<T> range)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (range == null)
				throw new ArgumentNullException("range");

			foreach (T item in range)
				extends.Add(item);
		}
	}
}
