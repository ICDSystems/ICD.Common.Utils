using System;
#if SIMPLSHARP
using Crestron.SimplSharp;
#else
using System.Threading;
#endif

namespace ICD.Common.Utils
{
	/// <summary>
	/// Like the CCriticalSection, the CMutex tends to get disposed before the parent is
	/// done with it. This class is an attempt to gracefully handle the ObjectDisposedExceptions
	/// we see on program termination, ocassionally causing the program to restart instead of stop.
	/// </summary>
	public sealed class SafeMutex
	{
#if SIMPLSHARP
		private readonly CMutex m_Mutex;
#else
		private readonly Mutex m_Mutex;
#endif

		/// <summary>
		/// Constructor.
		/// </summary>
		public SafeMutex()
		{
#if SIMPLSHARP
			m_Mutex = new CMutex();
#else
			m_Mutex = new Mutex();
#endif
		}

		#region Methods

		/// <summary>
		/// Waits the given number of milliseconds to aquire the mutex.
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns>True if the mutex was aquired.</returns>
		public bool WaitForMutex(int timeout)
		{
			try
			{
#if SIMPLSHARP
				return m_Mutex.WaitForMutex(timeout);
#else
                return m_Mutex.WaitOne(timeout);
#endif
			}
			catch (ObjectDisposedException)
			{
				return false;
			}
		}

		/// <summary>
		/// Releases the mutex.
		/// </summary>
		public void ReleaseMutex()
		{
			try
			{
				m_Mutex.ReleaseMutex();
			}
			catch (ObjectDisposedException)
			{
				// Releasing a disposed mutex in this case is valid behaviour
			}
		}

		#endregion
	}
}
