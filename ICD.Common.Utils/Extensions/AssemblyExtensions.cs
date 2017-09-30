using System;
#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif
using ICD.Common.Properties;
using ICD.Common.Utils.IO;

namespace ICD.Common.Utils.Extensions
{
    public static class AssemblyExtensions
    {
		/// <summary>
		/// Gets the path for the given assembly. Returns null if the assembly can not be found on disk.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[CanBeNull]
		public static string GetPath(this Assembly extends)
		{
			if (extends == null)
				throw new ArgumentNullException();

			string path = extends
#if SIMPLSHARP
				.GetName()
#endif
				.CodeBase;

			if (path == null)
			{
				path = extends.Location;
			}
			else
			{
				const string prefix = @"file:///";
				if (path.StartsWith(prefix))
					path = path.Substring(prefix.Length);
			}

			return IcdFile.Exists(path) ? path : null;
		}
    }
}
