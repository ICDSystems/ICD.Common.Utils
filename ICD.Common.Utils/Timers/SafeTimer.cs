using System;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
#if SIMPLSHARP
using Timer = Crestron.SimplSharp.CTimer;
using Timeout = Crestron.SimplSharp.Timeout;
#else
using Timer = System.Threading.Timer;
using Timeout = System.Threading.Timeout;
#endif

namespace ICD.Common.Utils.Timers
{
	/// <summary>
	/// SafeTimer is a platform agnostic timer that better handles disposal than the underlying timer.
	/// </summary>
	public sealed class SafeTimer : IStateDisposable
	{
		private readonly Timer m_Timer;
		private Action m_Callback;

#if !SIMPLSHARP
        private int m_RepeatPeriod;
#endif

		/// <summary>
		/// Returns true if this instance has been disposed.
		/// </summary>
		public bool IsDisposed { get; private set; }

		#region Constructors

		/// <summary>
		/// Creates a timer that is called immediately and then every repeatPeriod in milliseconds.
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
			m_Timer = new Timer(SafeCallback, null, Timeout.Infinite, Timeout.Infinite);

			Reset(dueTime, repeatPeriod);
		}

		/// <summary>
		/// Creates a timer that is initially stopped.
		/// </summary>
		/// <param name="callback"></param>
		/// <returns></returns>
		public static SafeTimer Stopped(Action callback)
		{
			return new SafeTimer(callback, Timeout.Infinite, Timeout.Infinite);
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

			IsDisposed = true;

			Stop();
			m_Timer.Dispose();

			m_Callback = null;
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
			Reset(dueTime, Timeout.Infinite);
		}

		/// <summary>
		/// Callback is called after the dueTime milliseconds and every repeatPeriod milliseconds.
		/// </summary>
		/// <param name="dueTime"></param>
		/// <param name="repeatPeriod"></param>
		public void Reset(long dueTime, long repeatPeriod)
		{
			if (dueTime < 0 && dueTime != Timeout.Infinite)
				throw new ArgumentOutOfRangeException("dueTime", "DueTime must be greater than or equal to 0ms");

			if (dueTime >= int.MaxValue)
				throw new ArgumentOutOfRangeException("dueTime", string.Format("DueTime must be less than {0:n0}ms", int.MaxValue));

			if (repeatPeriod < 0 && repeatPeriod != Timeout.Infinite)
				throw new ArgumentOutOfRangeException("repeatPeriod", "Repeat period must be greater than or equal to 0ms");

			if (repeatPeriod >= int.MaxValue)
				throw new ArgumentOutOfRangeException("repeatPeriod", string.Format("Repeat period must be less than {0:n0}ms", int.MaxValue));

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
			// the reference to the timer will be cleared, and eventually the Timer will call the callback
			// despite being stopped/disposed.
			if (IsDisposed || m_Timer == null
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
