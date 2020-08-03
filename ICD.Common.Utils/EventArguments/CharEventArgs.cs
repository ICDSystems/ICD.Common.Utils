using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils.EventArguments
{
	[PublicAPI]
	public sealed class CharEventArgs : GenericEventArgs<char>
	{
		public CharEventArgs(char value) : base(value)
		{
		}
	}

	public static class CharEventArgsExtensions
	{
		/// <summary>
		/// Raises the event safely. Simply skips if the handler is null.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="sender"></param>
		/// <param name="data"></param>
		public static void Raise([CanBeNull]this EventHandler<CharEventArgs> extends, object sender, char data)
		{
			extends.Raise(sender, new CharEventArgs(data));
		}
	}
}
