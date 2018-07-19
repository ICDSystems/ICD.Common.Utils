using System;
using System.Collections.Generic;
using ICD.Common.Properties;
#if SIMPLSHARP
using Crestron.SimplSharp;
#else
using System.Diagnostics;
#endif

namespace ICD.Common.Utils.Timers
{
	public sealed class IcdStopwatch
	{
		// Used with profiling
		private const long YELLOW_MILLISECONDS = 100;
		private const long RED_MILLISECONDS = 400;

		private readonly Stopwatch m_Stopwatch;

		#region Properties

		public long ElapsedMilliseconds { get { return m_Stopwatch.ElapsedMilliseconds; } }

		public bool IsRunning { get { return m_Stopwatch.IsRunning; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public IcdStopwatch()
			: this(new Stopwatch())
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="stopwatch"></param>
		public IcdStopwatch(Stopwatch stopwatch)
		{
			m_Stopwatch = stopwatch;
		}

		public static IcdStopwatch StartNew()
		{
			return new IcdStopwatch(Stopwatch.StartNew());
		}

		#endregion

		#region Methods

		/// <summary>
		/// Stops the stopwatch at the current elapsed time.
		/// </summary>
		public void Stop()
		{
			m_Stopwatch.Stop();
		}

		/// <summary>
		/// Starts or resumes the stopwatch from the current elapsed time.
		/// </summary>
		public void Start()
		{
			m_Stopwatch.Start();
		}

		/// <summary>
		/// Stops the stopwatch and resets the elapsed time to 0.
		/// </summary>
		public void Reset()
		{
			m_Stopwatch.Reset();
		}

		/// <summary>
		/// Resets the stopwatch and starts again from an elapsed time of 0.
		/// </summary>
		public void Restart()
		{
			Reset();
			Start();
		}

		#endregion

		#region Profiling

		/// <summary>
		/// Executes the action and prints the duration to the console.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="name"></param>
		[PublicAPI]
		public static void Profile(Action action, string name)
		{
			if (action == null)
				throw new ArgumentNullException("action");

			IcdStopwatch stopwatch = StartNew();
			action();
			PrintProfile(stopwatch, name);
		}

		/// <summary>
		/// Executes the function and prints the duration to the console.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="func"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		[PublicAPI]
		public static T Profile<T>(Func<T> func, string name)
		{
			if (func == null)
				throw new ArgumentNullException("func");

			T output = default(T);

			Profile(() =>
			        {
				        output = func();
			        }, name);

			return output;
		}

		/// <summary>
		/// Profiles getting each item from the enumerable.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static IEnumerable<T> Profile<T>(IEnumerable<T> enumerable, string name)
		{
			using (IEnumerator<T> enumerator = enumerable.GetEnumerator())
			{
// ReSharper disable once AccessToDisposedClosure
				while (Profile(() => enumerator.MoveNext(), name))
					yield return enumerator.Current;
			}
		}

		/// <summary>
		/// Executes the event handler and profiles each of the attached subscribers.
		/// </summary>
		/// <param name="eventHandler"></param>
		/// <param name="sender"></param>
		/// <param name="name"></param>
		public static void Profile(EventHandler eventHandler, object sender, string name)
		{
			if (eventHandler == null)
			{
				PrintProfile(new IcdStopwatch(), string.Format("{0} - No invocations", name));
				return;
			}

// ReSharper disable PossibleInvalidCastExceptionInForeachLoop
			foreach (EventHandler subscriber in eventHandler.GetInvocationList())
// ReSharper restore PossibleInvalidCastExceptionInForeachLoop
			{
				string subscriberName = string.Format("{0} - {1}", name, subscriber.Target.GetType().Name);

				EventHandler subscriber1 = subscriber;
				Profile(() => subscriber1(sender, EventArgs.Empty), subscriberName);
			}
		}

		/// <summary>
		/// Executes the event handler and profiles each of the attached subscribers.
		/// </summary>
		/// <param name="eventHandler"></param>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		/// <param name="name"></param>
		public static void Profile<T>(EventHandler<T> eventHandler, object sender, T eventArgs, string name)
			where T : EventArgs
		{
			if (eventHandler == null)
			{
				PrintProfile(new IcdStopwatch(), string.Format("{0} - No invocations", name));
				return;
			}

// ReSharper disable PossibleInvalidCastExceptionInForeachLoop
			foreach (EventHandler<T> subscriber in eventHandler.GetInvocationList())
// ReSharper restore PossibleInvalidCastExceptionInForeachLoop
			{
				string subscriberName = string.Format("{0} - {1}", name, subscriber.Target.GetType().Name);

				EventHandler<T> subscriber1 = subscriber;
				Profile(() => subscriber1(sender, eventArgs), subscriberName);
			}
		}

		private static void PrintProfile(IcdStopwatch stopwatch, string name)
		{
			long elapsed = stopwatch.ElapsedMilliseconds;

			eConsoleColor color = eConsoleColor.Green;
			if (elapsed >= YELLOW_MILLISECONDS)
				color = eConsoleColor.Yellow;
			if (elapsed >= RED_MILLISECONDS)
				color = eConsoleColor.Red;

			IcdConsole.Print(color, "{0}ms", elapsed);
			IcdConsole.PrintLine(" to execute {0}", name);
		}

		#endregion
	}
}
