using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
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

		private static int s_ProfileIndentCount;

		#region Properties

		public long ElapsedTicks { get { return m_Stopwatch.ElapsedTicks; } }

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

			s_ProfileIndentCount++;

			try
			{
				IcdStopwatch stopwatch = StartNew();
				action();
				PrintProfile(stopwatch.ElapsedTicks, name);
			}
			finally
			{
				s_ProfileIndentCount--;
			}
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

			s_ProfileIndentCount++;

			try
			{
				IcdStopwatch stopwatch = StartNew();
				T output = func();
				PrintProfile(stopwatch.ElapsedTicks, name);

				return output;
			}
			finally
			{
				s_ProfileIndentCount--;
			}
		}

		/// <summary>
		/// Executes the action the given number of iterations and prints timing information to the console.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="iterations"></param>
		/// <param name="name"></param>
		public static void Profile(Action action, int iterations, string name)
		{
			if (action == null)
				throw new ArgumentNullException("action");

			if (iterations <= 0)
				throw new ArgumentOutOfRangeException("iterations", "Iterations must be greater than 0");

			name = string.Format("{0} ({1} Iterations)", name, iterations);

			long totalTicks = 0;
			List<long> orderedMs = new List<long>(iterations);

			for (int index = 0; index < iterations; index++)
			{
				IcdStopwatch stopwatch = StartNew();
				{
					action();
				}
				long duration = stopwatch.ElapsedTicks;
				stopwatch.Stop();

				orderedMs.InsertSorted(duration);
				totalTicks += duration;
			}

			float totalMs = totalTicks / (float)TimeSpan.TicksPerMillisecond;
			float averageMs = (totalTicks / (float)iterations) / TimeSpan.TicksPerMillisecond;
			float medianMs = (orderedMs[iterations / 2]) / (float)TimeSpan.TicksPerMillisecond;
			float shortestMs = orderedMs[0] / (float)TimeSpan.TicksPerMillisecond;
			float longestMs = orderedMs[iterations - 1] / (float)TimeSpan.TicksPerMillisecond;

			TableBuilder builder = new TableBuilder(name, "Duration (ms)");

			builder.AddRow("Total", string.Format("{0:n}", totalMs));
			builder.AddRow("Average", string.Format("{0:n}", averageMs));
			builder.AddRow("Median", string.Format("{0:n}", medianMs));
			builder.AddRow("Shortest", string.Format("{0:n}", shortestMs));
			builder.AddRow("Longest", string.Format("{0:n}", longestMs));

			IcdConsole.PrintLine(builder.ToString());
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
			if (enumerable == null)
				throw new ArgumentNullException("enumerable");

			// TODO - Print a fancy table with a total duration for the sequence

			List<T> output = new List<T>();

			using (IEnumerator<T> enumerator = enumerable.GetEnumerator())
			{
// ReSharper disable once AccessToDisposedClosure
				while (Profile(() => enumerator.MoveNext(), name))
					output.Add(enumerator.Current);
			}

			return output;
		}

		/// <summary>
		/// Executes the event handler and profiles each of the attached subscribers.
		/// </summary>
		/// <param name="eventHandler"></param>
		/// <param name="sender"></param>
		/// <param name="name"></param>
		public static void Profile(EventHandler eventHandler, object sender, string name)
		{
			s_ProfileIndentCount++;

			try
			{
				if (eventHandler == null)
				{
					PrintProfile(0, string.Format("{0} - No invocations", name));
					return;
				}
			}
			finally
			{
				s_ProfileIndentCount--;
			}

			// TODO - Print a fancy table with a total duration for the event

			foreach (EventHandler subscriber in eventHandler.GetInvocationList().Cast<EventHandler>())
			{
				string subscriberName = string.Format("{0} - {1}.{2}", name, subscriber.Target.GetType().Name,
				                                      subscriber.GetMethodInfo().GetSignature(true));

				EventHandler subscriber1 = subscriber;
				Profile(() => subscriber1(sender, EventArgs.Empty), subscriberName);
			}
		}

		/// <summary>
		/// Executes the event handler and profiles each of the attached subscribers.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="eventHandler"></param>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		/// <param name="name"></param>
		public static void Profile<T>(EventHandler<T> eventHandler, object sender, T eventArgs, string name)
			where T : EventArgs
		{
			s_ProfileIndentCount++;

			try
			{
				if (eventHandler == null)
				{
					PrintProfile(0, string.Format("{0} - No invocations", name));
					return;
				}
			}
			finally
			{
				s_ProfileIndentCount--;
			}

			// TODO - Print a fancy table with a total duration for the event

			foreach (EventHandler<T> subscriber in eventHandler.GetInvocationList().Cast<EventHandler<T>>())
			{
				string subscriberName = string.Format("{0} - {1}.{2}", name, subscriber.Target.GetType().Name,
				                                      subscriber.GetMethodInfo().GetSignature(true));

				EventHandler<T> subscriber1 = subscriber;
				Profile(() => subscriber1(sender, eventArgs), subscriberName);
			}
		}

		private static void PrintProfile(long ticks, string name)
		{
			float elapsed = ticks / (float)TimeSpan.TicksPerMillisecond;

			eConsoleColor color = eConsoleColor.Green;
			if (elapsed >= YELLOW_MILLISECONDS)
				color = eConsoleColor.Yellow;
			if (elapsed >= RED_MILLISECONDS)
				color = eConsoleColor.Red;

			string padding = null;

			if (s_ProfileIndentCount > 0)
				padding = StringUtils.Repeat("  ", s_ProfileIndentCount - 1);

			IcdConsole.Print(color, "{0}{1:n}ms ", padding, elapsed);
			IcdConsole.PrintLine("to execute {0}", name);
		}

		#endregion
	}
}
