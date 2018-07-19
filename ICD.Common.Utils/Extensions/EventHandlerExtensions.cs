using System;
#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif

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
		public static void Raise(this EventHandler extends, object sender)
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
		public static void Raise<T>(this EventHandler<T> extends, object sender, T args)
			where T : EventArgs
		{
			if (args == null)
				throw new ArgumentNullException("args");

			if (extends != null)
				extends(sender, args);
		}

		/// <summary>
		/// Cross-platform shim for getting MethodInfo for the delegate.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static MethodInfo GetMethodInfo(this Delegate extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

#if SIMPLSHARP
			return extends.GetMethod();
#else
			return extends.Method;
#endif
		}
	}
}
