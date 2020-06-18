using System;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils.Services.Scheduler
{
	public abstract class AbstractScheduledAction : IScheduledAction
	{
		public event EventHandler OnScheduledRunTimeChanged;

		private DateTime? m_NextRunTimeUtc;

		public DateTime? NextRunTimeUtc
		{
			get { return m_NextRunTimeUtc; }
			private set
			{
				if (m_NextRunTimeUtc == value)
					return;

				m_NextRunTimeUtc = value;

				OnScheduledRunTimeChanged.Raise(this);
			}
		}

		public void Run()
		{
			NextRunTimeUtc = RunFinal();
		}

		protected void UpdateNextRunTime()
		{
			NextRunTimeUtc = GetNextRunTimeUtc();
		}

		/// <summary>
		/// Runs when the action has hit its scheduled time
		/// </summary>
		protected abstract DateTime? RunFinal();

		/// <summary>
		/// Runs after RunFinal in order to set the next run time of this action
		/// </summary>
		protected abstract DateTime? GetNextRunTimeUtc();
	}
}