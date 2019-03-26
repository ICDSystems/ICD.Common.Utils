namespace ICD.Common.Utils.EventArguments
{
	public sealed class BoolEventArgs : GenericEventArgs<bool>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		public BoolEventArgs(bool data)
			: base(data)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="eventArgs"></param>
		public BoolEventArgs(BoolEventArgs eventArgs)
			: this(eventArgs.Data)
		{
		}
	}
}
