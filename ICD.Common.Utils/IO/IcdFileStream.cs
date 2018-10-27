#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
#else
using System.IO;
#endif
using System.Text;

namespace ICD.Common.Utils.IO
{
	public sealed class IcdFileStream : IcdStream
	{
		public FileStream WrappedFileStream { get { return WrappedStream as FileStream; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fileStrean"></param>
		public IcdFileStream(FileStream fileStrean)
			: base(fileStrean)
		{
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

		public void Write(string data, Encoding encoding)
		{
			byte[] info = encoding.GetBytes(data);
			WrappedFileStream.Write(info, 0, info.Length);
		}
	}
}
