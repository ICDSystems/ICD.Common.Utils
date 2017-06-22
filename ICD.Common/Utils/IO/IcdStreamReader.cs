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
	}
}
