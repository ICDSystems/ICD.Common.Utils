using System;
using ICD.Common.Properties;

namespace ICD.Common.Utils
{
	/// <summary>
	/// Provides features for managing the disposed state of an IDisposable.
	/// </summary>
	public interface IStateDisposable : IDisposable
	{
		/// <summary>
		/// Returns true if this instance has been disposed.
		/// </summary>
		[PublicAPI]
		bool IsDisposed { get; }
	}
}
