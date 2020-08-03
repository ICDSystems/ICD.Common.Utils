using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils.EventArguments
{
	[PublicAPI]
	public sealed class UShortEventArgs : GenericEventArgs<ushort>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		public UShortEventArgs(ushort data) : base(data)
		{
		}
	}

	public static class UShortEventArgsExtensions
	{
		/// <summary>
		/// Raises the event safely. Simply skips if the handler is null.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="sender"></param>
		/// <param name="data"></param>
		public static void Raise([CanBeNull]this EventHandler<UShortEventArgs> extends, object sender, ushort data)
		{
			extends.Raise(sender, new UShortEventArgs(data));
		}
	}
}
