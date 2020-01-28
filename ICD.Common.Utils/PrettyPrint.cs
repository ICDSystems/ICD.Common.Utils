using ICD.Common.Properties;
using ICD.Common.Utils.Json;

namespace ICD.Common.Utils
{
	/// <summary>
	/// Utils for printing various data types in human readable structures.
	/// </summary>
	public static class PrettyPrint
	{
		#region Methods

		[PublicAPI]
		public static void PrintLine([CanBeNull] object value)
		{
			string serial = ToString(value);
			IcdConsole.PrintLine(serial);
		}

		[PublicAPI]
		public static string ToString([CanBeNull] object value)
		{
			return JsonUtils.Format(value);
		}

		#endregion
	}
}
