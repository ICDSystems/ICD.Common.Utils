using System;
using System.Collections.Generic;

namespace ICD.Common.Utils.Comparers
{
	public sealed class PropertyComparer<TParent, TProperty> : IComparer<TParent>
	{
		private readonly IComparer<TProperty> m_Comparer;
		private readonly Func<TParent, TProperty> m_GetProperty;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="comparer"></param>
		/// <param name="getProperty"></param>
		public PropertyComparer(IComparer<TProperty> comparer, Func<TParent, TProperty> getProperty)
		{
			m_Comparer = comparer;
			m_GetProperty = getProperty;
		}

		public int Compare(TParent x, TParent y)
		{
			TProperty a = m_GetProperty(x);
			TProperty b = m_GetProperty(y);

			return m_Comparer.Compare(a, b);
		}
	}
}
