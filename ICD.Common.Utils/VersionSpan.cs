using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils
{
	/// <summary>
	/// Describes the difference between two versions
	/// </summary>
	public sealed class VersionSpan
	{
		public Version Start { get; set; }
		public Version End { get; set; }

		/// <summary>
		/// Returns true if the given version is included in the span, inclusively.
		/// </summary>
		/// <param name="version"></param>
		/// <returns></returns>
		public bool Contains([NotNull] Version version)
		{
			if (version == null)
				throw new ArgumentNullException("version");

			return version.ClearUndefined() >= Start.ClearUndefined() &&
			       version.ClearUndefined() <= End.ClearUndefined();
		}
	}
}
