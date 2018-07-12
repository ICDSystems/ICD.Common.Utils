namespace ICD.Common.Utils.Services.Scheduler
{
	public interface IActionSchedulerService
	{
		void Add(IScheduledAction action);

		void Remove(IScheduledAction action);
	}
}