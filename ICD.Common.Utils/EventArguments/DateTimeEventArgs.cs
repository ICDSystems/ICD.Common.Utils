using System;

namespace ICD.Common.EventArguments
{
	public sealed class DateTimeEventArgs : GenericEventArgs<DateTime>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		public DateTimeEventArgs(DateTime data)
			: base(data)
		{
		}
	}
}
