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

		[Test]
		public void TryEnterTest()
		{
			int result = 0;

			SafeCriticalSection section = new SafeCriticalSection();
			section.Enter();

			// ReSharper disable once NotAccessedVariable
			ThreadingUtils.SafeInvoke(() => { result = section.TryEnter() ? 0 : 1; });
			
			Assert.IsTrue(ThreadingUtils.Wait(() => result == 1, 1000));

			section.Leave();

			// ReSharper disable once RedundantAssignment
			ThreadingUtils.SafeInvoke(() =>
			{
				result = section.TryEnter() ? 2 : 0;
				section.Leave();
			});

			Assert.IsTrue(ThreadingUtils.Wait(() => result == 2, 1000));
		}
	}
}
