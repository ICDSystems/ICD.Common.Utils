using ICD.Common.EventArguments;

namespace ICD.Common.Services.Logging
{
	public sealed class LogItemEventArgs : GenericEventArgs<LogItem>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="item"></param>
		public LogItemEventArgs(LogItem item)
			: base(item)
		{
		}
	}
}
