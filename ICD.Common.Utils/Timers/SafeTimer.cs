using System;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
#if SIMPLSHARP
using Crestron.SimplSharp;
#else
using System.Threading;
#endif

namespace ICD.Common.Utils.Timers
{
	/// <summary>
	/// SafeTimer wraps CTimer to hide some of the jank. 
	/// </summary>
	public sealed class SafeTimer : IStateDisposable
	{
#if SIMPLSHARP
		private readonly CTimer m_Timer;
#else
        private readonly Timer m_Timer;
        private int m_RepeatPeriod;
#endif
		private Action m_Callback;

		/// <summary>
		/// Returns true if this instance has been disposed.
		/// </summary>
		public bool IsDisposed { get; private set; }

		#region Constructors

		/// <summary>
		/// Creates a timer that is called every repeatPeriod in milliseconds.
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="repeatPeriod"></param>
		public SafeTimer(Action callback, long repeatPeriod)
			: this(callback, 0, repeatPeriod)
		{
		}

		/// <summary>
		/// Creates a timer that is called in dueTime milliseconds and then every
		/// repeatPeriod milliseconds afterwards.
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="dueTime"></param>
		/// <param name="repeatPeriod"></param>
		public SafeTimer(Action callback, long dueTime, long repeatPeriod)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");

			m_Callback = callback;
#if SIMPLSHARP
			m_Timer = new CTimer(SafeCallback, null, dueTime, repeatPeriod);
#else
            m_RepeatPeriod = (int)repeatPeriod;
            m_Timer = new Timer(SafeCallback, null, (int)dueTime, m_RepeatPeriod);
#endif
		}

		/// <summary>
		/// Creates a timer that is initially stopped.
		/// </summary>
		/// <param name="callback"></param>
		/// <returns></returns>
		public static SafeTimer Stopped(Action callback)
		{
			//No due time or repeat period on a stopped timer
			SafeTimer output = new SafeTimer(callback, -1, -1);
			output.Stop();
			return output;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			if (IsDisposed)
				return;

			Stop();
			m_Timer.Dispose();

			m_Callback = null;

			IsDisposed = true;
		}

		/// <summary>
		/// Stops the timer.
		/// </summary>
		public void Stop()
		{
#if SIMPLSHARP
			m_Timer.Stop();
#else
            m_Timer.Change(Timeout.Infinite, Timeout.Infinite);
#endif
		}

		/// <summary>
		/// Immediately calls the callback and resets the timer
		/// </summary>
		public void Trigger()
		{
#if SIMPLSHARP
			m_Timer.Reset();
#else
			m_Timer.Change(0, m_RepeatPeriod);
			m_Callback();
#endif
		}

		/// <summary>
		/// Callback is called once after the dueTime milliseconds.
		/// </summary>
		/// <param name="dueTime"></param>
		public void Reset(long dueTime)
		{
#if SIMPLSHARP
			m_Timer.Reset(dueTime);
#else
			m_Timer.Change((int)dueTime, Timeout.Infinite);
#endif
		}

		/// <summary>
		/// Callback is called after the dueTime milliseconds and every repeatPeriod milliseconds.
		/// </summary>
		/// <param name="dueTime"></param>
		/// <param name="repeatPeriod"></param>
		public void Reset(long dueTime, long repeatPeriod)
		{
#if SIMPLSHARP
			m_Timer.Reset(dueTime, repeatPeriod);
#else
            m_RepeatPeriod = (int)repeatPeriod;
            m_Timer.Change((int)dueTime, m_RepeatPeriod);
#endif
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Only executes the callback if the timer has not been disposed.
		/// Catches any exceptions and logs them.
		/// </summary>
		/// <param name="unused"></param>
		private void SafeCallback(object unused)
		{
			// Essentially the meat of this class. There's some weirdness with the garbage collector where
			// the reference to the timer will be cleared, and eventually the CTimer will call the callback
			// despite being stopped/disposed.
			if (m_Timer == null
#if SIMPLSHARP
			    || m_Timer.Disposed
#endif
				)
				return;

			try
			{
				m_Callback();
			}
			catch (Exception e)
			{
				string message = string.Format("{0} failed to execute callback - {1}", GetType().Name, e.Message);
				ServiceProvider.TryGetService<ILoggerService>().AddEntry(eSeverity.Error, e, message);
			}
		}

		#endregion
	}
}
