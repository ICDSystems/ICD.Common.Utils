#if SIMPLSHARP
using Crestron.SimplSharp;
#else
using System;
using System.IO.Compression;
#endif

namespace ICD.Common.Utils
{
	/// <summary>
	/// Utils for managing archives.
	/// </summary>
	public static class IcdZip
	{
		/// <summary>
		/// Unzips the archive at the given path.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="outputPath"></param>
		public static bool Unzip(string path, string outputPath)
		{
#if SIMPLSHARP
			return CrestronZIP.Unzip(path, outputPath) == CrestronZIP.ResultCode.ZR_OK;
#else
            try
            {
                using (ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Read))
                    archive.ExtractToDirectory(outputPath);
            }
            catch (Exception)
            {
                return false;
            }
            
            return true;
#endif
		}
	}
}
