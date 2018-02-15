using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Json
{
    public abstract class AbstractGenericJsonConverterTest
    {
		[Test]
	    public abstract void WriteJsonTest();

		[Test]
	    public abstract void ReadJsonTest();
    }
}
