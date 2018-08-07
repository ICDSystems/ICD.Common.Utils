using System;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
#else
using System.IO;
#endif

namespace ICD.Common.Utils.IO
{
	public class IcdTextReader : IDisposable
	{
		private readonly TextReader m_TextReader;

		public TextReader WrappedTextReader { get { return m_TextReader; } }

		protected IcdTextReader(TextReader textReader)
		{
			m_TextReader = textReader;
		}

		public void Dispose()
		{
			m_TextReader.Dispose();
		}
	}
}
