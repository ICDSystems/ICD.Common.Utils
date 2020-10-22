using System;

namespace ICD.Common.Utils.Collections
{
	// Poor-mans System.Collections.Specialized
	// Delete when we finally drop Crestron 3-series.
	public interface INotifyCollectionChanged
	{
		/// <summary>
		/// Raised when the contents of the collection change, or the order changes.
		/// </summary>
		event EventHandler OnCollectionChanged;
	}
}
