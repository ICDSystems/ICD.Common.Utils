#if SIMPLSHARP
using SqliteDataReader = Crestron.SimplSharp.SQLite.SQLiteDataReader;
#else
using System;
using Microsoft.Data.Sqlite;
#endif

namespace ICD.Common.Utils.Sqlite
{
	public sealed class IcdSqliteDataReader : IcdDbDataReader
	{
		private readonly SqliteDataReader m_Reader;

		public override object this[string columnId] { get { return m_Reader[columnId]; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="reader"></param>
		public IcdSqliteDataReader(SqliteDataReader reader)
		{
			m_Reader = reader;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_Reader.Dispose();
		}

		public override bool Read()
		{
			return m_Reader.Read();
		}
	}
}
