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
		private readonly Func<T, int> m_GetHashCode;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="comparer"></param>
		/// <param name="getHashCode"></param>
		public FuncComparer(Func<T, T, bool> comparer, Func<T, int> getHashCode)
		{
			m_Comparer = comparer;
			m_GetHashCode = getHashCode;
		}

		public bool Equals(T x, T y)
		{
			return m_Comparer(x, y);
		}

		public int GetHashCode(T obj)
		{
			return m_GetHashCode(obj);
		}
	}
}
