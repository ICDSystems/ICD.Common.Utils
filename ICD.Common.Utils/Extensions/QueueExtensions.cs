using System;
using System.Collections.Generic;
using ICD.Common.Properties;

namespace ICD.Common.Utils.Extensions
{
	public static class QueueExtensions
	{
		/// <summary>
		/// Enqueues each item in the sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="items"></param>
		public static void EnqueueRange<T>([NotNull] this Queue<T> extends, [NotNull] IEnumerable<T> items)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (items == null)
				throw new ArgumentNullException("items");

			foreach (T item in items)
				extends.Enqueue(item);
		}

		/// <summary>
		/// Dequeues the next item in the queue. Returns false if the queue is empty.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public static bool Dequeue<T>([NotNull] this Queue<T> extends, out T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			item = default(T);

			if (extends.Count == 0)
				return false;

			item = extends.Dequeue();
			return true;
		}

		/// <summary>
		/// Peeks the next item in the queue. Returns false if the queue is empty.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public static bool Peek<T>([NotNull] this Queue<T> extends, out T item)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			item = default(T);

			if (extends.Count == 0)
				return false;

			item = extends.Peek();
			return true;
		}
	}
}
