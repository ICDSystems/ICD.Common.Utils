using System;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
#else
using System.IO;
#endif

namespace ICD.Common.Utils.IO
{
	public sealed class IcdFileStream : IcdStream
	{
		public int Position { get; set; }

		public FileStream WrappedFileStream { get { return WrappedStream as FileStream; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fileStrean"></param>
		public IcdFileStream(FileStream fileStrean)
			: base(fileStrean)
		{
		}

		public static IcdFileStream OpenWrite(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			return new IcdFileStream(File.OpenWrite(path));
		}

		public void Flush()
		{
			WrappedFileStream.Flush();
		}

		public void Close()
		{
#if SIMPLSHARP
			WrappedFileStream.Close();
#else
			WrappedFileStream.Dispose();
#endif
		}
	}
}
