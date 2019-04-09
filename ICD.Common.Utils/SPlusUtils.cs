using ICD.Common.Properties;

namespace ICD.Common.Utils
{
	[PublicAPI("S+")]
	public static class SPlusUtils
	{
		/// <summary>
		/// Convert two ushort's to an int
		/// </summary>
		/// <param name="lowWord">ushort for the least significant 16 bits</param>
		/// <param name="highWord">ushort for the most significant 1 bits</param>
		/// <returns></returns>
		[PublicAPI("S+")]
		public static int ConvertToInt(ushort lowWord, ushort highWord)
		{
			return (highWord << 16) + lowWord;
		}
	}
}