using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils.EventArguments
{
	public sealed class BoolEventArgs : GenericEventArgs<bool>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		public BoolEventArgs(bool data)
			: base(data)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="eventArgs"></param>
		public BoolEventArgs(BoolEventArgs eventArgs)
			: this(eventArgs.Data)
		{
		}
	}

	public static class BoolEventArgsExtensions
	{
		/// <summary>
		/// Raises the event safely. Simply skips if the handler is null.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="sender"></param>
		/// <param name="data"></param>
		public static void Raise([CanBeNull]this EventHandler<BoolEventArgs> extends, object sender, bool data)
		{
			extends.Raise(sender, new BoolEventArgs(data));
		}
	}
}
