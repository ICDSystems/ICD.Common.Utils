using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
#if SIMPLSHARP
using Crestron.SimplSharp;
#else
using System.Threading.Tasks;
#endif

namespace ICD.Common.Utils
{
	public static class ThreadingUtils
	{
		/// <summary>
		/// Wait until the given condition is true.
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="timeout"></param>
		/// <returns>False if the call times out</returns>
		public static bool Wait(Func<bool> condition, long timeout)
		{
			if (condition == null)
				throw new ArgumentNullException("condition");

			DateTime end = IcdEnvironment.GetLocalTime().AddMilliseconds(timeout);

			while (!condition())
			{
				if (IcdEnvironment.GetLocalTime() >= end)
					return false;
			}

			return true;
		}

		/// <summary>
		/// Puts the current thread to sleep for the given amount of time.
		/// </summary>
		/// <param name="milliseconds"></param>
		public static void Sleep(int milliseconds)
		{
#if SIMPLSHARP
			CrestronEnvironment.Sleep(milliseconds);
#else
			Task.Delay(TimeSpan.FromMilliseconds(milliseconds)).Wait();
#endif
		}

		/// <summary>
		/// Executes the callback as a short-lived, threaded task.
		/// </summary>
		/// <param name="callback"></param>
		[PublicAPI]
		public static object SafeInvoke(Action callback)
		{
			return SafeInvoke<object>(unused => callback(), null);
		}

		/// <summary>
		/// Executes the callback as a short-lived, threaded task.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="callback"></param>
		/// <param name="param"></param>
		[PublicAPI]
		public static object SafeInvoke<T>(Action<T> callback, T param)
		{
#if SIMPLSHARP
			return CrestronInvoke.BeginInvoke(unused => GetHandledCallback(callback, param)(), null);
#else
            return Task.Run(GetHandledCallback(callback, param));
#endif
		}

		/// <summary>
		/// Wraps the given callback in a try/catch to avoid crashing crestron programs.
		/// http://www.crestronlabs.com/showthread.php?12205-Exception-in-CrestronInvoke-thread-crashes-the-program
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="callback"></param>
		/// <param name="param"></param>
		private static Action GetHandledCallback<T>(Action<T> callback, T param)
		{
			return () =>
			       {
				       try
				       {
					       callback(param);
				       }
				       catch (Exception e)
				       {
					       ServiceProvider.TryGetService<ILoggerService>()
					                      .AddEntry(eSeverity.Error, e, "{0} failed to execute callback", typeof(ThreadingUtils).Name);
				       }
			       };
		}
	}
}
