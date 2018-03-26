using System;
using System.Collections.Generic;

namespace ICD.Common.Utils.Comparers
{
	public sealed class PredicateComparer<TParent, TValue> : IComparer<TParent>
	{
		private readonly IComparer<TValue> m_Comparer;
		private readonly Func<TParent, TValue> m_Predicate;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="predicate"></param>
		public PredicateComparer(Func<TParent, TValue> predicate)
			: this(Comparer<TValue>.Default, predicate)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="comparer"></param>
		/// <param name="predicate"></param>
		public PredicateComparer(IComparer<TValue> comparer, Func<TParent, TValue> predicate)
		{
			m_Comparer = comparer;
			m_Predicate = predicate;
		}

		public int Compare(TParent x, TParent y)
		{
			TValue a = m_Predicate(x);
			TValue b = m_Predicate(y);

			return m_Comparer.Compare(a, b);
		}
	}
}
