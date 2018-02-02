using NUnit.Framework;
#if SIMPLSHARP
using Crestron.IO;
#else
using System.IO;
#endif

namespace ICD.Common.Utils.Tests
{
	[TestFixture]
	public sealed class PathUtilsTest
	{
		#region Properties

		[Test]
		public static void RootPathTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public static void NvramPathTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public static void ProgramConfigPathTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public static void CommonConfigPathTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public static void CommonLibPathTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public static void ProgramLibPathTest()
		{
			Assert.Inconclusive();
		}

		#endregion

		#region Methods

		[Test]
		public void JoinTest()
		{
			string expected = Path.Combine("A", "B");
			expected = Path.Combine(expected, "C");

			Assert.AreEqual(expected, PathUtils.Join("A", "B", "C"));
		}

		[Test]
		public static void GetFullPathTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public static void ChangeFilenameWithoutExtTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public static void GetPathWithoutExtensionTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public static void RecurseFilePathsTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public static void GetDefaultConfigPathTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public static void GetProgramConfigPathTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public static void PathExistsTest()
		{
			Assert.Inconclusive();
		}

		#endregion
	}
}
