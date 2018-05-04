using System;

namespace ICD.Common.Utils.EventArguments
{
	public class GenericEventArgs<T> : EventArgs, IGenericEventArgs<T>
	{
		/// <summary>
		/// Gets the wrapped data associated with the event.
		/// </summary>
		public T Data { get; private set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		public GenericEventArgs(T data)
		{
			Data = data;
		}
	}
}
