using System;

namespace ICD.Common.Utils
{
	public sealed partial class SafeCriticalSection
	{
		/// <summary>
		/// Enters the critical section, executes the callback and leaves the section.
		/// </summary>
		/// <param name="callback"></param>
		public void Execute(Action callback)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");

			try
			{
				Enter();
				callback();
			}
			finally
			{
				Leave();
			}
		}

		/// <summary>
		/// Enters the critical section, executes the callback and leaves the section.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="callback"></param>
		/// <returns></returns>
		public T Execute<T>(Func<T> callback)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");

			try
			{
				Enter();
				return callback();
			}
			finally
			{
				Leave();
			}
		}
	}
}
