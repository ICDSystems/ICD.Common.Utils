using ICD.Common.Utils.Extensions;
using NUnit.Framework;
using System.Linq;

namespace ICD.Common.Utils.Tests_NetStandard.Extensions
{
	[TestFixture]
    public sealed class EnumerableExtensionsTest
    {
		[Test]
		public void ConsolidateTest()
		{
			string[] sequence = EnumerableExtensions.Consolidate(new string[] { "A", "B", "B", "C" }).ToArray();

			Assert.AreEqual(3, sequence.Length, StringUtils.ArrayFormat(sequence));
			Assert.AreEqual("A", sequence[0]);
			Assert.AreEqual("B", sequence[1]);
			Assert.AreEqual("C", sequence[2]);
		}
	}
}
