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
		public static string ReadToEnd([NotNull] string path, [NotNull] Encoding encoding)
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
		public static DateTime GetLastWriteTime([NotNull] string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			return File.GetLastWriteTime(path);
		}

		[PublicAPI]
		public static bool Exists([CanBeNull] string path)
		{
			// Consistent with Net Standard
			if (path == null)
				return false;

			try
			{
				return File.Exists(path);
			}
			// Crestron's AdjustPathForMono method throws an exception that is inconsistent with Net Standard...
			catch (Exception)
			{
				return false;
			}
		}

		[PublicAPI]
		public static void Copy([NotNull] string pathFrom, [NotNull] string pathTo)
		{
			if (pathFrom == null)
				throw new ArgumentNullException("pathFrom");

			if (pathTo == null)
				throw new ArgumentNullException("pathTo");

			File.Copy(pathFrom, pathTo);
		}

		[PublicAPI]
		public static void Delete([NotNull] string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			File.Delete(path);
		}

		public static IcdStream OpenRead(string path)
		{
			return new IcdFileStream(File.OpenRead(path));
		}

		[PublicAPI]
		public static IcdFileStream OpenWrite(string path)
		{
			return new IcdFileStream(File.OpenWrite(path));
		}

		[PublicAPI]
		public static IcdFileStream Open(string path, eIcdFileMode mode)
		{
			return new IcdFileStream(File.Open(path, mode.ToFileMode()));
		}

		[PublicAPI]
		public static DateTime GetCreationTime(string path)
		{
			return File.GetCreationTime(path);
		}

		[PublicAPI]
		public static IcdFileStream Create(string path)
		{
			return new IcdFileStream(File.Create(path));
		}

		[PublicAPI]
		public static IcdStreamWriter AppendText(string path)
		{
			return new IcdStreamWriter(File.AppendText(path));
		}

		public static void WriteAllBytes(string path, byte[] bytes)
		{
			using (FileStream stream = File.OpenWrite(path))
				stream.Write(bytes, 0, bytes.Length);
		}

		public static void Move([NotNull] string sourceFileName, [NotNull] string destFileName)
		{
			File.Move(sourceFileName, destFileName);
		}
	}
}
