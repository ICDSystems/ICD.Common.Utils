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
	}
}
