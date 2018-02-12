using System;
using System.Collections.Generic;

namespace ICD.Common.Utils
{
	/// <summary>
	/// Simple comparer for comparing items using a callback.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class FuncComparer<T> : IEqualityComparer<T>
	{
		private readonly Func<T, T, bool> m_Comparer;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="comparer"></param>
		public FuncComparer(Func<T, T, bool> comparer)
		{
			m_Comparer = comparer;
		}

		public bool Equals(T x, T y)
		{
			return m_Comparer(x, y);
		}

		public int GetHashCode(T obj)
		{
			return obj.GetHashCode();
		}
	}
}
