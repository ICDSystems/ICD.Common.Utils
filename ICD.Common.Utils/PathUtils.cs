using System;
using System.Collections.Generic;
using System.Linq;
#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif
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
				if (IcdEnvironment.CrestronRuntimeEnvironment == IcdEnvironment.eCrestronRuntimeEnvironment.Server)
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
		/// Gets the path to the program entry point assembly (i.e. ICD.Connect.Core_SimplSharp.dll)
		/// </summary>
		[PublicAPI]
		[CanBeNull]
		public static string ProgramFilePath
		{
			get
			{
				Assembly entry = AssemblyUtils.GetEntryAssembly();
				return entry == null ? null : entry.GetPath();
			}
		}

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
				switch (IcdEnvironment.CrestronSeries)
				{
					case IcdEnvironment.eCrestronSeries.FourSeries:
						return Join(RootPath, "user");
					default:
						return Join(RootPath, "User");
				}
#elif LINUX
				return Join(RootPath, "opt", "ICD.Connect");
#else
				return Join(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ICD.Connect");
#endif
			}
		}

		/// <summary>
		/// Returns the absolute path to the configuration directory.
		/// </summary>
		/// <value></value>
		[PublicAPI]
		public static string ProgramConfigPath { get { return Join(RootConfigPath, ProgramConfigDirectory); } }

		/// <summary>
		/// Returns the name of the program config directory.
		/// </summary>
		[PublicAPI]
		public static string ProgramConfigDirectory
		{
			get
			{
				// Crestron Server doesn't have meaningful program number
				if (IcdEnvironment.CrestronRuntimeEnvironment == IcdEnvironment.eCrestronRuntimeEnvironment.Server)
					return "ProgramConfig";
				
				return string.Format("Program{0:D2}Config", ProgramUtils.ProgramNumber);
			}
		}

		/// <summary>
		/// Returns the absolute path to the program data directory.
		/// </summary>
		/// <value></value>
		[PublicAPI]
		public static string ProgramDataPath { get { return Join(RootConfigPath, ProgramDataDirectory); } }

		/// <summary>
		/// Returns the name of the program data directory.
		/// This directory contains runtime program data that should be retained through deployments.
		/// </summary>
		public static string ProgramDataDirectory
		{
			get
			{
				// Crestron Server doesn't have meaningful program number
				if (IcdEnvironment.CrestronRuntimeEnvironment == IcdEnvironment.eCrestronRuntimeEnvironment.Server)
					return "ProgramData";
				
				return string.Format("Program{0:D2}Data", ProgramUtils.ProgramNumber);
			}
		}

		/// <summary>
		/// Returns the absolute path to the common configuration directory.
		/// </summary>
		[PublicAPI]
		public static string CommonConfigPath { get { return Join(RootConfigPath, CommonConfigDirectory); } }

		[PublicAPI]
		public static string CommonConfigDirectory { get { return "CommonConfig"; }}

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
				string directoryName;

				// Crestron Server doesn't have meaningful program number
				if (IcdEnvironment.CrestronRuntimeEnvironment == IcdEnvironment.eCrestronRuntimeEnvironment.Server)
					directoryName = "ProgramLogs";
				else
					directoryName = string.Format("Program{0:D2}Logs", ProgramUtils.ProgramNumber);

				return Join(RootConfigPath, directoryName);
			}
		}

		/// <summary>
		/// Returns the absolute path to the control system web server directory.
		/// </summary>
		/// <value></value>
		[PublicAPI]
		public static string WebServerPath
		{
			get
			{
				if (IcdEnvironment.Framework == IcdEnvironment.eFramework.Crestron)
				{
					// 3-series
					if (IcdEnvironment.CrestronSeries == IcdEnvironment.eCrestronSeries.ThreeSeries)
						return Join(RootPath, "HTML");
					
					// 4-series non-server (because Crestron)
					if (IcdEnvironment.CrestronRuntimeEnvironment == IcdEnvironment.eCrestronRuntimeEnvironment.Appliance)
						return Join(RootPath, "html");
					
					// 4-series server (because Crestron)
					return Join(RootPath, "Html");
				}

#if LINUX
						return Join(RootPath, "var", "www", "html");
#else
						return "C:\\INetPub";
#endif
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
			if (items == null)
				throw new ArgumentNullException("items");

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
			if (localPath == null)
				throw new ArgumentNullException("localPath");

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
			if (localPath == null)
				throw new ArgumentNullException("localPath");

			string local = Join(localPath);
			return Join(CommonConfigPath, local);
		}

		/// <summary>
		/// Appends the local path to the program config path.
		/// </summary>
		/// <returns></returns>
		public static string GetProgramConfigPath(params string[] localPath)
		{
			if (localPath == null)
				throw new ArgumentNullException("localPath");

			string local = Join(localPath);
			return Join(ProgramConfigPath, local);
		}

		/// <summary>
		/// Appends the local path to the program data path.
		/// </summary>
		/// <returns></returns>
		public static string GetProgramDataPath(params string[] localPath)
		{
			if (localPath == null)
				throw new ArgumentNullException("localPath");

			string local = Join(localPath);
			return Join(ProgramDataPath, local);
		}

		/// <summary>
		/// Appends the local path to the room data path.
		/// </summary>
		/// <returns></returns>
		public static string GetRoomDataPath(int roomId, params string[] localPath)
		{
			if (localPath == null)
				throw new ArgumentNullException("localPath");

			string local = Join(localPath);
			string roomDataDirectory = GetRoomDataDirectory(roomId);
			return Join(ProgramDataPath, roomDataDirectory, local);
		}

		/// <summary>
		/// Gets the directory name of the room data directory for the room with the given id.
		/// </summary>
		/// <param name="roomId"></param>
		/// <returns></returns>
		public static string GetRoomDataDirectory(int roomId)
		{
			return string.Format("Room{0}Data", roomId);
		}

		/// <summary>
		/// Appends the local path to the user data path.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="localPath"></param>
		/// <returns></returns>
		public static string GetUserDataPath(string userName, params string[] localPath)
		{
			if (localPath == null)
				throw new ArgumentNullException("localPath");

			string local = Join(localPath);
			string userDataDirectory = GetUserDataDirectory(userName);
			return Join(ProgramDataPath, userDataDirectory, local);
		}

		/// <summary>
		/// Gets the directory name of the user data directory for the user with the given name.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public static string GetUserDataDirectory(string userName)
		{
			return string.Format("User{0}Data", StringUtils.RemoveWhitespace(userName));
		}

		/// <summary>
		/// Appends the local path to the web server path.
		/// </summary>
		/// <param name="localPath"></param>
		/// <returns></returns>
		public static string GetWebServerPath(params string[] localPath)
		{
			if (localPath == null)
				throw new ArgumentNullException("localPath");

			string local = Join(localPath);
			return Join(WebServerPath, local);
		}

		/// <summary>
		/// Gets the URL to the resource at the given web server path.
		/// </summary>
		/// <param name="webServerPath"></param>
		/// <returns></returns>
		public static string GetUrl(string webServerPath)
		{
			if (webServerPath == null)
				throw new ArgumentNullException("webServerPath");

			if (!webServerPath.StartsWith(WebServerPath))
				throw new ArgumentException("Path is not in the web server directory");

			string local = webServerPath.Substring(WebServerPath.Length + 1)
			                            .Replace('\\', '/');

			return string.Format("{0}/{1}", IcdEnvironment.NetworkAddresses.First(), local);
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
