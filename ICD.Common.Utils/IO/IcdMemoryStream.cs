#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
#else
using System.IO;
#endif

namespace ICD.Common.Utils.IO
{
	public sealed class IcdMemoryStream : IcdStream
	{
		public MemoryStream WrappedMemoryStream { get { return WrappedStream as MemoryStream; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		public IcdMemoryStream()
			: this(new MemoryStream())
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="memoryStream"></param>
		public IcdMemoryStream(MemoryStream memoryStream)
			: base(memoryStream)
		{
		}

		public void Flush()
		{
			WrappedMemoryStream.Flush();
		}

		public void Close()
		{
#if SIMPLSHARP
			WrappedMemoryStream.Close();
#else
            WrappedMemoryStream.Dispose();
#endif
		}
	}
}
