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
		[NotNull]
		public static IEnumerable<IEnumerable<T>> GetCliques<T>([NotNull] IEnumerable<T> nodes,
		                                                        [NotNull] Func<T, IEnumerable<T>> getAdjacent)
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
		/// <param name="node"></param>
		/// <param name="getAdjacent"></param>
		/// <returns></returns>
		[NotNull]
		public static IEnumerable<T> GetClique<T>([NotNull] T node, [NotNull] Func<T, IEnumerable<T>> getAdjacent)
		{
			if (node == null)
				throw new ArgumentNullException("node");

			if (getAdjacent == null)
				throw new ArgumentNullException("getAdjacent");

			return BreadthFirstSearch(node, getAdjacent);
		}

		/// <summary>
		/// Gets the clique containing the given node.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="map"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		[NotNull]
		public static IEnumerable<T> GetClique<T>([NotNull] IDictionary<T, IEnumerable<T>> map, [NotNull] T node)
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
		private static IEnumerable<T> GetClique<T>([NotNull] IDictionary<T, IEnumerable<T>> map,
		                                           [NotNull] IcdHashSet<T> visited, [NotNull] T node)
		{
			if (map == null)
				throw new ArgumentNullException("map");

			if (visited == null)
				throw new ArgumentNullException("visited");

// ReSharper disable once CompareNonConstrainedGenericWithNull
			if (node == null)
				throw new ArgumentNullException("node");

			if (!visited.Add(node))
				yield break;

			IEnumerable<T> adjacent;
			if (!map.TryGetValue(node, out adjacent))
				yield break;

			yield return node;

			foreach (T item in adjacent.SelectMany(a => GetClique(map, visited, a)))
				yield return item;
		}

		/// <summary>
		/// Returns true if there is a path from the given root to the given destination node.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="root"></param>
		/// <param name="destination"></param>
		/// <param name="getChildren"></param>
		/// <returns></returns>
		public static bool BreadthFirstSearch<T>([NotNull] T root, [NotNull] T destination,
		                                         [NotNull] Func<T, IEnumerable<T>> getChildren)
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
			if (root == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("root");

// ReSharper disable CompareNonConstrainedGenericWithNull
			if (destination == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("destination");

			if (getChildren == null)
				throw new ArgumentNullException("getChildren");

			return BreadthFirstSearch(root, destination, getChildren, EqualityComparer<T>.Default);
		}

		/// <summary>
		/// Returns true if there is a path from the given root to the given destination node.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="root"></param>
		/// <param name="destination"></param>
		/// <param name="getChildren"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static bool BreadthFirstSearch<T>([NotNull] T root, [NotNull] T destination,
		                                         [NotNull] Func<T, IEnumerable<T>> getChildren,
		                                         [NotNull] IEqualityComparer<T> comparer)
		{
			// ReSharper disable CompareNonConstrainedGenericWithNull
			if (root == null)
			// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("root");

			// ReSharper disable CompareNonConstrainedGenericWithNull
			if (destination == null)
			// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("destination");

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
		public static IEnumerable<T> BreadthFirstSearch<T>([NotNull] T root, [NotNull] Func<T, IEnumerable<T>> getChildren)
		{
			// ReSharper disable CompareNonConstrainedGenericWithNull
			if (root == null)
			// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("root");

			if (getChildren == null)
				throw new ArgumentNullException("getChildren");

			return BreadthFirstSearchPaths(root, getChildren, EqualityComparer<T>.Default).Select(kvp => kvp.Key);
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
		public static IEnumerable<T> BreadthFirstSearchPath<T>([NotNull] T root, [NotNull] T destination,
		                                                       [NotNull] Func<T, IEnumerable<T>> getChildren)
		{
			// ReSharper disable CompareNonConstrainedGenericWithNull
			if (root == null)
			// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("root");

			// ReSharper disable CompareNonConstrainedGenericWithNull
			if (destination == null)
			// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("destination");

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
		public static IEnumerable<T> BreadthFirstSearchPath<T>([NotNull] T root, [NotNull] T destination,
		                                                       [NotNull] Func<T, IEnumerable<T>> getChildren,
		                                                       [NotNull] IEqualityComparer<T> comparer)
		{
			// ReSharper disable CompareNonConstrainedGenericWithNull
			if (root == null)
			// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("root");

			// ReSharper disable CompareNonConstrainedGenericWithNull
			if (destination == null)
			// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("destination");

			if (getChildren == null)
				throw new ArgumentNullException("getChildren");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			return BreadthFirstSearchPathManyDestinations(root, destination.Yield(), getChildren, comparer)
				.Select(kvp => kvp.Value)
				.FirstOrDefault();
		}

		/// <summary>
		/// Returns the shortest path from root to each destination via breadth-first search.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="root"></param>
		/// <param name="destinations"></param>
		/// <param name="getChildren"></param>
		/// <returns></returns>
		[NotNull]
		public static IEnumerable<KeyValuePair<T, IEnumerable<T>>>
			BreadthFirstSearchPathManyDestinations<T>([NotNull] T root, [NotNull] IEnumerable<T> destinations,
			                                          [NotNull] Func<T, IEnumerable<T>> getChildren)
		{
			// ReSharper disable CompareNonConstrainedGenericWithNull
			if (root == null)
			// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("root");

			if (destinations == null)
				throw new ArgumentNullException("destinations");

			if (getChildren == null)
				throw new ArgumentNullException("getChildren");

			return BreadthFirstSearchPathManyDestinations(root, destinations, getChildren, EqualityComparer<T>.Default);
		}

		/// <summary>
		/// Returns the shortest path from root to each destination via breadth-first search.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="root"></param>
		/// <param name="destinations"></param>
		/// <param name="getChildren"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		[NotNull]
		public static IEnumerable<KeyValuePair<T, IEnumerable<T>>>
			BreadthFirstSearchPathManyDestinations<T>([NotNull] T root, [NotNull] IEnumerable<T> destinations,
			                                          [NotNull] Func<T, IEnumerable<T>> getChildren,
			                                          [NotNull] IEqualityComparer<T> comparer)
		{
			// ReSharper disable CompareNonConstrainedGenericWithNull
			if (root == null)
			// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("root");

			if (destinations == null)
				throw new ArgumentNullException("destinations");

			if (getChildren == null)
				throw new ArgumentNullException("getChildren");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			IcdHashSet<T> destinationsSet = destinations as IcdHashSet<T> ?? destinations.ToIcdHashSet();
			return BreadthFirstSearchPaths(root, getChildren, comparer)
				.Where(kvp => destinationsSet.Contains(kvp.Key))
				.Take(destinationsSet.Count);
		}

		/// <summary>
		/// Returns the shortest path from root to each destination via breadth-first search.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="root"></param>
		/// <param name="getChildren"></param>
		/// <returns></returns>
		[NotNull]
		public static IEnumerable<KeyValuePair<T, IEnumerable<T>>>
			BreadthFirstSearchPaths<T>([NotNull] T root, [NotNull] Func<T, IEnumerable<T>> getChildren)
		{
			// ReSharper disable CompareNonConstrainedGenericWithNull
			if (root == null)
			// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("root");

			if (getChildren == null)
				throw new ArgumentNullException("getChildren");

			return BreadthFirstSearchPaths(root, getChildren, EqualityComparer<T>.Default);
		}

		/// <summary>
		/// Returns the shortest path from root to each destination via breadth-first search.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="root"></param>
		/// <param name="getChildren"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		[NotNull]
		public static IEnumerable<KeyValuePair<T, IEnumerable<T>>>
			BreadthFirstSearchPaths<T>([NotNull] T root, [NotNull] Func<T, IEnumerable<T>> getChildren,
			                           [NotNull] IEqualityComparer<T> comparer)
		{
			// ReSharper disable CompareNonConstrainedGenericWithNull
			if (root == null)
			// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("root");

			if (getChildren == null)
				throw new ArgumentNullException("getChildren");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			// Edge case - root is the same as destination
			yield return new KeyValuePair<T, IEnumerable<T>>(root, root.Yield());

			Queue<T> queue = new Queue<T>();
			queue.Enqueue(root);

			Dictionary<T, T> nodeParents = new Dictionary<T, T>(comparer)
			{
				{root, default(T)}
			};

			while (queue.Count > 0)
			{
				T current = queue.Dequeue();

				foreach (T node in getChildren(current).Where(node => !nodeParents.ContainsKey(node)))
				{
					queue.Enqueue(node);
					nodeParents.Add(node, current);

					yield return
						new KeyValuePair<T, IEnumerable<T>>(node, GetPath(node, root, nodeParents, comparer).Reverse());
				}
			}
		}

		/// <summary>
		/// Walks through a map of nodes from the starting point.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="nodeParents"></param>
		/// <returns></returns>
		[NotNull]
		public static IEnumerable<T> GetPath<T>([NotNull] T start, [NotNull] T end, [NotNull] IDictionary<T, T> nodeParents)
		{
			// ReSharper disable CompareNonConstrainedGenericWithNull
			if (start == null)
				// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("start");

			// ReSharper disable CompareNonConstrainedGenericWithNull
			if (end == null)
				// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("end");

			if (nodeParents == null)
				throw new ArgumentNullException("nodeParents");

			return GetPath(start, end, nodeParents, EqualityComparer<T>.Default);
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
		[NotNull]
		public static IEnumerable<T> GetPath<T>([NotNull] T start, [NotNull] T end, [NotNull] IDictionary<T, T> nodeParents,
		                                         [NotNull] IEqualityComparer<T> comparer)
		{
			// ReSharper disable CompareNonConstrainedGenericWithNull
			if (start == null) 
			// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("start");

			// ReSharper disable CompareNonConstrainedGenericWithNull
			if (end == null)
			// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("end");

			if (nodeParents == null)
				throw new ArgumentNullException("nodeParents");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			IcdHashSet<T> visited = new IcdHashSet<T>();

			while (true)
			{
				yield return start;

				if (comparer.Equals(start, end))
					break;

				visited.Add(start);

				T next;
				// ReSharper disable CompareNonConstrainedGenericWithNull
				if (!nodeParents.TryGetValue(start, out next) || next == null)
				// ReSharper restore CompareNonConstrainedGenericWithNull
					throw new InvalidOperationException("No path");

				if (visited.Contains(next))
					break;

				start = next;
			}
		}

		/// <summary>
		/// Returns all of the nodes in the tree via depth-first search.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="root"></param>
		/// <param name="getChildren"></param>
		/// <returns></returns>
		[NotNull]
		public static IEnumerable<T> DepthFirstSearch<T>([NotNull] T root, [NotNull] Func<T, IEnumerable<T>> getChildren)
		{
			if (root == null)
				throw new ArgumentNullException("root");

			if (getChildren == null)
				throw new ArgumentNullException("getChildren");

			IcdHashSet<T> visited = new IcdHashSet<T>();
			Stack<T> stack = new Stack<T>();

			stack.Push(root);

			while (stack.Count != 0)
			{
				T current = stack.Pop();
				if (!visited.Add(current))
					continue;

				yield return current;

				foreach (T child in getChildren(current).Reverse())
					stack.Push(child);
			}
		}
	}
}
