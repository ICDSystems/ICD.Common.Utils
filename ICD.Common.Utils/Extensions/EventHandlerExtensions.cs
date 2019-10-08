using System;
using ICD.Common.Properties;

namespace ICD.Common.Utils.Extensions
{
	/// <summary>
	/// Extension methods for EventHandlers.
	/// </summary>
	public static class EventHandlerExtensions
	{
		/// <summary>
		/// Raises the event safely. Simply skips if the handler is null.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="sender"></param>
		public static void Raise([CanBeNull] this EventHandler extends, object sender)
		{
			if (extends != null)
				extends(sender, EventArgs.Empty);
		}

		/// <summary>
		/// Raises the event safely. Simply skips if the handler is null.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extends"></param>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public static void Raise<T>([CanBeNull]this EventHandler<T> extends, object sender, [NotNull]T args)
			where T : EventArgs
		{
			if (args == null)
				throw new ArgumentNullException("args");

			if (extends != null)
				extends(sender, args);
		}
	}
}
