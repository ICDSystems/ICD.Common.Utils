using NUnit.Framework;

namespace ICD.Common.Utils.Tests
{
	[TestFixture]
	public sealed class RegexUtilsTest
	{
		[Test]
		public static void MatchesTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public static void MatchesOptionsTest()
		{
			Assert.Inconclusive();
		}

		[TestCase(@"[assembly: AssemblyFileVersion(""1.2.3"")][assembly: AssemblyFileVersion(""1.2.3"")]",
			@"[assembly: AssemblyFileVersion(""2.0.3.0"")][assembly: AssemblyFileVersion(""2.0.3.0"")]",
			@"AssemblyFileVersion\(""(?<version>(\d+\.?){4})""\)",
			"version",
			"1.2.3")]
		public static void ReplaceGroupTest(string expected, string input, string pattern, string groupName,
		                                    string replacement)
		{
			string result = RegexUtils.ReplaceGroup(input, pattern, groupName, replacement);
			Assert.AreEqual(expected, result);
		}

		[Test]
		public static void ReplaceGroupFuncTest()
		{
			Assert.Inconclusive();
		}
	}
}
