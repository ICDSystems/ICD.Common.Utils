using System;

namespace ICD.Common.Utils.Sqlite
{
    public interface IIcdDataReader : IDisposable, IIcdDataRecord
    {
		bool Read();
	}
}
