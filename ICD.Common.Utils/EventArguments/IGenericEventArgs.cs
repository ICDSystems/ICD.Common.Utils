namespace ICD.Common.Utils.EventArguments
{
	public interface IGenericEventArgs<T>
	{
		/// <summary>
		/// Gets the wrapped data associated with the event.
		/// </summary>
		T Data { get; }
	}
}
