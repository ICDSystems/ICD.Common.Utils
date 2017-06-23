using System;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
#else
using System.IO;
#endif

namespace ICD.Common.Utils.IO
{
	public class IcdTextWriter : IDisposable
	{
		private readonly TextWriter m_TextWriter;

		public TextWriter WrappedTextWriter { get { return m_TextWriter; } }

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
			Dispose();
		}

		public void Dispose()
		{
			m_TextWriter.Dispose();
		}
	}
}
