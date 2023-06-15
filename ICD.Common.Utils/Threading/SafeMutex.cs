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
	public sealed class SafeMutex : IDisposable
	{
	        private bool _disposedValue;
	
#if SIMPLSHARP
		private readonly CMutex m_Mutex;
#else
		private readonly Mutex m_Mutex;
#endif

		/// <summary>
        	/// Initializes a new instance of the <see cref="SafeMutex"/> class.
        	/// </summary>
		public SafeMutex()
		{
#if SIMPLSHARP
			m_Mutex = new CMutex();
#else
			m_Mutex = new Mutex();
#endif
		}
 		
		/// <summary>
        	/// Initializes a new instance of the <see cref="SafeMutex"/> class.
        	/// </summary>
        	/// <inheritdoc cref="System.Threading.Mutex.Mutex(bool)"/>
        	public SafeMutex(bool initiallyOwned)
        	{
#if SIMPLSHARP
			m_Mutex = new CMutex(initiallyOwned);
#else
            		m_Mutex = new Mutex(initiallyOwned);
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

		#region IDisposable

		private void Dispose(bool disposing)
	        {
			if (!_disposedValue)
			{
				if (disposing)
				{
					// dispose managed state (managed objects)				
					try
					{
						// disposing of a mutex automatically releases it. Match that behavior
						m_mutex.ReleaseMutex();
						m_Mutex.Dispose();
					}
            				catch (ObjectDisposedException)
            				{
                				// Releasing a disposed mutex in this case is valid behaviour
            				}
                		}

                		// free unmanaged resources (unmanaged objects) and override finalizer
                		// set large fields to null
                		_disposedValue = true;
			}
		}
        

		// override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~SafeMutex()
		// {
		//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		//     Dispose(disposing: false);
		// }

		/// <inheritdoc/>
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
		
		#endregion
	}		
}
