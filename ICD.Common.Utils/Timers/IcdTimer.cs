using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils.Timers
{
	/// <summary>
	/// IcdTimer provides events for time increments as well as timer elapsed.
	/// </summary>
	public sealed class IcdTimer : IDisposable
	{
		private const long DEFAULT_HEARTBEAT_INTERVAL = 500;

		/// <summary>
		/// Called when the timer is restarted/stopped.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnIsRunningChanged;

		/// <summary>
		/// Called when the timer has elapsed.
		/// </summary>
		public event EventHandler OnElapsed;

		/// <summary>
		/// Called when the milliseconds count changes. Useful for updating UIs.
		/// </summary>
		public event EventHandler OnMillisecondsChanged;

		private readonly IcdStopwatch m_Stopwatch;

		private readonly SafeTimer m_Heartbeat;
		private long m_LastHeartbeatMilliseconds;

		#region Properties

		/// <summary>
		/// Returns true if the timer is stopped.
		/// </summary>
		public bool IsStopped { get { return !m_Stopwatch.IsRunning; } }

		/// <summary>
		/// Returns true if the timer is running.
		/// </summary>
		public bool IsRunning { get { return m_Stopwatch.IsRunning; } }

		/// <summary>
		/// Returns true if the timer has elapsed.
		/// </summary>
		public bool IsElapsed { get { return Milliseconds > Length; } }

		/// <summary>
		/// Returns the number of milliseconds that have passed since the timer started.
		/// </summary>
		public long Milliseconds { get { return m_Stopwatch.ElapsedMilliseconds; } }

		/// <summary>
		/// The number of milliseconds before the timer is elapsed.
		/// </summary>
		public long Length { get; private set; }

		/// <summary>
		/// Gets the remaining number of milliseconds until the timer is elapsed. Returns 0 if the timer has elapsed.
		/// </summary>
		public long Remaining { get { return Math.Max(Length - Milliseconds, 0); } }

		/// <summary>
		/// Gets the remaining number of seconds until the timer is elapsed. Returns 0 if the timer has elapsed.
		/// </summary>
		public long RemainingSeconds { get { return (long)Math.Ceiling(Remaining / 1000.0f); } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public IcdTimer()
			: this(DEFAULT_HEARTBEAT_INTERVAL)
		{
		}

		/// <summary>
		/// Creates an IcdTimer with the specified heartbeat interval for the internal timer.
		/// This allows a finer resolution for timing than the default 500ms.
		/// </summary>
		/// <param name="heartbeatInterval"></param>
		public IcdTimer(long heartbeatInterval)
		{
			m_Heartbeat = new SafeTimer(HeartbeatCallback, heartbeatInterval, heartbeatInterval);
			m_Stopwatch = new IcdStopwatch();

			Stop();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			OnIsRunningChanged = null;
			OnElapsed = null;
			OnMillisecondsChanged = null;

			m_Stopwatch.Stop();
			m_Heartbeat.Dispose();
		}

		/// <summary>
		/// Restarts the timer.
		/// </summary>
		public void Restart(long length)
		{
			Length = length;

			m_Stopwatch.Reset();
			m_LastHeartbeatMilliseconds = 0;
			m_Stopwatch.Start();

			RaiseOnIsRunningChanged();
		}

		/// <summary>
		/// Stops the timer.
		/// </summary>
		public void Stop()
		{
			m_Stopwatch.Stop();

			RaiseOnIsRunningChanged();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Called when the heartbeat timer elapses.
		/// </summary>
		private void HeartbeatCallback()
		{
			if (Milliseconds == m_LastHeartbeatMilliseconds)
				return;

			OnMillisecondsChanged.Raise(this);

			if (m_LastHeartbeatMilliseconds <= Length && IsElapsed)
				OnElapsed.Raise(this);

			m_LastHeartbeatMilliseconds = Milliseconds;
		}

		/// <summary>
		/// Raises the OnIsRunningChanged event.
		/// </summary>
		private void RaiseOnIsRunningChanged()
		{
			OnIsRunningChanged.Raise(this, new BoolEventArgs(IsRunning));
		}

		#endregion
	}
}
