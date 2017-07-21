using System.Linq;
using NUnit.Framework;
using ICD.Common.Utils;
using ICD.Common.Properties;
#if SIMPLSHARP
using Crestron.IO;
#else
using System.IO;
#endif

namespace RSD.SimplSharp.Common.Interfaces.Tests.Utils
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
