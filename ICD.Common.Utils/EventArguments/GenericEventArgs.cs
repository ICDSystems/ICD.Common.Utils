using System;

namespace ICD.Common.EventArguments
{
	public abstract class GenericEventArgs<T> : EventArgs
	{
		public T Data { get; private set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		protected GenericEventArgs(T data)
		{
			Data = data;
		}
	}
}
