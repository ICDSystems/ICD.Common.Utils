using System;
using System.Collections.Generic;

namespace ICD.Common.Utils
{
	public static class RecursionUtils
	{
		/// <summary>
		/// Returns all of the nodes in the tree via breadth-first search.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="root"></param>
		/// <param name="getChildren"></param>
		/// <returns></returns>
		public static IEnumerable<T> BreadthFirstSearch<T>(T root, Func<T, IEnumerable<T>> getChildren)
		{
			Queue<T> process = new Queue<T>();
			process.Enqueue(root);

			while (process.Count > 0)
			{
				T current = process.Dequeue();
				yield return current;

				foreach (T child in getChildren(current))
					process.Enqueue(child);
			}
		}
	}
}
