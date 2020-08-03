using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils.EventArguments
{
	public sealed class IntEventArgs : GenericEventArgs<int>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		public IntEventArgs(int data) : base(data)
		{
		}
	}

	public static class IntEventArgsExtensions
	{
		/// <summary>
		/// Raises the event safely. Simply skips if the handler is null.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="sender"></param>
		/// <param name="data"></param>
		public static void Raise([CanBeNull]this EventHandler<IntEventArgs> extends, object sender, int data)
		{
			extends.Raise(sender, new IntEventArgs(data));
		}
	}
}
