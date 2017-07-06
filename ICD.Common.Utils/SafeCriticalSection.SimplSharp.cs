#if SIMPLSHARP
using Crestron.SimplSharp;

namespace ICD.Common.Utils
{
	/// <summary>
	/// CCriticalSection tends to get disposed before the parent is done with it.
	/// This class is an attempt to gracefully handle the ObjectDisposedExceptions we see on
	/// program termination, ocassionally causing the program to restart instead of stop.
	/// </summary>
	public sealed partial class SafeCriticalSection
	{
		private readonly CCriticalSection m_CriticalSection;

		/// <summary>
		/// Constructor.
		/// </summary>
		public SafeCriticalSection()
		{
			m_CriticalSection = new CCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Block until ownership of the critical section can be obtained.
		/// </summary>
		public void Enter()
		{
			if (m_CriticalSection == null || m_CriticalSection.Disposed)
				return;

			m_CriticalSection.Enter();
		}

		/// <summary>
		/// Release ownership of the critical section.
		/// </summary>
		public void Leave()
		{
			if (m_CriticalSection == null || m_CriticalSection.Disposed)
				return;

			m_CriticalSection.Leave();
		}

		/// <summary>
		/// Attempt to enter the critical section without blocking.
		/// </summary>
		/// <returns>
		/// True, calling thread has ownership of the critical section; otherwise, false.
		/// </returns>
		public bool TryEnter()
		{
			if (m_CriticalSection == null || m_CriticalSection.Disposed)
				return false;

			return m_CriticalSection.TryEnter();
		}

		#endregion
	}
}

#endif
