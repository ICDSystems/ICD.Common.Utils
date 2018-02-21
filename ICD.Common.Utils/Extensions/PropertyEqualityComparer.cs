using System;
using System.Collections.Generic;

namespace ICD.Common.Utils.Extensions
{
	/// <summary>
	/// Allows for comparing items based on some property.
	/// </summary>
	/// <typeparam name="TParent"></typeparam>
	/// <typeparam name="TProperty"></typeparam>
	public sealed class PropertyEqualityComparer<TParent, TProperty> : IEqualityComparer<TParent>
	{
		private readonly IEqualityComparer<TProperty> m_Comparer;
		private readonly Func<TParent, TProperty> m_GetProperty;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="comparer"></param>
		/// <param name="getProperty"></param>
		public PropertyEqualityComparer(IEqualityComparer<TProperty> comparer, Func<TParent, TProperty> getProperty)
		{
			m_Comparer = comparer;
			m_GetProperty = getProperty;
		}

		public bool Equals(TParent x, TParent y)
		{
			TProperty a = m_GetProperty(x);
			TProperty b = m_GetProperty(y);

			return m_Comparer.Equals(a, b);
		}

		public int GetHashCode(TParent parent)
		{
			TProperty property = m_GetProperty(parent);
			return m_Comparer.GetHashCode(property);
		}
	}
}
