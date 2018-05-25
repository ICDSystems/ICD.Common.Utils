using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils
{
	public static class RecursionUtils
	{
		/// <summary>
		/// Returns a sequence of the cliques in the graph.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="nodes"></param>
		/// <param name="getAdjacent"></param>
		/// <returns></returns>
		public static IEnumerable<IEnumerable<T>> GetCliques<T>(IEnumerable<T> nodes, Func<T, IEnumerable<T>> getAdjacent)
		{
			if (nodes == null)
				throw new ArgumentNullException("nodes");

			if (getAdjacent == null)
				throw new ArgumentNullException("getAdjacent");

			Dictionary<T, IEnumerable<T>> map = nodes.ToDictionary(n => n, getAdjacent);
			IcdHashSet<T> visited = new IcdHashSet<T>();

			return map.Keys
			          .Where(n => !visited.Contains(n))
			          .Select(node => GetClique(map, visited, node));
		}

		/// <summary>
		/// Gets the clique containing the given node.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="map"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetClique<T>(IDictionary<T, IEnumerable<T>> map, T node)
		{
			if (map == null)
				throw new ArgumentNullException("map");

// ReSharper disable once CompareNonConstrainedGenericWithNull
			if (node == null)
				throw new ArgumentNullException("node");

			return GetClique(map, new IcdHashSet<T>(), node);
		}

		/// <summary>
		/// Gets the clique containing the node.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="map"></param>
		/// <param name="visited"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		private static IEnumerable<T> GetClique<T>(IDictionary<T, IEnumerable<T>> map, IcdHashSet<T> visited, T node)
		{
			if (map == null)
				throw new ArgumentNullException("map");

			if (visited == null)
				throw new ArgumentNullException("visited");

// ReSharper disable once CompareNonConstrainedGenericWithNull
			if (node == null)
				throw new ArgumentNullException("node");

			if (visited.Contains(node))
				yield break;

			if (!map.ContainsKey(node))
				yield break;

			visited.Add(node);

			yield return node;

			IEnumerable<T> adjacent = map.GetDefault(node, Enumerable.Empty<T>());

			foreach (T item in adjacent.SelectMany(a => GetClique(map, visited, a)))
				yield return item;
		}

		/// <summary>
		/// Returns true if there is a path from the given root to the given child node.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="root"></param>
		/// <param name="child"></param>
		/// <param name="getChildren"></param>
		/// <returns></returns>
		public static bool BreadthFirstSearch<T>(T root, T child, Func<T, IEnumerable<T>> getChildren)
		{
			if (getChildren == null)
				throw new ArgumentNullException("getChildren");

			return BreadthFirstSearchPath(root, child, getChildren) != null;
		}

		/// <summary>
		/// Returns true if there is a path from the given root to the given child node.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="root"></param>
		/// <param name="destination"></param>
		/// <param name="getChildren"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static bool BreadthFirstSearch<T>(T root, T destination, Func<T, IEnumerable<T>> getChildren, IEqualityComparer<T> comparer)
		{
			if (getChildren == null)
				throw new ArgumentNullException("getChildren");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			return BreadthFirstSearchPath(root, destination, getChildren, comparer) != null;
		}

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

			// Edge case - root and destination are the same
			if (comparer.Equals(root, destination))
				return new[] {root};

			Queue<T> queue = new Queue<T>();
			queue.Enqueue(root);

			Dictionary<T, T> nodeParents = new Dictionary<T, T>(comparer);

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
						return GetPath(destination, root, nodeParents, comparer).Reverse();
				}
			}

			return null;
		}

		[NotNull]
		public static Dictionary<T, IEnumerable<T>> BreadthFirstSearchManyDestinations<T>(T root,
		                                                                                  IEnumerable<T> destinations,
		                                                                                  Func<T, IEnumerable<T>>
			                                                                                  getChildren)
		{
			if (getChildren == null)
				throw new ArgumentNullException("getChildren");

			return BreadthFirstSearchPathManyDestinations(root, destinations, getChildren, EqualityComparer<T>.Default);
		}

		[NotNull]
		public static Dictionary<T, IEnumerable<T>> BreadthFirstSearchPathManyDestinations<T>(T root,
		                                                                                      IEnumerable<T>
			                                                                                      destinations,
		                                                                                      Func<T, IEnumerable<T>>
			                                                                                      getChildren,
		                                                                                      IEqualityComparer<T>
			                                                                                      comparer)
		{
			if (destinations == null)
				throw new ArgumentNullException("destinations");

			if (getChildren == null)
				throw new ArgumentNullException("getChildren");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			IcdHashSet<T> destinationsToBeProcessed = new IcdHashSet<T>(destinations);
			IcdHashSet<T> destinationsProcessed = new IcdHashSet<T>();
			Dictionary<T, IEnumerable<T>> pathsToReturn = new Dictionary<T, IEnumerable<T>>();

			// Edge case, root is the destination
			foreach (T destination in
				destinationsToBeProcessed.Where(destination => comparer.Equals(root, destination)))
			{
				destinationsProcessed.Add(destination);
				pathsToReturn.Add(destination, new[] {root});
			}

			foreach (T destination in destinationsProcessed)
				destinationsToBeProcessed.Remove(destination);
			destinationsProcessed.Clear();
			if (destinationsToBeProcessed.Count == 0)
				return pathsToReturn;

			Queue<T> queue = new Queue<T>();
			queue.Enqueue(root);

			Dictionary<T, T> nodeParents = new Dictionary<T, T>(comparer);

			while (queue.Count > 0)
			{
				T current = queue.Dequeue();

				foreach (T node in getChildren(current).Where(node => !nodeParents.ContainsKey(node)))
				{
					queue.Enqueue(node);
					nodeParents.Add(node, current);

					T closureNode = node;
					foreach (T destination in
						destinationsToBeProcessed.Where(destination => comparer.Equals(closureNode, destination)))
					{
						destinationsProcessed.Add(destination);
						pathsToReturn.Add(destination, GetPath(destination, root, nodeParents, comparer).Reverse());
					}

					foreach (T destination in destinationsProcessed)
						destinationsToBeProcessed.Remove(destination);

					destinationsProcessed.Clear();

					if (destinationsToBeProcessed.Count == 0)
						return pathsToReturn;
				}
			}

			return pathsToReturn;
		}

		/// <summary>
		/// Walks through a map of nodes from the starting point.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="nodeParents"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		private static IEnumerable<T> GetPath<T>(T start, T end, IDictionary<T, T> nodeParents, IEqualityComparer<T> comparer)
		{
			IcdHashSet<T> visited = new IcdHashSet<T>();

			while (true)
			{
				yield return start;

				if (comparer.Equals(start, end))
					break;

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
