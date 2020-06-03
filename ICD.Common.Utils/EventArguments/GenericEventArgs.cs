using System;

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
}
