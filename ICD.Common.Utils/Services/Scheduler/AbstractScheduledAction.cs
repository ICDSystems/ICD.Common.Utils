using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils.Services.Scheduler
{
	public abstract class AbstractScheduledAction : IScheduledAction
	{
		public event EventHandler OnScheduledRunTimeChanged;

		private DateTime? m_NextRunTime;

		public DateTime? NextRunTime
		{
			get { return m_NextRunTime; }
			private set
			{
				if (m_NextRunTime == value)
					return;

				m_NextRunTime = value;

				OnScheduledRunTimeChanged.Raise(this);
			}
		}

		public void Run()
		{
			RunFinal();
			NextRunTime = UpdateRunTime();
		}

		/// <summary>
		/// Runs when the action has hit its scheduled time
		/// </summary>
		public abstract void RunFinal();

		/// <summary>
		/// Runs after RunFinal in order to set the next run time of this action
		/// </summary>
		public abstract DateTime? UpdateRunTime();
	}
}