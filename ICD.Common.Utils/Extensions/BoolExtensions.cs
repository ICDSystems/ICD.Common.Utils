namespace ICD.Common.Utils.Extensions
{
	public static class BoolExtensions
	{
		public static ushort ToUShort(this bool b)
		{
			return b ? (ushort)1 : (ushort)0;
		}
	}
}