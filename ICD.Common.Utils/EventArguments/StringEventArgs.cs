using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils.EventArguments
{
	public sealed class StringEventArgs : GenericEventArgs<string>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		public StringEventArgs(string data) : base(data)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="eventArgs"></param>
		public StringEventArgs(StringEventArgs eventArgs)
			: base(eventArgs.Data)
		{
		}
	}

	public static class StringEventArgsExtensions
	{
		/// <summary>
		/// Raises the event safely. Simply skips if the handler is null.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="sender"></param>
		/// <param name="data"></param>
		public static void Raise([CanBeNull]this EventHandler<StringEventArgs> extends, object sender, string data)
		{
			extends.Raise(sender, new StringEventArgs(data));
		}
	}
}
