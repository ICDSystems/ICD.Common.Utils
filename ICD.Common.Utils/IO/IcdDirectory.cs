using System;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
#else
using ICD.Common.Utils.Extensions;
using System.IO;
using System.Reflection;
#endif

namespace ICD.Common.Utils.IO
{
	public static class IcdDirectory
	{
		public static string GetApplicationDirectory()
		{
#if SIMPLSHARP
			return Directory.GetApplicationDirectory();
#else
			string pathToDll = Assembly.GetExecutingAssembly().GetPath();
			return pathToDll == null ? null : IcdPath.GetDirectoryName(pathToDll);
#endif
		}

		/// <summary>
		/// This gets the application root directory for Crestron systems
		/// </summary>
		/// <returns></returns>
		public static string GetApplicationRootDirectory()
		{
#if SIMPLSHARP
			return Directory.GetApplicationRootDirectory();
#else
			return Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetPath());
#endif
		}

		public static bool Exists(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			return Directory.Exists(path);
		}

		public static string[] GetFiles(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			return Directory.GetFiles(path);
		}

		public static string[] GetFiles(string path, string filter)
		{
			if (path == null)
				throw new ArgumentException("path");

			return Directory.GetFiles(path, filter);
		}

		public static string[] GetDirectories(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			return Directory.GetDirectories(path);
		}

		public static void Delete(string path, bool recursive)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			Directory.Delete(path, recursive);
		}

		public static void CreateDirectory(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			Directory.CreateDirectory(path);
		}

		public static string GetDirectoryRoot(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			return Directory.GetDirectoryRoot(path);
		}
	}
}
