using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
#else
using System.IO;
#endif

namespace ICD.Common.Utils.IO
{
	public static class IcdPath
	{
		public static char DirectorySeparatorChar { get { return Path.DirectorySeparatorChar; } }

		public static char AltDirectorySeparatorChar { get { return Path.AltDirectorySeparatorChar; } }

		public static string GetFileName([NotNull] string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			return Path.GetFileName(path);
		}

		public static string GetFileNameWithoutExtension([NotNull] string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			return Path.GetFileNameWithoutExtension(path);
		}

		[CanBeNull]
		public static string GetDirectoryName([NotNull] string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			return Path.GetDirectoryName(path);
		}

		public static string GetExtension([NotNull] string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			return Path.GetExtension(path);
		}

		public static string Combine([NotNull] string a, [NotNull] string b)
		{
			if (a == null)
				throw new ArgumentNullException("a");

			if (b == null)
				throw new ArgumentNullException("b");

			return Path.Combine(a, b);
		}

		public static string ChangeExtension([NotNull] string path, [NotNull] string ext)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			if (ext == null)
				throw new ArgumentNullException("ext");

			return Path.ChangeExtension(path, ext);
		}

		public static string GetRelativePath(string folder, string filespec)
		{
			Uri pathUri = new Uri(filespec);

			// Folders must end in a slash
			if (!folder.EndsWith(Path.DirectorySeparatorChar))
				folder += Path.DirectorySeparatorChar;

			Uri folderUri = new Uri(folder);
			return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
		}
	}
}
