using System;

namespace ICD.Common.Utils.Services.Scheduler
{
	public interface IScheduledAction
	{
		/// <summary>
		/// Raised when the scheduled run time has changed. The sender of the event must be the action itself
		/// </summary>
		event EventHandler OnScheduledRunTimeChanged;

		/// <summary>
		/// Gets the next time this action should be run
		/// </summary>
		DateTime? NextRunTime { get; }

		void Run();
	}
}
