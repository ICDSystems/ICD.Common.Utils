namespace ICD.Common.EventArguments
{
	public sealed class FloatEventArgs : GenericEventArgs<float>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		public FloatEventArgs(float data)
			: base(data)
		{
		}
	}
}
