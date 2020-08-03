using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils.EventArguments
{
	public sealed class FloatEventArgs : GenericEventArgs<float>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		public FloatEventArgs(float data)
			: base(data)
		{
		}
	}

	public static class FloatEventArgsExtensions
	{
		/// <summary>
		/// Raises the event safely. Simply skips if the handler is null.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="sender"></param>
		/// <param name="data"></param>
		public static void Raise([CanBeNull]this EventHandler<FloatEventArgs> extends, object sender, float data)
		{
			extends.Raise(sender, new FloatEventArgs(data));
		}
	}
}
