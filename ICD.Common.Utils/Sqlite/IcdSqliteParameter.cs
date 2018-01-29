#if SIMPLSHARP
using SqliteParameter = Crestron.SimplSharp.SQLite.SQLiteParameter;
#else
using Microsoft.Data.Sqlite;
#endif

namespace ICD.Common.Utils.Sqlite
{
	public sealed class IcdSqliteParameter
	{
		private readonly SqliteParameter m_Parameter;

		public object Value { get { return m_Parameter.Value; } set { m_Parameter.Value = value; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parameter"></param>
		public IcdSqliteParameter(SqliteParameter parameter)
		{
			m_Parameter = parameter;
		}
	}
}
