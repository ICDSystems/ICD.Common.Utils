using System;
#if SIMPLSHARP
using Crestron.SimplSharp;
#else
using System.Threading.Tasks;
#endif
using ICD.Common.Properties;
using ICD.Common.Services;
using ICD.Common.Services.Logging;

namespace ICD.Common.Utils
{
	public static class ThreadingUtils
	{
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
					                      .AddEntry(eSeverity.Error, e, e.Message);
				       }
			       };
		}
	}
}
