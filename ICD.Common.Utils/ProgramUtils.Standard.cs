#if NETSTANDARD
using ICD.Common.Utils.IO;
using System;
using System.Reflection;
using System.Security.Principal;
using ICD.Common.Properties;

namespace ICD.Common.Utils
{
    public static partial class ProgramUtils
    {
	    /// <summary>
	    /// Gets the program number.
	    /// </summary>
	    [PublicAPI]
	    public static uint ProgramNumber { get; set; } = 1;

		/// <summary>
		/// Gets the compile date of the program.
		/// </summary>
		[PublicAPI]
		public static DateTime CompiledDate
		{
			get
			{
				return IcdFile.GetLastWriteTime(Assembly.GetEntryAssembly().Location);
			}
		}

		/// <summary>
		/// Gets the compiler revision version.
		/// </summary>
		[PublicAPI]
		public static Version CompilerRevision
		{
			get
			{
				return Assembly.GetEntryAssembly().GetName().Version;
			}
		}

		/// <summary>
		/// Gets the name of the program dll.
		/// </summary>
		[PublicAPI]
		public static string ProgramFile { get { return IcdPath.GetFileName(Assembly.GetEntryAssembly().Location); } }

		/// <summary>
		/// Gets the name of the application.
		/// </summary>
		[PublicAPI]
		public static string ApplicationName
		{
			get
			{
				return Assembly.GetEntryAssembly().GetName().Name;
			}
		}

		/// <summary>
		/// Gets the date and time the program was installed.
		/// </summary>
		[PublicAPI]
		public static DateTime ProgramInstallDate
		{
			get { return IcdFile.GetCreationTime(PathUtils.Join(PathUtils.ProgramPath, ProgramFile)); }
		}

		/// <summary>
		/// Returns true if the current executing user is an admin.
		/// </summary>
		[PublicAPI]
		public static bool IsElevated
		{
			get
			{
				WindowsIdentity current = WindowsIdentity.GetCurrent();
				return new WindowsPrincipal(current).IsInRole(WindowsBuiltInRole.Administrator);
			}
		}
	}
}
#endif
