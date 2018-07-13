using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Timers;

namespace ICD.Common.Utils.Services.Scheduler
{
	public sealed class ActionSchedulerService : IActionSchedulerService, IDisposable
	{
		private readonly List<IScheduledAction> m_Actions;
		private readonly SafeTimer m_Timer;
		private readonly SafeCriticalSection m_CriticalSection;

		public ActionSchedulerService()
		{
			m_Actions = new List<IScheduledAction>();
			m_Timer = new SafeTimer(TimerCallback, -1);
			m_CriticalSection = new SafeCriticalSection();
		}

		public void Dispose()
		{
			Clear();

			m_Timer.Stop();
			m_Timer.Dispose();
		}

		#region Methods

		public void Add(IScheduledAction action)
		{
			m_CriticalSection.Enter();
			try
			{
				Subscribe(action);
				m_Actions.AddSorted(action, a => a.NextRunTime);
			}
			finally
			{
				m_CriticalSection.Leave();
			}

			RescheduleTimer();
		}

		public void Remove(IScheduledAction action)
		{
			m_CriticalSection.Enter();
			try
			{
				Unsubscribe(action);
				m_Actions.Remove(action);
			}
			finally
			{
				m_CriticalSection.Leave();
			}

			RescheduleTimer();
		}

		public void Clear()
		{
			m_CriticalSection.Enter();
			try
			{
				foreach (IScheduledAction action in m_Actions)
				{
					Unsubscribe(action);
				}

				m_Actions.Clear();
			}
			finally
			{
				m_CriticalSection.Leave();
			}

			RescheduleTimer();
		}

		public override string ToString()
		{
			return "ActionSchedulerService";
		}

		#endregion

		#region Private Methods

		private void TimerCallback()
		{
			IScheduledAction[] actionsToRun;

			m_CriticalSection.Enter();
			try
			{
				actionsToRun = m_Actions
					.Where(a => a.NextRunTime < IcdEnvironment.GetLocalTime())
					.OrderBy(a => a.NextRunTime)
					.ToArray();
			}
			finally
			{
				m_CriticalSection.Leave();
			}

			// leave the critical section so any actions that run can enter
			foreach (IScheduledAction action in actionsToRun)
			{
				try
				{
					action.Run();
				}
				catch (Exception ex)
				{
					Log(eSeverity.Error, ex, "Error occurred while running scheduled action");
				}
			}

			RescheduleTimer();
		}

		private void RescheduleTimer()
		{
			// enter again to check the closest next run time
			m_CriticalSection.Enter();
			try
			{
				var action = m_Actions.FirstOrDefault(a => a.NextRunTime != null);
				if (action == null || action.NextRunTime == null)
				{
					m_Timer.Stop();
					return;
				}

				long msToNextAction = (long)(action.NextRunTime.Value - IcdEnvironment.GetLocalTime()).TotalMilliseconds;
				m_Timer.Reset(msToNextAction);
			}
			finally
			{
				m_CriticalSection.Leave();
			}
		}

		private void Log(eSeverity severity, string message, params object[] args)
		{
			ILoggerService logger = ServiceProvider.TryGetService<ILoggerService>();
			if (logger == null)
				return;

			logger.AddEntry(severity, string.Format("{0} - {1}", this, message), args);
		}

		private void Log(eSeverity severity, Exception ex, string message, params object[] args)
		{
			ILoggerService logger = ServiceProvider.TryGetService<ILoggerService>();
			if (logger == null)
				return;

			logger.AddEntry(severity, ex, string.Format("{0} - {1}", this, message), args);
		}

		#endregion

		#region Action Callbacks

		private void Subscribe(IScheduledAction action)
		{
			action.OnScheduledRunTimeChanged += ActionOnScheduledRunTimeChanged;
		}

		private void Unsubscribe(IScheduledAction action)
		{
			action.OnScheduledRunTimeChanged -= ActionOnScheduledRunTimeChanged;
		}

		private void ActionOnScheduledRunTimeChanged(object sender, EventArgs eventArgs)
		{
			IScheduledAction action = sender as IScheduledAction;
			if (action == null)
				return;

			m_CriticalSection.Enter();
			try
			{
				m_Actions.Remove(action);
				m_Actions.AddSorted(action, a => a.NextRunTime);
			}
			finally
			{
				m_CriticalSection.Leave();
			}

			RescheduleTimer();
		}

		#endregion
	}
}