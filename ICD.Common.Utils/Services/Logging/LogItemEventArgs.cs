using ICD.Common.Utils.EventArguments;

namespace ICD.Common.Utils.Services.Logging
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
