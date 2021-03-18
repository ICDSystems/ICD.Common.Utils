using System;

namespace ICD.Common.Utils.Extensions
{
	public static class VersionExtensions
	{
		/// <summary>
		/// Creates a new Version using 0 instead of -1 for omitted quadrants.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static Version ClearUndefined(this Version extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return new Version(Math.Max(0, extends.Major),
			                   Math.Max(0, extends.Minor),
			                   Math.Max(0, extends.Build),
			                   Math.Max(0, extends.Revision));
		}

		/// <summary>
		/// Formats the version to X.XXX.XXXX
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static string ToCrestronString(this Version extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return string.Format("{0}.{1:D3}.{2:D4}", extends.Major, extends.Minor, extends.Build);
		}
	}
}
