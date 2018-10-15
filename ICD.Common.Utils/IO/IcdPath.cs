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
