using NUnit.Framework;
using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Services.Logging;

namespace ICD.Common.Utils.Tests.Services.Logging
{
    [UsedImplicitly, TestFixture]
    public sealed class LogItemTest
    {
        [UsedImplicitly, Test]
        public void TimestampTest()
        {
            LogItem item = new LogItem(eSeverity.Critical, null);
            DateTime time = IcdEnvironment.GetLocalTime();

            Assert.IsTrue((time - item.Timestamp).TotalSeconds <= 1);
        }

        [UsedImplicitly]
        [TestCase(eSeverity.Alert)]
        public void SeverityTest(eSeverity severity)
        {
            LogItem item = new LogItem(severity, null);
            Assert.AreEqual(severity, item.Severity);
        }

        [UsedImplicitly]
        [TestCase(null)]
        [TestCase("test")]
        public void MessageTest(string message)
        {
            LogItem item = new LogItem(eSeverity.Critical, message);
            Assert.AreEqual(message, item.Message);
        }

        [UsedImplicitly, Test]
        public void GetFusionLogTextTest()
        {
            Assert.Inconclusive();
        }

        [UsedImplicitly, Test]
        public void EqualityOperatorTest()
        {
            LogItem a = new LogItem(eSeverity.Alert, "test");
            LogItem b = a;
            LogItem c = new LogItem(eSeverity.Critical, "dsfds");

            Assert.IsTrue(a == b);
            Assert.IsFalse(b == c);
            Assert.IsFalse(b == null);
        }

        [UsedImplicitly, Test]
        public void InequalityOperatorTest()
        {
            LogItem a = new LogItem(eSeverity.Alert, "test");
            LogItem b = a;
            LogItem c = new LogItem(eSeverity.Critical, "dsfds");

            Assert.IsFalse(a != b);
            Assert.IsTrue(b != c);
            Assert.IsTrue(b != null);
        }

        [UsedImplicitly, Test]
        public void EqualityTest()
        {
            LogItem a = new LogItem(eSeverity.Alert, "test");
            LogItem b = a;
            LogItem c = new LogItem(eSeverity.Critical, "dsfds");

            Assert.IsTrue(a.Equals(b));
            Assert.IsFalse(b.Equals(c));
            Assert.IsFalse(b.Equals(null));
        }
    }
}
