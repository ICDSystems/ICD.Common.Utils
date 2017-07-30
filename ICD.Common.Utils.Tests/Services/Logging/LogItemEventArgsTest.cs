using ICD.Common.Services.Logging;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests_NetStandard.Services.Logging
{
    [TestFixture]
    public sealed class LogItemEventArgsTest
    {
        [Test]
        public void DataTest()
        {
            LogItem item = new LogItem(eSeverity.Critical, "test");
            LogItemEventArgs eventArgs = new LogItemEventArgs(item);

            Assert.AreEqual(item, eventArgs.Data);
        }
    }
}
