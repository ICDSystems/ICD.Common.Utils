#if !SIMPLSHARP
using ICD.Common.Utils.IO;
using System;
using System.Reflection;
using ICD.Common.Properties;

namespace ICD.Common.Utils
{
    public static partial class ProgramUtils
    {
		/// <summary>
		/// Gets the program number.
		/// </summary>
		[PublicAPI]
		public static uint ProgramNumber
		{
			get
			{
				return 1;
			}
		}

		/// <summary>
		/// Gets the compile date of the program.
		/// </summary>
		[PublicAPI]
		public static string CompiledDate
		{
			get
			{
				return IcdFile.GetLastWriteTime(Assembly.GetEntryAssembly().Location).ToString();
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
	}
}
#endif
