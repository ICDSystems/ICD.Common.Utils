using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ICD.Common.Utils.Tests
{
	[TestFixture]
    public sealed class RecursionUtilsTest
    {
		[Test]
		public void BreadthFirstSearchTest()
		{
			IEnumerable<int> GetChildren(int node)
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

			int[] nodes = RecursionUtils.BreadthFirstSearch(1, GetChildren).ToArray();

			Assert.AreEqual(4, nodes.Length);
			Assert.AreEqual(1, nodes[0]);
			Assert.AreEqual(2, nodes[1]);
			Assert.AreEqual(3, nodes[2]);
			Assert.AreEqual(4, nodes[3]);
		}
	}
}
