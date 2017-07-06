using System;
using System.Text;

namespace ICD.Common.Utils.Extensions
{
	public static class StringBuilderExtensions
	{
		/// <summary>
		/// Empties the StringBuilder.
		/// </summary>
		/// <param name="extends"></param>
		public static void Clear(this StringBuilder extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			extends.Remove(0, extends.Length);
		}

		/// <summary>
		/// Returns the current string value of the StringBuilder and clears the
		///	StringBuilder for further use.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static string Pop(this StringBuilder extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			string output = extends.ToString();
			extends.Clear();
			return output;
		}
	}
}
