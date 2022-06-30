#if !NETSTANDARD
using SqliteParameterCollection = Crestron.SimplSharp.SQLite.SQLiteParameterCollection;
#else
using Microsoft.Data.Sqlite;
#endif

namespace ICD.Common.Utils.Sqlite
{
	public sealed class IcdSqliteParameterCollection
	{
		private readonly SqliteParameterCollection m_Parameters;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="commandParameters"></param>
		public IcdSqliteParameterCollection(SqliteParameterCollection commandParameters)
		{
			m_Parameters = commandParameters;
		}

		public IcdSqliteParameter Add(string name, eDbType type)
		{
			return new IcdSqliteParameter(m_Parameters.Add(name, type.ToParamType()));
		}

		public void AddWithValue(string parameterName, object value)
		{
			m_Parameters.AddWithValue(parameterName, value);
		}
	}
}
