namespace ICD.Common.Utils.Sqlite
{
    public interface IIcdDataRecord
    {
		object this[string columnId] { get; }
	}
}
