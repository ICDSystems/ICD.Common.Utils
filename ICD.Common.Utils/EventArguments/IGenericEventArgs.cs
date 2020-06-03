namespace ICD.Common.Utils.EventArguments
{
	public interface IGenericEventArgs
	{
		/// <summary>
		/// Gets the wrapped data associated with the event.
		/// </summary>
		object Data { get; }
	}

	public interface IGenericEventArgs<T> : IGenericEventArgs
	{
		/// <summary>
		/// Gets the wrapped data associated with the event.
		/// </summary>
		new T Data { get; }
	}
}
