namespace ICD.Common.Utils.Sqlite
{
    public abstract class IcdDbDataReader : IIcdDataReader
    {
		/// <summary>
		/// Release resources.
		/// </summary>
		public abstract void Dispose();

		public abstract bool Read();

		public abstract object this[string columnId] { get; }
	}
}
