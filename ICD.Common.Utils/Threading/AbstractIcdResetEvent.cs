using System;
using ICD.Common.Properties;
#if SIMPLSHARP
using Crestron.SimplSharp;
#else
using System.Threading;
#endif

namespace ICD.Common.Utils
{
	public abstract class AbstractIcdResetEvent : IDisposable
	{

#if SIMPLSHARP
		private readonly CEvent m_Event;
#else
		private readonly  EventWaitHandle m_Event;
#endif

#if SIMPLSHARP
		/// <summary>
		/// Initializes a new instance of the IcdManualResetEvent class with the CEvent
		/// </summary>
		[PublicAPI]
		protected AbstractIcdResetEvent(CEvent eventHandle)
		{
			m_Event = eventHandle;
		}
#else
		/// <summary>
		/// Initializes a new instance of the IcdManualResetEvent class with the CEvent
		/// </summary>
		[PublicAPI]
		protected AbstractIcdResetEvent(EventWaitHandle eventHandle)
		{
			m_Event = eventHandle;
		}

#endif

		/// <summary>
		/// Sets the state of the event to signaled, allowing one or more waiting threads to proceed.
		/// </summary>
		/// <returns>true if the operation succeeds; otherwise, false.</returns>
		[PublicAPI]
		public bool Set()
		{
			return m_Event.Set();
		}

		/// <summary>
		/// Sets the state of the event to nonsignaled, causing threads to block.
		/// </summary>
		/// <returns>true if the operation succeeds; otherwise, false.</returns>
		[PublicAPI]
		public bool Reset()
		{
			return m_Event.Reset();
		}

		/// <summary>
		/// Function to wait for the event to be signaled. This will block indefinitely until the event is signaled.
		/// </summary>
		/// <returns>True if the current instance receives a signal otherwise false.</returns>
		[PublicAPI]
		public bool WaitOne()
		{
#if SIMPLSHARP
			return m_Event.Wait();
#else
			return m_Event.WaitOne();
#endif
		}

		/// <summary>
		/// Function to wait for the event to be signaled.
		/// </summary>
		/// <param name="timeout">Timeout in milliseconds or Timeout.Infinite to wait indefinitely.</param>
		/// <returns>True if the current instance receives a signal otherwise false.</returns>
		public bool WaitOne(int timeout)
		{
#if SIMPLSHARP
			return m_Event.Wait(timeout);
#else
			return m_Event.WaitOne(timeout);
#endif
		}

		/// <summary>
		/// Clean up of resources.
		/// </summary>
		public void Dispose()
		{
			m_Event.Dispose();
		}

		/// <summary>
		/// Close the event to release all resources used by this instance.
		/// </summary>
		[PublicAPI]
		public void Close()
		{
			m_Event.Close();
		}
	}
}