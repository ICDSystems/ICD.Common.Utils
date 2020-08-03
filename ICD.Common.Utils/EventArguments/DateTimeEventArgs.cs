using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils.EventArguments
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

	public static class DateTimeEventArgsExtensions
	{
		/// <summary>
		/// Raises the event safely. Simply skips if the handler is null.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="sender"></param>
		/// <param name="data"></param>
		public static void Raise([CanBeNull]this EventHandler<DateTimeEventArgs> extends, object sender, DateTime data)
		{
			extends.Raise(sender, new DateTimeEventArgs(data));
		}
	}
}
