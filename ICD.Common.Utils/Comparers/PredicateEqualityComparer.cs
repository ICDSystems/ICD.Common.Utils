using System;
using System.Collections.Generic;

namespace ICD.Common.Utils.Comparers
{
	public sealed class PredicateEqualityComparer<TParent, TValue> : IEqualityComparer<TParent>
	{
		private readonly IEqualityComparer<TValue> m_Comparer;
		private readonly Func<TParent, TValue> m_Predicate;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="predicate"></param>
		public PredicateEqualityComparer(Func<TParent, TValue> predicate)
			: this(EqualityComparer<TValue>.Default, predicate)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="comparer"></param>
		/// <param name="predicate"></param>
		public PredicateEqualityComparer(IEqualityComparer<TValue> comparer, Func<TParent, TValue> predicate)
		{
			m_Comparer = comparer;
			m_Predicate = predicate;
		}

		public bool Equals(TParent x, TParent y)
		{
			TValue a = m_Predicate(x);
			TValue b = m_Predicate(y);

			return m_Comparer.Equals(a, b);
		}

		public int GetHashCode(TParent parent)
		{
			TValue value = m_Predicate(parent);
			return m_Comparer.GetHashCode(value);
		}
	}
}
