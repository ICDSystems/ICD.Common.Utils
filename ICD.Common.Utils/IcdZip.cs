using System;
#if SIMPLSHARP
using Crestron.SimplSharp;
#else
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
		/// <param name="message"></param>
		public static bool Unzip(string path, string outputPath, out string message)
		{
			try
			{
#if SIMPLSHARP
				CrestronZIP.ResultCode result = CrestronZIP.Unzip(path, outputPath);
				message = result.ToString();
				return result == CrestronZIP.ResultCode.ZR_OK;
#else
                using (ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Read))
                    archive.ExtractToDirectory(outputPath);
				message = "Success";
				return true;
#endif
			}
			catch (Exception e)
			{
				message = e.Message;
				return false;
			}
		}
	}
}
