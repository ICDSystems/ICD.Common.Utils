using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils.EventArguments
{
	public class GenericEventArgs<T> : EventArgs, IGenericEventArgs<T>
	{
		private readonly T m_Data;

		/// <summary>
		/// Gets the wrapped data associated with the event.
		/// </summary>
		object IGenericEventArgs.Data { get { return Data; } }

		/// <summary>
		/// Gets the wrapped data associated with the event.
		/// </summary>
		public T Data { get { return m_Data; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		public GenericEventArgs(T data)
		{
			m_Data = data;
		}
	}

	public static class GenericEventArgsExtensions
	{
		/// <summary>
		/// Raises the event safely. Simply skips if the handler is null.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="sender"></param>
		/// <param name="data"></param>
		public static void Raise<T>([CanBeNull]this EventHandler<GenericEventArgs<T>> extends, object sender, T data)
		{
			extends.Raise(sender, new GenericEventArgs<T>(data));
		}
	}
}
