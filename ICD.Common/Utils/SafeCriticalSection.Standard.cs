#if !SIMPLSHARP
using System.Threading;

namespace ICD.Common.Utils
{
	public sealed partial class SafeCriticalSection
	{
		private readonly Mutex m_Mutex;

		/// <summary>
		/// Constructor.
		/// </summary>
		public SafeCriticalSection()
		{
			m_Mutex = new Mutex();
		}

		#region Methods

		/// <summary>
		/// Block until ownership of the critical section can be obtained.
		/// </summary>
		public void Enter()
		{
			m_Mutex.WaitOne();
		}

		/// <summary>
		/// Release ownership of the critical section.
		/// </summary>
		public void Leave()
		{
			m_Mutex.ReleaseMutex();
		}

		/// <summary>
		/// Attempt to enter the critical section without blocking.
		/// </summary>
		/// <returns>
		/// True, calling thread has ownership of the critical section; otherwise, false.
		/// </returns>
		public bool TryEnter()
		{
			return m_Mutex.WaitOne(0);
		}

		#endregion
	}
}

#endif
