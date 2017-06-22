using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils.Timers
{
	/// <summary>
	/// Simple class for implementing things like volume ramps, where a button has an
	/// immediate effect, and then begins ramping after a brief delay.
	/// </summary>
	[PublicAPI]
	public sealed class Repeater : IDisposable
	{
		/// <summary>
		/// Raised on the initial repeat.
		/// </summary>
		[PublicAPI]
		public event EventHandler OnInitialRepeat;

		/// <summary>
		/// Raised on each subsequent repeat.
		/// </summary>
		[PublicAPI]
		public event EventHandler OnRepeat;

		private readonly SafeTimer m_RepeatTimer;

		private readonly long m_BeforeRepeat;
		private readonly long m_BetweenRepeat;

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="beforeRepeat">The delay before the second increment</param>
		/// <param name="betweenRepeat">The delay between each subsequent repeat</param>
		public Repeater(long beforeRepeat, long betweenRepeat)
		{
			m_RepeatTimer = SafeTimer.Stopped(RepeatCallback);

			m_BeforeRepeat = beforeRepeat;
			m_BetweenRepeat = betweenRepeat;
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
		[PublicAPI]
		public void Start()
		{
			OnInitialRepeat.Raise(this);

			m_RepeatTimer.Reset(m_BeforeRepeat, m_BetweenRepeat);
		}

		/// <summary>
		/// Stop repeating volume.
		/// </summary>
		[PublicAPI]
		public void Stop()
		{
			m_RepeatTimer.Stop();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Called for every repeat.
		/// </summary>
		private void RepeatCallback()
		{
			OnRepeat.Raise(this);
		}

		#endregion
	}
}
