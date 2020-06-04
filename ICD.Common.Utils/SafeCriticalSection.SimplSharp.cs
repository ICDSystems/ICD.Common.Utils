using System;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
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
#if DEBUG
		private const int TIMEOUT = 5 * 60 * 1000;

		private readonly CMutex m_CriticalSection;
#else
		private readonly CCriticalSection m_CriticalSection;
#endif

		/// <summary>
		/// Constructor.
		/// </summary>
		public SafeCriticalSection()
		{
#if DEBUG
			m_CriticalSection = new CMutex();
#else
			m_CriticalSection = new CCriticalSection();
#endif
		}

		#region Methods

		/// <summary>
		/// Block until ownership of the critical section can be obtained.
		/// </summary>
		public void Enter()
		{
			if (m_CriticalSection == null)
				return;

			try
			{
#if DEBUG
				int attempt = 1;
				while (!m_CriticalSection.WaitForMutex(TIMEOUT))
				{
					// Poor-man's stack trace
					try
					{
						throw new InvalidProgramException("Failed to aquire mutex in expected timeframe - Attempt " + attempt++);
					}
					catch (Exception e)
					{
						ServiceProvider.GetService<ILoggerService>()
						               .AddEntry(eSeverity.Error, e, "Deadlock detected in program");
					}
				}
#else
				m_CriticalSection.Enter();
#endif
			}
			catch (ObjectDisposedException)
			{
			}
		}

		/// <summary>
		/// Release ownership of the critical section.
		/// </summary>
		public void Leave()
		{
			if (m_CriticalSection == null)
				return;

			try
			{
#if DEBUG
				m_CriticalSection.ReleaseMutex();
#else
				m_CriticalSection.Leave();
#endif
			}
			catch (ObjectDisposedException)
			{
			}
		}

		/// <summary>
		/// Attempt to enter the critical section without blocking.
		/// </summary>
		/// <returns>
		/// True, calling thread has ownership of the critical section; otherwise, false.
		/// </returns>
		public bool TryEnter()
		{
			if (m_CriticalSection == null)
				return false;

			try
			{
#if DEBUG
				return m_CriticalSection.WaitForMutex(0);
#else
				return m_CriticalSection.TryEnter();
#endif
			}
			catch (ObjectDisposedException)
			{
				return false;
			}
		}

		#endregion
	}
}

#endif
