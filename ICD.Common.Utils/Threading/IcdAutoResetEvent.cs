using ICD.Common.Properties;
#if SIMPLSHARP
using Crestron.SimplSharp;
# else
using System.Threading;
#endif

namespace ICD.Common.Utils
{
	/// <summary>
	/// Notifies one or more waiting threads that an event has occurred.  Every thread that passes WaitOne causes the event to be reset
	/// </summary>
	[PublicAPI]
	public sealed class IcdAutoResetEvent : AbstractIcdResetEvent
	{

		/// <summary>
		/// Initializes a new instance of the IcdAutoResetEvent class with the initial state to nonsignaled.
		/// </summary>
		[PublicAPI]
		public IcdAutoResetEvent() : this(false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the IcdAutoResetEvent class with a Boolean value indicating whether to set the initial state to signaled.
		/// </summary>
		/// <param name="initialState">true to set the initial state signaled; false to set the initial state to nonsignaled.</param>
		[PublicAPI]
		public IcdAutoResetEvent(bool initialState) : 
#if SIMPLSHARP
			base(new CEvent(true, initialState))
#else
			base(new AutoResetEvent(initialState))
#endif
		{
		}
	}
}