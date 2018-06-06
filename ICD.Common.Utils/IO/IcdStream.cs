using System;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
#else
using System.IO;
#endif

namespace ICD.Common.Utils.IO
{
	public class IcdStream : IDisposable
	{
		private readonly Stream m_Stream;

		public Stream WrappedStream { get { return m_Stream; } }

		public long Length { get { return m_Stream.Length; } }

		public long Position { get { return m_Stream.Position; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="stream"></param>
		public IcdStream(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");

			m_Stream = stream;
		}

		public void Dispose()
		{
			m_Stream.Dispose();
		}

		public void Seek(long offset, SeekOrigin end)
		{
			m_Stream.Seek(offset, end);
		}
	}
}
