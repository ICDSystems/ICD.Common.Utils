namespace ICD.Common.EventArguments
{
	public sealed class IntEventArgs : GenericEventArgs<int>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		public IntEventArgs(int data) : base(data)
		{
		}
	}
}
