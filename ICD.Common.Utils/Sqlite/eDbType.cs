#if SIMPLSHARP
using Crestron.SimplSharp.CrestronData;
#else
using System;
using Microsoft.Data.Sqlite;
#endif

namespace ICD.Common.Utils.Sqlite
{
	public enum eDbType
	{
		AnsiString = 0,
		Binary = 1,
		Byte = 2,
		Boolean = 3,
		Currency = 4,
		Date = 5,
		DateTime = 6,
		Decimal = 7,
		Double = 8,
		Guid = 9,
		Int16 = 10,
		Int32 = 11,
		Int64 = 12,
		Object = 13,
		SByte = 14,
		Single = 15,
		String = 16,
		Time = 17,
		UInt16 = 18,
		UInt32 = 19,
		UInt64 = 20,
		VarNumeric = 21,
		AnsiStringFixedLength = 22,
		StringFixedLength = 23,
		Xml = 25,
		DateTime2 = 26,
		DateTimeOffset = 27
	}

	public static class DbTypeExtensions
	{
		public static
#if SIMPLSHARP
			DbType
#else
			SqliteType
#endif
			ToParamType(this eDbType extends)
		{
#if SIMPLSHARP
			return (DbType)extends;
#else
			switch (extends)
			{
				case eDbType.AnsiString:
					break;
				case eDbType.Binary:
					break;
				case eDbType.Byte:
					break;
				case eDbType.Boolean:
					break;
				case eDbType.Currency:
					break;
				case eDbType.Date:
					break;
				case eDbType.DateTime:
					break;
				case eDbType.Decimal:
				case eDbType.Double:
					return SqliteType.Real;
				case eDbType.Guid:
					break;
				case eDbType.Int16:
				case eDbType.Int32:
				case eDbType.Int64:
				case eDbType.UInt16:
				case eDbType.UInt32:
				case eDbType.UInt64:
				case eDbType.SByte:
				case eDbType.Single:
					return SqliteType.Integer;
				
				case eDbType.Object:
					break;
				
					break;
				case eDbType.String:
					break;
				case eDbType.Time:
					break;

					break;
				case eDbType.VarNumeric:
					break;
				case eDbType.AnsiStringFixedLength:
				case eDbType.StringFixedLength:
					break;
				case eDbType.Xml:
					break;
				case eDbType.DateTime2:
					break;
				case eDbType.DateTimeOffset:
					break;
				default:
					throw new ArgumentOutOfRangeException("extends");
			}
#endif
		}
	}
}
