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
			Assert.Inconclusive();
		}

		[Test]
		public void TryEnterTest()
		{
			int result = 0;

			SafeCriticalSection section = new SafeCriticalSection();
			section.Enter();

			// ReSharper disable once NotAccessedVariable
			object handle = ThreadingUtils.SafeInvoke(() => { result = section.TryEnter() ? 0 : 1; });
			ThreadingUtils.Sleep(1000);

			Assert.AreEqual(1, result);

			section.Leave();

			// ReSharper disable once RedundantAssignment
			handle = ThreadingUtils.SafeInvoke(() =>
			{
				result = section.TryEnter() ? 2 : 0;
				section.Leave();
			});
			ThreadingUtils.Sleep(1000);

			Assert.AreEqual(2, result);
		}
	}
}
