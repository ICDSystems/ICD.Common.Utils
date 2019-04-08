using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.IO;

namespace ICD.Common.Utils
{
	/// <summary>
	/// Provides util methods for working with file/directory paths.
	/// </summary>
	public static class PathUtils
	{
		#region Properties

		/// <summary>
		/// Gets the path to the root directory of the processor.
		/// </summary>
		[PublicAPI]
		public static string RootPath {
			get
			{
				if (IcdEnvironment.RuntimeEnvironment == IcdEnvironment.eRuntimeEnvironment.SimplSharpProMono)
					return IcdDirectory.GetApplicationRootDirectory();

				return IcdDirectory.GetDirectoryRoot(IcdPath.DirectorySeparatorChar.ToString());
			}
		}

		/// <summary>
		/// Gets the path to the program directory
		/// </summary>
		[PublicAPI]
		public static string ProgramPath { get { return IcdDirectory.GetApplicationDirectory(); } }

		/// <summary>
		/// Gets the path to the root config directory,
		/// which contains common and program-specific config directories.
		/// </summary>
		[PublicAPI]
		public static string RootConfigPath
		{
			get
			{
#if SIMPLSHARP
				return Join(RootPath, "User");
#elif LINUX
				return Join(RootPath, "opt", "ICD.Connect");
#else
				return Join(RootPath, "ProgramData", "ICD.Connect");
#endif
			}
		}

		/// <summary>
		/// Returns the absolute path to the configuration directory.
		/// </summary>
		/// <value></value>
		[PublicAPI]
		public static string ProgramConfigPath
		{
			get
			{
				string directoryName = "Config";

				switch (IcdEnvironment.RuntimeEnvironment)
				{
					case IcdEnvironment.eRuntimeEnvironment.SimplSharp:
					case IcdEnvironment.eRuntimeEnvironment.SimplSharpPro:
						directoryName = string.Format("Program{0:D2}Config", ProgramUtils.ProgramNumber);
						break;
				}

				return Join(RootConfigPath, directoryName);
			}
		}

		/// <summary>
		/// Returns the absolute path to the common configuration directory.
		/// </summary>
		[PublicAPI]
		public static string CommonConfigPath { get { return Join(RootConfigPath, "CommonConfig"); } }

		/// <summary>
		/// Returns the absolute path to the common config library directory.
		/// </summary>
		[PublicAPI]
		public static string CommonLibPath { get { return Join(CommonConfigPath, "Lib"); } }

		/// <summary>
		/// Returns the absolute path to the program config library directory.
		/// </summary>
		[PublicAPI]
		public static string ProgramLibPath { get { return Join(ProgramConfigPath, "Lib"); } }

		/// <summary>
		/// Returns the absolute path to the logs directory.
		/// </summary>
		/// <value></value>
		[PublicAPI]
		public static string ProgramLogsPath
		{
			get
			{
				string directoryName = "Logs";

				switch (IcdEnvironment.RuntimeEnvironment)
				{
					case IcdEnvironment.eRuntimeEnvironment.SimplSharp:
					case IcdEnvironment.eRuntimeEnvironment.SimplSharpPro:
						directoryName = string.Format("Program{0:D2}Logs", ProgramUtils.ProgramNumber);
						break;
				}

				return Join(RootConfigPath, directoryName);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Creates a path from the given path nodes.
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public static string Join(params string[] items)
		{
			return items.Length > 1
				       ? items.Skip(1).Aggregate(items.First(), IcdPath.Combine)
				       : items.FirstOrDefault(string.Empty);
		}

		/// <summary>
		/// Gets the full path for the given path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string GetFullPath(string path)
		{
			return Join(IcdDirectory.GetApplicationDirectory(), path);
		}

		/// <summary>
		/// Replaces the filename while leaving the directory and extension intact.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="newName"></param>
		/// <returns></returns>
		public static string ChangeFilenameWithoutExt(string path, string newName)
		{
			string dir = IcdPath.GetDirectoryName(path);
			string ext = IcdPath.GetExtension(path);

			return Join(dir, newName + ext);
		}

		/// <summary>
		/// Removes the extension from the given path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string GetPathWithoutExtension(string path)
		{
			string dir = IcdPath.GetDirectoryName(path);
			string filename = IcdPath.GetFileNameWithoutExtension(path);

			return Join(dir, filename);
		}

		/// <summary>
		/// Recurses over the file paths at the given directory.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static IEnumerable<string> RecurseFilePaths(string path)
		{
			if (!IcdDirectory.Exists(path))
				return Enumerable.Empty<string>();

			return RecursionUtils.BreadthFirstSearch(path, IcdDirectory.GetDirectories)
			                     .SelectMany(p => IcdDirectory.GetFiles(p));
		}

		/// <summary>
		/// Searches the program config path, common config path, and application path to
		/// find the first config that exists with the given local path.
		/// </summary>
		/// <param name="localPath"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string GetDefaultConfigPath(params string[] localPath)
		{
			string local = Join(localPath);

			// Program slot configuration
			string programPath = GetProgramConfigPath(localPath);
			if (PathExists(programPath))
				return programPath;

			// Common program configuration
			string commonPath = GetCommonConfigPath(local);
			if (PathExists(commonPath))
				return commonPath;

			return Join(IcdDirectory.GetApplicationDirectory(), local); // Installation defaults
		}

		/// <summary>
		/// Appends the local path to the common config path.
		/// </summary>
		/// <returns></returns>
		public static string GetCommonConfigPath(params string[] localPath)
		{
			string local = Join(localPath);
			return Join(CommonConfigPath, local);
		}

		/// <summary>
		/// Appends the local path to the program config path.
		/// </summary>
		/// <returns></returns>
		public static string GetProgramConfigPath(params string[] localPath)
		{
			string local = Join(localPath);
			return Join(ProgramConfigPath, local);
		}

		/// <summary>
		/// Returns true if the given path exists.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool PathExists(string path)
		{
			return IcdFile.Exists(path) || IcdDirectory.Exists(path);
		}

		/// <summary>
		/// Returns the path if the given path is already a directory or has a trailing slash.
		/// Otherwise returns the parent directory name.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string GetDirectoryNameFromPath(string path)
		{
			if (IcdDirectory.Exists(path))
				return path;

			if (path.EndsWith(IcdPath.DirectorySeparatorChar) || path.EndsWith(IcdPath.AltDirectorySeparatorChar))
				return path;

			return IcdPath.GetDirectoryName(path);
		}

		#endregion
	}
}
