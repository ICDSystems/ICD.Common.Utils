using NUnit.Framework;

namespace ICD.Common.Utils.Tests
{
	[TestFixture]
    public sealed class SafeCriticalSectionTest
    {
		[Test]
		public void ExecuteTest()
		{
			bool result = false;

			SafeCriticalSection section = new SafeCriticalSection();
			section.Execute(() => result = true);

			Assert.IsTrue(result);
		}

		[Test]
		public void ExecuteReturnTest()
		{
			SafeCriticalSection section = new SafeCriticalSection();
			Assert.IsTrue(section.Execute(() => true));
		}

		[Test]
		public void EnterTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void LeaveTest()
		{
			SafeCriticalSection section = new SafeCriticalSection();
			Assert.DoesNotThrow(() => section.Leave());

			Assert.Inconclusive();
		}
    }
}
