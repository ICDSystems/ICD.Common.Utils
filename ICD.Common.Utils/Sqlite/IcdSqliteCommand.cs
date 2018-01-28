using System;
#if SIMPLSHARP
using SqliteCommand = Crestron.SimplSharp.SQLite.SQLiteCommand;
#else
using Microsoft.Data.Sqlite;
#endif

namespace ICD.Common.Utils.Sqlite
{
	public sealed class IcdSqliteCommand : IDisposable
	{
		private readonly SqliteCommand m_Command;
		private readonly IcdSqliteParameterCollection m_Parameters;

		public IcdSqliteParameterCollection Parameters { get { return m_Parameters; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="connection"></param>
		public IcdSqliteCommand(string query, IcdSqliteConnection connection)
		{
			m_Command = new SqliteCommand(query, connection.WrappedConnection);
			m_Parameters = new IcdSqliteParameterCollection(m_Command.Parameters);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			m_Command.Dispose();
		}

		/// <summary>
		/// Executes the command against the database and returns a data reader.
		/// </summary>
		/// <returns>The data reader.</returns>
		public IcdSqliteDataReader ExecuteReader()
		{
			return new IcdSqliteDataReader(m_Command.ExecuteReader());
		}

		/// <summary>
		/// Executes the command against the database.
		/// </summary>
		/// <returns>The number of rows inserted, updated, or deleted. -1 for SELECT statements.</returns>
		public int ExecuteNonQuery()
		{
			return m_Command.ExecuteNonQuery();
		}

		public object ExecuteScalar()
		{
			return m_Command.ExecuteScalar();
		}
	}
}