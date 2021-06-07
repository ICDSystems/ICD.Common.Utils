using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils.Comparers
{
	/// <summary>
	/// Undefined Versions have a value of 0.0.-1.-1
	/// This comparer Maxs Versions to 0.0.0.0
	/// </summary>
	public sealed class UndefinedVersionComparer : IComparer<Version>
	{
		private static UndefinedVersionComparer s_Instance;

		public static UndefinedVersionComparer Instance
		{
			get { return s_Instance = s_Instance ?? new UndefinedVersionComparer(); }
		}

		public int Compare(Version x, Version y)
		{
			if (x == null && y == null)
				return 0;
			if (x == null)
				return -1;
			if (y == null)
				return 1;

			return x.ClearUndefined()
			        .CompareTo(y.ClearUndefined());
		}
	}
}
