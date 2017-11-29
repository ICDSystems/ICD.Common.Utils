using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Collections;

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
			if (getChildren == null)
				throw new ArgumentNullException("getChildren");

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

		/// <summary>
		/// Returns the shortest path from root to destination via breadth-first search.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="root"></param>
		/// <param name="destination"></param>
		/// <param name="getChildren"></param>
		/// <returns></returns>
		[CanBeNull]
		public static IEnumerable<T> BreadthFirstSearchPath<T>(T root, T destination, Func<T, IEnumerable<T>> getChildren)
		{
			if (getChildren == null)
				throw new ArgumentNullException("getChildren");

			return BreadthFirstSearchPath(root, destination, getChildren, EqualityComparer<T>.Default);
		}

		/// <summary>
		/// Returns the shortest path from root to destination via breadth-first search.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="root"></param>
		/// <param name="destination"></param>
		/// <param name="getChildren"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		[CanBeNull]
		public static IEnumerable<T> BreadthFirstSearchPath<T>(T root, T destination, Func<T, IEnumerable<T>> getChildren,
		                                                       IEqualityComparer<T> comparer)
		{
			if (getChildren == null)
				throw new ArgumentNullException("getChildren");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			Queue<T> queue = new Queue<T>();
			queue.Enqueue(root);

			Dictionary<T, T> nodeParents = new Dictionary<T, T>();

			while (queue.Count > 0)
			{
				T current = queue.Dequeue();

				foreach (T node in getChildren(current))
				{
					if (nodeParents.ContainsKey(node))
						continue;

					queue.Enqueue(node);
					nodeParents.Add(node, current);

					// Found a path to the destination
					if (comparer.Equals(node, destination))
						return GetPath(destination, nodeParents).Reverse();
				}
			}

			return null;
		}

		/// <summary>
		/// Walks through a map of nodes from the starting point.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="start"></param>
		/// <param name="nodeParents"></param>
		/// <returns></returns>
		private static IEnumerable<T> GetPath<T>(T start, IDictionary<T, T> nodeParents)
		{
			IcdHashSet<T> visited = new IcdHashSet<T>();

			while (true)
			{
				yield return start;
				visited.Add(start);

				T next;
				if (!nodeParents.TryGetValue(start, out next))
					break;

				if (visited.Contains(next))
					break;

				start = next;
			}
		}
	}
}
