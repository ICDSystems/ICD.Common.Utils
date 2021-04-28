using System;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
#elif STANDARD
using System.IO;
#endif

namespace ICD.Common.Utils.IO
{
	public sealed class IcdStreamReader : IDisposable
	{
		private readonly StreamReader m_StreamReader;

		public StreamReader WrappedStreamReader { get { return m_StreamReader; } }

		public bool EndOfStream { get { return m_StreamReader.EndOfStream; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="memoryStream"></param>
		public IcdStreamReader(IcdMemoryStream memoryStream)
		{
			if (memoryStream == null)
				throw new ArgumentNullException("memoryStream");

			m_StreamReader = new StreamReader(memoryStream.WrappedMemoryStream);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="path"></param>
		public IcdStreamReader(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			if (!IcdFile.Exists(path))
				throw new FileNotFoundException("Error creating stream reader, file not found");

			m_StreamReader = new StreamReader(path);
		}

		~IcdStreamReader()
		{
			Dispose();
		}

		public string ReadToEnd()
		{
			return m_StreamReader.ReadToEnd();
		}

		public void Dispose()
		{
			m_StreamReader.Dispose();
		}

		public string ReadLine()
		{
			return m_StreamReader.ReadLine();
		}
	}
}
