﻿using System;
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
			RunFinal();
			NextRunTimeUtc = GetNextRunTimeUtc();
		}

		public void UpdateNextRunTime()
		{
			NextRunTimeUtc = GetNextRunTimeUtc();
		}

		/// <summary>
		/// Runs when the action has hit its scheduled time
		/// </summary>
		public abstract void RunFinal();

		/// <summary>
		/// Runs after RunFinal in order to set the next run time of this action
		/// </summary>
		public abstract DateTime? GetNextRunTimeUtc();
	}
}