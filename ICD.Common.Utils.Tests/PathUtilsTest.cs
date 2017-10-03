using ICD.Common.Properties;
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
		[Test, UsedImplicitly]
		public void JoinTest()
		{
			string expected = Path.Combine("A", "B");
			expected = Path.Combine(expected, "C");

			Assert.AreEqual(expected, PathUtils.Join("A", "B", "C"));
		}
	}
}
