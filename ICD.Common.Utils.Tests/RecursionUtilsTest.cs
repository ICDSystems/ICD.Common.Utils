using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICD.Common.Utils.Tests
{
	[TestFixture]
	public sealed class RecursionUtilsTest
	{
		private static IEnumerable<int> Graph(int node)
		{
			switch (node)
			{
				case 1:
					yield return 2;
					yield return 3;
					break;

				case 2:
					yield return 4;
					break;

				default:
					yield break;
			}
		}

		private static IEnumerable<int> WideGraph(int node)
		{
			switch (node)
			{
				case 1:
					yield return 2;
					yield return 3;
					yield return 4;
					yield return 5;
					yield return 6;
					break;
				case 2:
					yield return 21;
					yield return 22;
					break;
				case 3:
					yield return 1;
					yield return 31;
					yield return 32;
					break;
				case 4:
					yield return 41;
					yield return 42;
					break;
				case 5:
					yield return 51;
					yield return 52;
					break;
				case 6:
					yield return 61;
					yield return 62;
					break;
				case 21:
					yield return 62;
					break;
				case 41:
					yield return 43;
					break;
				case 42:
					yield return 43;
					break;
				default:
					yield break;
			}
		}

		private static readonly Dictionary<int, IEnumerable<int>> s_CliqueGraph = new Dictionary<int, IEnumerable<int>>
		{
			{1, new[] {2, 3}},
			{2, new[] {1, 4}},
			{3, new[] {1}},
			{4, new[] {2}},
			{5, new[] {6}},
			{6, new[] {5}}
		};

		[Test]
		public void GetCliquesTest()
		{
			int[][] cliques =
				RecursionUtils.GetCliques(s_CliqueGraph.Keys, i => s_CliqueGraph[i])
				              .Select(c => c.ToArray())
				              .ToArray();

			Assert.AreEqual(2, cliques.Length);

			int[] clique1 = cliques.FirstOrDefault(c => c.Contains(1));
			int[] clique2 = cliques.FirstOrDefault(c => c != clique1);

			Assert.NotNull(clique1);
			Assert.NotNull(clique2);

			Assert.AreEqual(4, clique1.Length);
			Assert.IsTrue(clique1.Contains(1));
			Assert.IsTrue(clique1.Contains(2));
			Assert.IsTrue(clique1.Contains(3));
			Assert.IsTrue(clique1.Contains(4));

			Assert.AreEqual(2, clique2.Length);
			Assert.IsTrue(clique2.Contains(5));
			Assert.IsTrue(clique2.Contains(6));
		}

		[Test]
		public void GetCliqueTest()
		{
			int[] clique = RecursionUtils.GetClique(s_CliqueGraph, 1).ToArray();

			Assert.AreEqual(4, clique.Length);
			Assert.IsTrue(clique.Contains(1));
			Assert.IsTrue(clique.Contains(2));
			Assert.IsTrue(clique.Contains(3));
			Assert.IsTrue(clique.Contains(4));

			clique = RecursionUtils.GetClique(s_CliqueGraph, 5).ToArray();

			Assert.AreEqual(2, clique.Length);
			Assert.IsTrue(clique.Contains(5));
			Assert.IsTrue(clique.Contains(6));
		}

		[Test]
		public void BreadthFirstSearchTest()
		{
			// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
			Assert.Throws<ArgumentNullException>(() => RecursionUtils.BreadthFirstSearch(1, null).ToArray());

			int[] nodes = RecursionUtils.BreadthFirstSearch(1, Graph).ToArray();

			Assert.AreEqual(4, nodes.Length);
			Assert.AreEqual(1, nodes[0]);
			Assert.AreEqual(2, nodes[1]);
			Assert.AreEqual(3, nodes[2]);
			Assert.AreEqual(4, nodes[3]);
		}

		[Test]
		public void BreadthFirstSearchPathTest()
		{
			Assert.Throws<ArgumentNullException>(() => RecursionUtils.BreadthFirstSearchPath(1, 4, null));

			Assert.IsNull(RecursionUtils.BreadthFirstSearchPath(1, 5, Graph));

			// ReSharper disable once AssignNullToNotNullAttribute
			int[] path = RecursionUtils.BreadthFirstSearchPath(1, 4, Graph).ToArray();

			Assert.AreEqual(3, path.Length);
			Assert.AreEqual(1, path[0]);
			Assert.AreEqual(2, path[1]);
			Assert.AreEqual(4, path[2]);

			IEnumerable<int> noPath = RecursionUtils.BreadthFirstSearchPath(3, 4, Graph);

			Assert.IsNull(noPath);
		}

		/// <summary>
		/// Test to ensure that when start and end node are the same, breadth first search returns that single node.
		/// </summary>
		[Test]
		public void BreadthFirstSearchPathSingleNodeTest()
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			int[] path = RecursionUtils.BreadthFirstSearchPath(1, 1, Graph).ToArray();

			Assert.AreEqual(1, path.Length);
			Assert.AreEqual(1, path[0]);
		}

		[Test]
		public void BreadthFirstSearchManyDestinationsTest()
		{
			Assert.Throws<ArgumentNullException>(() => RecursionUtils.BreadthFirstSearchManyDestinations(1, new[] { 1 }, null));
			Assert.Throws<ArgumentNullException>(() => RecursionUtils.BreadthFirstSearchManyDestinations(1, null, Graph));
			Assert.AreEqual(0, RecursionUtils.BreadthFirstSearchManyDestinations(1, new[] { 5 }, Graph).Count());

			Dictionary<int, IEnumerable<int>> paths =
				RecursionUtils.BreadthFirstSearchManyDestinations(1, new[] {21, 22, 31, 43, 62}, WideGraph)
				              .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

			//Make sure all paths were found
			Assert.IsTrue(paths.Keys.Contains(21));
			Assert.IsTrue(paths.Keys.Contains(22));
			Assert.IsTrue(paths.Keys.Contains(31));
			Assert.IsTrue(paths.Keys.Contains(43));
			Assert.IsTrue(paths.Keys.Contains(62));

			//Make sure the shortest paths are taken
			Assert.AreEqual(3, paths[21].Count());
			Assert.AreEqual(3, paths[22].Count());
			Assert.AreEqual(3, paths[31].Count()); // infinite loop exists between 1 and 3
			Assert.AreEqual(4, paths[43].Count()); // 43 has two parents of equal distance, 41 and 42
			Assert.AreEqual(3, paths[62].Count()); // two paths exist, one is 3, one is 4

			// make sure that destinations which were not asked for were not returned
			Assert.IsFalse(paths.Keys.Contains(1));
			Assert.IsFalse(paths.Keys.Contains(2));
			Assert.IsFalse(paths.Keys.Contains(3));
			Assert.IsFalse(paths.Keys.Contains(32));

			//Verify path for destination 21
			Assert.AreEqual(1, paths[21].ToArray()[0]);
			Assert.AreEqual(2, paths[21].ToArray()[1]);
			Assert.AreEqual(21, paths[21].ToArray()[2]);

			//Verify path for destination 22
			Assert.AreEqual(1, paths[22].ToArray()[0]);
			Assert.AreEqual(2, paths[22].ToArray()[1]);
			Assert.AreEqual(22, paths[22].ToArray()[2]);

			//Verify path for destination 31
			Assert.AreEqual(1, paths[31].ToArray()[0]);
			Assert.AreEqual(3, paths[31].ToArray()[1]);
			Assert.AreEqual(31, paths[31].ToArray()[2]);

			//Verify path for destination 43
			Assert.AreEqual(1, paths[43].ToArray()[0]);
			Assert.AreEqual(4, paths[43].ToArray()[1]);
			Assert.AreEqual(41, paths[43].ToArray()[2]); // when multiple parents exist with valid paths back, the first one should be consistently selected
			Assert.AreEqual(43, paths[43].ToArray()[3]);

			//Verify path for destination 62
			Assert.AreEqual(1, paths[62].ToArray()[0]);
			Assert.AreEqual(6, paths[62].ToArray()[1]);
			Assert.AreEqual(62, paths[62].ToArray()[2]);
		}
	}
}
