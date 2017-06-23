﻿using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Services;
using ICD.Common.Services.Logging;
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
		public static string RootPath { get { return IcdDirectory.GetDirectoryRoot("\\"); } }

		/// <summary>
		/// Gets the path to the NVRAM directory.
		/// </summary>
		[PublicAPI]
		public static string NvramPath { get { return Join(RootPath, "NVRAM"); } }

		/// <summary>
		/// Returns the absolute path to the configuration directory.
		/// </summary>
		/// <value></value>
		[PublicAPI]
		public static string ProgramConfigPath
		{
			get
			{
				string directoryName = string.Format("Program{0:D2}Config", ProgramUtils.ProgramNumber);
				return Join(NvramPath, directoryName);
			}
		}

		/// <summary>
		/// Returns the absolute path to the common configuration directory.
		/// </summary>
		[PublicAPI]
		public static string CommonConfigPath { get { return Join(NvramPath, "CommonConfig"); } }

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

		#endregion

		#region Methods

		/// <summary>
		/// Creates a path from the given path nodes.
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public static string Join(params string[] items)
		{
			try
			{
				return items.Skip(1).Aggregate(items.First(), IcdPath.Combine);
			}
			catch (ArgumentException e)
			{
				throw new ArgumentException("Failed to join path: " + StringUtils.ArrayFormat(items), e);
			}
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
				yield break;

			Queue<string> queue = new Queue<string>();
			queue.Enqueue(path);

			while (queue.Count > 0)
			{
				path = queue.Dequeue();

				// Get the subdirectories
				try
				{
					foreach (string subDir in IcdDirectory.GetDirectories(path))
						queue.Enqueue(subDir);
				}
				catch (Exception e)
				{
					ServiceProvider.TryGetService<ILoggerService>().AddEntry(eSeverity.Error, e, e.Message);
				}

				// Get the files
				string[] files;
				try
				{
					files = IcdDirectory.GetFiles(path);
				}
				catch (Exception e)
				{
					ServiceProvider.TryGetService<ILoggerService>().AddEntry(eSeverity.Error, e, e.Message);
					continue;
				}

				foreach (string filePath in files)
					yield return filePath;
			}
		}

		/// <summary>
		/// Searches the program config path, common config path, and application path to
		/// find the first config that exists with the given local path.
		/// </summary>
		/// <param name="localPath"></param>
		/// <returns></returns>
		public static string GetDefaultConfigPath(params string[] localPath)
		{
			string local = Join(localPath);

			// Program slot configuration
			string programPath = Join(ProgramConfigPath, local);
			if (PathExists(programPath))
				return programPath;

			// Common program configuration
			string commonPath = Join(CommonConfigPath, local);
			return PathExists(commonPath)
				       ? commonPath
				       : Join(IcdDirectory.GetApplicationDirectory(), local); // Installation defaults
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
		/// Searches the application path, program config path and common config path to
		/// find the first IR driver that exists with the given local path.
		/// </summary>
		/// <param name="localPath"></param>
		/// <returns></returns>
		public static string GetIrDriversPath(params string[] localPath)
		{
			return GetDefaultConfigPath(localPath.Prepend("IRDrivers").ToArray());
		}

		/// <summary>
		/// Searches the application path, program config path and common config path to
		/// find the first SSL Driver that exists with the given local path.
		/// </summary>
		/// <param name="localPath"></param>
		/// <returns></returns>
		public static string GetSslCertificatesPath(params string[] localPath)
		{
			return GetDefaultConfigPath(localPath.Prepend("SSLCertificates").ToArray());
		}

		/// <summary>
		/// Returns true if the given path exists.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static bool PathExists(string path)
		{
			return IcdFile.Exists(path) || IcdDirectory.Exists(path);
		}

		#endregion
	}
}
