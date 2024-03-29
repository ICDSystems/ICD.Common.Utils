﻿using ICD.Common.Utils.Services.Logging;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Services.Logging
{
    [TestFixture]
    public sealed class SeverityEventArgsTest
    {
        [TestCase(eSeverity.Critical)]
        public void DataTest(eSeverity severity)
        {
            SeverityEventArgs args = new SeverityEventArgs(severity);
            Assert.AreEqual(severity, args.Data);
        }
    }
}
