using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils.Comparers
{
	/// <summary>
	/// Undefined Versions have a value of 0.0.-1.-1
	/// This comparer Maxs Versions to 0.0.0.0
	/// </summary>
	public sealed class UndefinedVersionEqualityComparer : IEqualityComparer<Version>
	{
		private static UndefinedVersionEqualityComparer s_Instance;

		public static UndefinedVersionEqualityComparer Instance
		{
			get { return s_Instance = s_Instance ?? new UndefinedVersionEqualityComparer(); }
		}

		public bool Equals(Version x, Version y)
		{
			return x.ClearUndefined()
			        .Equals(y.ClearUndefined());
		}

		public int GetHashCode(Version version)
		{
			return version.ClearUndefined()
			              .GetHashCode();
		}
	}
}
