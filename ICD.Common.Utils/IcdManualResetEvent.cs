using ICD.Common.Properties;
#if SIMPLSHARP
using Crestron.SimplSharp;
#else
using System.Threading;
#endif

namespace ICD.Common.Utils
{
	/// <summary>
	/// Notifies one or more waiting threads that an event has occurred.
	/// </summary>
	[PublicAPI]
	public sealed class IcdManualResetEvent : AbstractIcdResetEvent
	{

		/// <summary>
		/// Initializes a new instance of the IcdManualResetEvent class with the initial state to nonsignaled.
		/// </summary>
		[PublicAPI]
		public IcdManualResetEvent() : this(false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the IcdManualResetEvent class with a Boolean value indicating whether to set the initial state to signaled.
		/// </summary>
		/// <param name="initialState">true to set the initial state signaled; false to set the initial state to nonsignaled.</param>
		[PublicAPI]
		public IcdManualResetEvent(bool initialState) :
		
#if SIMPLSHARP
			base(new CEvent(false, initialState))
#else
			base(new ManualResetEvent(initialState))
#endif
		{
		}
	}
}