using System;
using ICD.Common.Utils.IO;
#if SIMPLSHARP
using SqliteConnection = Crestron.SimplSharp.SQLite.SQLiteConnection;
#else
using Microsoft.Data.Sqlite;
#endif

namespace ICD.Common.Utils.Sqlite
{
	public sealed class IcdSqliteConnection : IDisposable
	{
		private readonly SqliteConnection m_Connection;

		/// <summary>
		/// Gets the wrapped connection instance.
		/// </summary>
		public SqliteConnection WrappedConnection { get { return m_Connection; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="connectionString"></param>
		public IcdSqliteConnection(string connectionString)
		{
			m_Connection = new SqliteConnection(connectionString);
		}

		/// <summary>
		/// Creates a new SQLite database file at the given path.
		/// </summary>
		/// <param name="path"></param>
		public static void CreateFile(string path)
		{
			IcdFileStream fs = IcdFile.Create(path);
			fs.Close();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			m_Connection.Dispose();
		}

		/// <summary>
		/// Opens a connection to the database.
		/// </summary>
		public void Open()
		{
			m_Connection.Open();
		}
	}
}
