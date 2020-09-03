using System.Collections.Generic;
using System.Linq;

namespace ICD.Common.Utils.IO.Compression
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
		public static void Unzip(string path, string outputPath)
		{
			using (Unzip unzip = new Unzip(path))
				unzip.ExtractToDirectory(outputPath);
		}

		/// <summary>
		/// Gets the sequence of files names in the archive at the given path.
		/// </summary>
		public static IEnumerable<string> GetFileNames(string path)
		{
			using (Unzip unzip = new Unzip(path))
				return unzip.FileNames.ToArray();
		}
	}
}
