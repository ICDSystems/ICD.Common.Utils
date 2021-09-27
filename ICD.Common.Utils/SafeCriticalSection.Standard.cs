#if !SIMPLSHARP
using System.Threading;

namespace ICD.Common.Utils
{
	public sealed partial class SafeCriticalSection
	{
		#region Methods

		/// <summary>
		/// Block until ownership of the critical section can be obtained.
		/// </summary>
		public void Enter()
		{
			Monitor.Enter(this);
		}

		/// <summary>
		/// Release ownership of the critical section.
		/// </summary>
		public void Leave()
		{
			if (Monitor.IsEntered(this))
				Monitor.Exit(this);
		}

		#endregion
	}
}

#endif
