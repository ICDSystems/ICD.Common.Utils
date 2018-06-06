using System.Collections.Generic;
using ICD.Common.Utils.IO;

namespace ICD.Common.Utils.Comparers
{
	public sealed class FileNameEqualityComparer : IEqualityComparer<string>
	{
		private static FileNameEqualityComparer s_Instance;

		public static FileNameEqualityComparer Instance
		{
			get { return s_Instance ?? (s_Instance = new FileNameEqualityComparer()); }
		}

		private FileNameEqualityComparer()
		{
		}

		public bool Equals(string x, string y)
		{
			return GetHashCode(x) == GetHashCode(y);
		}

		public int GetHashCode(string obj)
		{
			return obj == null ? 0 : IcdPath.GetFileName(obj).GetHashCode();
		}
	}
}
