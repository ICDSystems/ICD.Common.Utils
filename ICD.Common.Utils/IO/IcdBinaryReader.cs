using System;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
#else
using System.IO;
#endif

namespace ICD.Common.Utils.IO
{
	public sealed class IcdBinaryReader : IDisposable
	{
		private readonly BinaryReader m_Reader;

		public BinaryReader WrappedReader { get { return m_Reader; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="stream"></param>
		public IcdBinaryReader(IcdStream stream)
			: this(new BinaryReader(stream.WrappedStream))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="reader"></param>
		public IcdBinaryReader(BinaryReader reader)
		{
			m_Reader = reader;
		}

		public void Dispose()
		{
			m_Reader.Dispose();
		}

		public void Close()
		{
			m_Reader.Close();
		}

		public ushort ReadUInt16()
		{
			return m_Reader.ReadUInt16();
		}

		public int ReadInt32()
		{
			return m_Reader.ReadInt32();
		}

		public short ReadInt16()
		{
			return m_Reader.ReadInt16();
		}

		public uint ReadUInt32()
		{
			return m_Reader.ReadUInt32();
		}

		public byte[] ReadBytes(short numberOfBytesToRead)
		{
			return m_Reader.ReadBytes(numberOfBytesToRead);
		}
	}
}