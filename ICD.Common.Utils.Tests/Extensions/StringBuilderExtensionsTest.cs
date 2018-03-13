using System.Text;
using ICD.Common.Utils.Extensions;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Extensions
{
    [TestFixture]
    public sealed class StringBuilderExtensionsTest
    {
        [Test]
        public void ClearTest()
        {
            StringBuilder builder = new StringBuilder("test");
            builder.Clear();

            Assert.AreEqual(0, builder.Length);
        }

        [Test]
        public void PopTest()
        {
            StringBuilder builder = new StringBuilder("test");

            Assert.AreEqual("test", builder.Pop());
            Assert.AreEqual(0, builder.Length);
        }
    }
}
