using System;

namespace ICD.Common.Utils.IO.Compression
{
	/// <summary>
	/// Zip archive entry.
	/// </summary>
	public sealed class IcdZipEntry
	{
		/// <summary>
		/// Gets or sets the name of a file or a directory.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the comment.
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		/// Gets or sets the CRC32.
		/// </summary>
		public uint Crc32 { get; set; }

		/// <summary>
		/// Gets or sets the compressed size of the file.
		/// </summary>
		public int CompressedSize { get; set; }

		/// <summary>
		/// Gets or sets the original size of the file.
		/// </summary>
		public int OriginalSize { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="IcdZipEntry" /> is deflated.
		/// </summary>
		public bool Deflated { get; set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="IcdZipEntry" /> is a directory.
		/// </summary>
		public bool IsDirectory { get { return Name.EndsWith("/"); } }

		/// <summary>
		/// Gets or sets the timestamp.
		/// </summary>
		public DateTime Timestamp { get; set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="IcdZipEntry" /> is a file.
		/// </summary>
		public bool IsFile { get { return !IsDirectory; } }

		public int HeaderOffset { get; set; }

		public int DataOffset { get; set; }
	}
}
