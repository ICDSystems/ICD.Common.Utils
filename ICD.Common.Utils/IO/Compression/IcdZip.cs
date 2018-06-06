using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if SIMPLSHARP
using Crestron.SimplSharp;
#else
using System.IO.Compression;
#endif

namespace ICD.Common.Utils.IO.Compression
{
	/// <summary>
	/// Utils for managing archives.
	/// </summary>
	public static class IcdZip
	{
		private const int DIRECTORY_SIGNATURE = 0x06054B50;
		private const int ENTRY_SIGNATURE = 0x02014B50;

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

		/// <summary>
		/// Gets the sequence of files names in the archive at the given path.
		/// </summary>
		public static IEnumerable<string> GetFileNames(string path)
		{
			return GetEntries(path).Select(e => e.Name)
			                       .Where(f => !f.EndsWith("/"))
			                       .OrderBy(f => f);
		}

		/// <summary>
		/// Gets sequence of zip file entries in the archive at the given path.
		/// </summary>
		public static IEnumerable<IcdZipEntry> GetEntries(string path)
		{
			/*
				Copyright (c) 2012-2013 Alexey Yakovlev

				Permission is hereby granted, free of charge, to any person obtaining a copy
				of this software and associated documentation files (the "Software"), to deal
				in the Software without restriction, including without limitation the rights
				to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
				copies of the Software, and to permit persons to whom the Software is
				furnished to do so, subject to the following conditions:

				The above copyright notice and this permission notice shall be included in
				all copies or substantial portions of the Software.

				THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
				IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
				FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
				AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
				LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
				OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
				THE SOFTWARE.
			*/

			using (IcdStream stream = IcdFile.OpenRead(path))
			{
				using (IcdBinaryReader reader = new IcdBinaryReader(stream))
				{
					if (stream.Length < 22)
						yield break;

					stream.Seek(-22, eSeekOrigin.End.ToSeekOrigin());

					// find directory signature
					while (reader.ReadInt32() != DIRECTORY_SIGNATURE)
					{
						if (stream.Position <= 5)
							yield break;

						// move 1 byte back
						stream.Seek(-5, eSeekOrigin.Current.ToSeekOrigin());
					}

					// read directory properties
					stream.Seek(6, eSeekOrigin.Current.ToSeekOrigin());
					ushort entries = reader.ReadUInt16();
					int difSize = reader.ReadInt32();
					uint dirOffset = reader.ReadUInt32();
					stream.Seek(dirOffset, eSeekOrigin.Begin.ToSeekOrigin());

					// read directory entries
					for (int i = 0; i < entries; i++)
					{
						if (reader.ReadInt32() != ENTRY_SIGNATURE)
							continue;

						// read file properties
						reader.ReadInt32();
						bool utf8 = (reader.ReadInt16() & 0x0800) != 0;
						short method = reader.ReadInt16();
						int timestamp = reader.ReadInt32();
						uint crc32 = reader.ReadUInt32();
						int compressedSize = reader.ReadInt32();
						int fileSize = reader.ReadInt32();
						short fileNameSize = reader.ReadInt16();
						short extraSize = reader.ReadInt16();
						short commentSize = reader.ReadInt16();
						int headerOffset = reader.ReadInt32();
						reader.ReadInt32();
						int fileHeaderOffset = reader.ReadInt32();
						byte[] fileNameBytes = reader.ReadBytes(fileNameSize);
						stream.Seek(extraSize, eSeekOrigin.Current.ToSeekOrigin());
						byte[] fileCommentBytes = reader.ReadBytes(commentSize);
						int fileDataOffset = CalculateFileDataOffset(stream, reader, fileHeaderOffset);

						// decode zip file entry
						Encoding encoder = utf8 ? Encoding.UTF8 : Encoding.Default;

						yield return new IcdZipEntry
						{
							Name = encoder.GetString(fileNameBytes, 0, fileNameBytes.Length),
							Comment = encoder.GetString(fileCommentBytes, 0, fileCommentBytes.Length),
							Crc32 = crc32,
							CompressedSize = compressedSize,
							OriginalSize = fileSize,
							HeaderOffset = fileHeaderOffset,
							DataOffset = fileDataOffset,
							Deflated = method == 8,
							Timestamp = ConvertToDateTime(timestamp)
						};
					}
				}
			}
		}

		private static int CalculateFileDataOffset(IcdStream stream, IcdBinaryReader reader, int fileHeaderOffset)
		{
			long position = stream.Position;
			stream.Seek(fileHeaderOffset + 26, eSeekOrigin.Begin.ToSeekOrigin());
			short fileNameSize = reader.ReadInt16();
			short extraSize = reader.ReadInt16();

			int fileOffset = (int)stream.Position + fileNameSize + extraSize;
			stream.Seek(position, eSeekOrigin.Begin.ToSeekOrigin());
			return fileOffset;
		}

		/// <summary>
		/// Converts DOS timestamp to a <see cref="DateTime"/> instance.
		/// </summary>
		/// <param name="dosTimestamp">The DOS timestamp.</param>
		/// <returns>The <see cref="DateTime"/> instance.</returns>
		private static DateTime ConvertToDateTime(int dosTimestamp)
		{
			return new DateTime((dosTimestamp >> 25) + 1980,
			                    (dosTimestamp >> 21) & 15,
			                    (dosTimestamp >> 16) & 31,
			                    (dosTimestamp >> 11) & 31,
			                    (dosTimestamp >> 5) & 63,
			                    (dosTimestamp & 31) * 2);
		}
	}
}
