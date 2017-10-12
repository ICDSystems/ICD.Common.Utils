﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICD.Common.Utils.Tests
{
	[TestFixture]
    public sealed class RecursionUtilsTest
    {
		private IEnumerable<int> Graph(int node)
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

		[Test]
		public void BreadthFirstSearchTest()
		{
			Assert.Throws<ArgumentNullException>(() => RecursionUtils.BreadthFirstSearch(1, null).ToArray());

			int[] nodes = RecursionUtils.BreadthFirstSearch(1, Graph).ToArray();

			Assert.AreEqual(4, nodes.Length);
			Assert.AreEqual(1, nodes[0]);
			Assert.AreEqual(2, nodes[1]);
			Assert.AreEqual(3, nodes[2]);
			Assert.AreEqual(4, nodes[3]);
		}

		[Test]
		public void BreadthFirstSearchPath()
		{
			Assert.Throws<ArgumentNullException>(() => RecursionUtils.BreadthFirstSearchPath(1, 4, null).ToArray());

			int[] path = RecursionUtils.BreadthFirstSearchPath(1, 4, Graph).ToArray();

			Assert.AreEqual(3, path.Length);
			Assert.AreEqual(1, path[0]);
			Assert.AreEqual(2, path[1]);
			Assert.AreEqual(4, path[2]);

			IEnumerable<int> noPath = RecursionUtils.BreadthFirstSearchPath(3, 4, Graph);

			Assert.IsNull(noPath);
		}
	}
}