using System;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
using GC = Crestron.SimplSharp.CrestronEnvironment.GC;

#else
using System.IO;
using GC = System.GC;
#endif

namespace ICD.Common.Utils.IO
{
	public class IcdTextWriter : IDisposable
	{
		private readonly TextWriter m_TextWriter;

		public TextWriter WrappedTextWriter { get { return m_TextWriter; } }

		private bool m_Disposed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="textWriter"></param>
		protected IcdTextWriter(TextWriter textWriter)
		{
			if (textWriter == null)
				throw new ArgumentNullException("textWriter");

			m_TextWriter = textWriter;
		}

		~IcdTextWriter()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (m_Disposed)
				return;

			if (disposing)
				m_TextWriter.Dispose();
			m_Disposed = true;
		}
	}
}
