using System;
using ICD.Common.Properties;

namespace ICD.Common.Utils.Timers
{
	/// <summary>
	/// Simple class for implementing things like volume ramps, where a button has an
	/// immediate effect, and then begins ramping after a brief delay.
	/// </summary>
	[PublicAPI]
	public sealed class Repeater : IDisposable
	{
		public delegate void RepeatCallback(bool isInitial);

		private readonly SafeTimer m_RepeatTimer;

		private RepeatCallback m_RepeatCallback; 

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public Repeater()
		{
			m_RepeatTimer = SafeTimer.Stopped(TimerElapsed);
		}

		/// <summary>
		/// Destructor.
		/// </summary>
		~Repeater()
		{
			Dispose();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			m_RepeatTimer.Dispose();
		}

		/// <summary>
		/// Begin repeating.
		/// </summary>
		/// <param name="repeatCallback"></param>
		/// <param name="beforeRepeat">The delay before the second increment</param>
		/// <param name="betweenRepeat">The delay between each subsequent repeat</param>
		[PublicAPI]
		public void Start([NotNull] RepeatCallback repeatCallback, long beforeRepeat, long betweenRepeat)
		{
			if (repeatCallback == null)
				throw new ArgumentNullException("repeatCallback");

			Stop();

			m_RepeatCallback = repeatCallback;
			m_RepeatCallback(true);

			m_RepeatTimer.Reset(beforeRepeat, betweenRepeat);
		}

		/// <summary>
		/// Stop repeating volume.
		/// </summary>
		[PublicAPI]
		public void Stop()
		{
			m_RepeatTimer.Stop();
			m_RepeatCallback = null;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Called for every repeat.
		/// </summary>
		private void TimerElapsed()
		{
			m_RepeatCallback(false);
		}

		#endregion
	}
}
