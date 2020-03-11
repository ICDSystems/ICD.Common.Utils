using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
#if SIMPLSHARP
using Crestron.SimplSharp;
using Crestron.SimplSharp.Reflection;
#else
using System.Threading.Tasks;
using MethodInfo = System.Reflection.MethodInfo;
#endif

namespace ICD.Common.Utils
{
	public static class ThreadingUtils
	{
		private static readonly IcdHashSet<ThreadState> s_Threads;
		private static readonly SafeCriticalSection s_ThreadsSection;

		/// <summary>
		/// Static contstructor.
		/// </summary>
		static ThreadingUtils()
		{
			s_Threads = new IcdHashSet<ThreadState>();
			s_ThreadsSection = new SafeCriticalSection();
		}

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

			DateTime end = IcdEnvironment.GetUtcTime().AddMilliseconds(timeout);

			while (!condition())
			{
				if (IcdEnvironment.GetUtcTime() >= end)
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
		public static void SafeInvoke(Action callback)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");

			SafeInvoke<object>(callback.GetMethodInfo(), unused => callback(), null);
		}

		/// <summary>
		/// Executes the callback as a short-lived, threaded task.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="callback"></param>
		/// <param name="param"></param>
		[PublicAPI]
		public static void SafeInvoke<T>(Action<T> callback, T param)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");

			SafeInvoke(callback.GetMethodInfo(), callback, param);
		}

		/// <summary>
		/// Executes the callback as a short-lived, threaded task.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="methodInfo"></param>
		/// <param name="callback"></param>
		/// <param name="param"></param>
		[PublicAPI]
		private static void SafeInvoke<T>(MethodInfo methodInfo, Action<T> callback, T param)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");

			ThreadState state = new ThreadState
			{
				MethodInfo = methodInfo,
			};
			state.Callback = GetHandledCallback(state, callback, param);

			// Add the state here so the created thread doesn't get garbage collected
			AddThreadState(state);

			state.Handle =
#if SIMPLSHARP
				CrestronInvoke.BeginInvoke(unused => state.Callback(), null);
#else
				Task.Run(state.Callback);
#endif
		}

		/// <summary>
		/// Wraps the given callback in a try/catch to avoid crashing crestron programs.
		/// http://www.crestronlabs.com/showthread.php?12205-Exception-in-CrestronInvoke-thread-crashes-the-program
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="state"></param>
		/// <param name="callback"></param>
		/// <param name="param"></param>
		private static Action GetHandledCallback<T>(ThreadState state, Action<T> callback, T param)
		{
			return () =>
			       {
				       try
				       {
					       state.Started = IcdEnvironment.GetUtcTime();

					       callback(param);
					       RemoveThreadState(state);
				       }
				       catch (Exception e)
				       {
					       ServiceProvider.TryGetService<ILoggerService>()
					                      .AddEntry(eSeverity.Error, e, "{0} failed to execute callback", typeof(ThreadingUtils).Name);
				       }
			       };
		}

		private static IEnumerable<ThreadState> GetActiveThreads()
		{
			return s_ThreadsSection.Execute(() => s_Threads.ToArray(s_Threads.Count));
		}

		private static void AddThreadState(ThreadState state)
		{
			s_ThreadsSection.Execute(() => s_Threads.Add(state));
		}

		private static void RemoveThreadState(ThreadState state)
		{
			s_ThreadsSection.Execute(() => s_Threads.Remove(state));
		}

		public static string PrintThreads()
		{
			TableBuilder builder = new TableBuilder("Name", "Started", "Duration");

			IEnumerable<ThreadState> states = GetActiveThreads().OrderBy(s => s.Name)
			                                                    .ThenBy(s => s.Started);

			foreach (ThreadState state in states)
				builder.AddRow(state.Name, state.Started, state.Duration);

			return builder.ToString();
		}

		private sealed class ThreadState
		{
			private string m_CachedName;

			/// <summary>
			/// Gets the time the thread was instantiated.
			/// </summary>
			public DateTime? Started { get; set; }

			/// <summary>
			/// Gets the duration of the thread.
			/// </summary>
			public TimeSpan? Duration { get { return IcdEnvironment.GetUtcTime() - Started; } }

			/// <summary>
			/// Threads can be garbage collected before they execute so we keep a reference.
			/// </summary>
			[UsedImplicitly]
			public object Handle { get; set; }

			/// <summary>
			/// Human readable name for the thread.
			/// </summary>
			public string Name { get { return m_CachedName = m_CachedName ?? BuildName(); } }

			/// <summary>
			/// The action that is being performed by the thread.
			/// </summary>
			public Action Callback { get; set; }

			/// <summary>
			/// Used for providing debug information on the executing method.
			/// </summary>
			public MethodInfo MethodInfo { get; set; }

			private string BuildName()
			{
				Type declaring = MethodInfo.DeclaringType;

				return declaring == null
					       ? MethodInfo.GetSignature(true)
					       : string.Format("{0}.{1}", declaring.Name, MethodInfo.GetSignature(true));
			}
		}
	}
}
