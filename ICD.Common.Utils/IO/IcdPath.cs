using System;
using ICD.Common.Properties;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
#else
using System.IO;
#endif

namespace ICD.Common.Utils.IO
{
	public static class IcdPath
	{
		public static string GetFileName(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			return Path.GetFileName(path);
		}

		public static string GetFileNameWithoutExtension(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			return Path.GetFileNameWithoutExtension(path);
		}

		[CanBeNull]
		public static string GetDirectoryName(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			return Path.GetDirectoryName(path);
		}

		public static string GetExtension(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			return Path.GetExtension(path);
		}

		public static string Combine(string a, string b)
		{
			if (a == null)
				throw new ArgumentNullException("a");

			if (b == null)
				throw new ArgumentNullException("b");

			return Path.Combine(a, b);
		}

		public static string ChangeExtension(string path, string ext)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			if (ext == null)
				throw new ArgumentNullException("ext");

			return Path.ChangeExtension(path, ext);
		}
	}
}
