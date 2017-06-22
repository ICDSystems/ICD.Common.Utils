#if SIMPLSHARP
using Crestron.SimplSharp;

#else
using System.Diagnostics;
#endif

namespace ICD.Common.Utils.Timers
{
	public sealed class IcdStopwatch
	{
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

		public void Stop()
		{
			m_Stopwatch.Stop();
		}

		public void Start()
		{
			m_Stopwatch.Start();
		}

		public void Reset()
		{
			m_Stopwatch.Reset();
		}

		#endregion
	}
}
