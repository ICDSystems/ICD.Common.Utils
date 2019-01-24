namespace ICD.Common.Utils.EventArguments
{
	public sealed class StringEventArgs : GenericEventArgs<string>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		public StringEventArgs(string data) : base(data)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="eventArgs"></param>
		public StringEventArgs(StringEventArgs eventArgs)
			: base(eventArgs.Data)
		{
		}
	}
}
