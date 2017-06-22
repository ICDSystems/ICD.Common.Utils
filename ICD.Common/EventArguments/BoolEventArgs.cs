namespace ICD.Common.EventArguments
{
	public sealed class BoolEventArgs : GenericEventArgs<bool>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		public BoolEventArgs(bool data) : base(data)
		{
		}
	}
}
