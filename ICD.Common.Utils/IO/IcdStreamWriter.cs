#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
#else
using System.IO;
#endif

namespace ICD.Common.Utils.IO
{
	public sealed class IcdStreamWriter : IcdTextWriter
	{
		public StreamWriter WrappedStreamWriter { get { return WrappedTextWriter as StreamWriter; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="baseStreamWriter"></param>
		public IcdStreamWriter(StreamWriter baseStreamWriter) : base(baseStreamWriter)
		{
		}

		public void WriteLine(string value)
		{
			WrappedStreamWriter.WriteLine(value);
		}
	}
}