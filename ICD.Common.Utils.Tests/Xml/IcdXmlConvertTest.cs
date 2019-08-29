using System.Linq;
using ICD.Common.Utils.Xml;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Xml
{
	[TestFixture]
	public sealed class IcdXmlConvertTest
	{
		[Test]
		public void DeserializeArrayGenericTest()
		{
			const string xml = @"<A>
	<B>1</B>
	<B>2</B>
</A>";

			using (IcdXmlReader reader = new IcdXmlReader(xml))
			{
				// Read to the first element
				reader.Read();

				int[] values = IcdXmlConvert.DeserializeArray<int>(reader).ToArray();

				Assert.AreEqual(new[] {1, 2}, values);
			}
		}

		[Test]
		public void DeserializeArrayTest()
		{
			Assert.Inconclusive();
		}
	}
}
