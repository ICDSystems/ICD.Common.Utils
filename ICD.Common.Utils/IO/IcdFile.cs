using System;
using System.Text;
using ICD.Common.Properties;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
#else
using System.IO;
#endif

namespace ICD.Common.Utils.IO
{
	public static class IcdFile
	{
		[PublicAPI]
		public static string ReadToEnd(string path, Encoding encoding)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			if (encoding == null)
				throw new ArgumentNullException("encoding");

#if SIMPLSHARP
			return File.ReadToEnd(path, encoding);
#else
            return File.ReadAllText(path, encoding);
#endif
		}

		[PublicAPI]
		public static DateTime GetLastWriteTime(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			return File.GetLastWriteTime(path);
		}

		[PublicAPI]
		public static bool Exists(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			return File.Exists(path);
		}

		[PublicAPI]
		public static void Copy(string pathFrom, string pathTo)
		{
			if (pathFrom == null)
				throw new ArgumentNullException("pathFrom");

			if (pathTo == null)
				throw new ArgumentNullException("pathTo");

			File.Copy(pathFrom, pathTo);
		}

		[PublicAPI]
		public static void Delete(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			File.Delete(path);
		}

		[PublicAPI]
		public static IcdFileStream OpenWrite(string path)
		{
			return new IcdFileStream(File.OpenWrite(path));
		}

		[PublicAPI]
		public static IcdFileStream Open(string path, FileMode mode)
		{
			return new IcdFileStream(File.Open(path, mode));
		}

		[PublicAPI]
		public static DateTime GetCreationTime(string path)
		{
			return File.GetCreationTime(path);
		}
	}
}
