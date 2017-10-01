#if !SIMPLSHARP
using System.Threading;

namespace ICD.Common.Utils
{
	public sealed partial class SafeCriticalSection
	{
		#region Methods

		/// <summary>
		/// Block until ownership of the critical section can be obtained.
		/// </summary>
		public void Enter()
		{
			Monitor.Enter(this);
		}

		/// <summary>
		/// Release ownership of the critical section.
		/// </summary>
		public void Leave()
		{
			Monitor.Exit(this);
		}

		/// <summary>
		/// Attempt to enter the critical section without blocking.
		/// </summary>
		/// <returns>
		/// True, calling thread has ownership of the critical section; otherwise, false.
		/// </returns>
		public bool TryEnter()
		{
			return Monitor.TryEnter(this);
		}

		#endregion
	}
}

#endif
