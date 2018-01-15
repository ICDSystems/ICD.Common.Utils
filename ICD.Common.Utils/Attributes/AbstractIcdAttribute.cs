using System;

namespace ICD.Common.Utils.Attributes
{
	/// <summary>
	/// AbstractIcdAttribute is the base class for all ICD attributes.
	/// It provides a global cache for looking up symbols via an attribute type.
	/// </summary>
	public abstract class AbstractIcdAttribute : Attribute
	{
		private readonly int m_HashCode;

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractIcdAttribute()
		{
			// Duplicate attributes (E.g. [A, A]) are considered to be the same instance by reflection.
			// We get around this by using a GUID for the hash code.
			m_HashCode = Guid.NewGuid().GetHashCode();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets the hash code for the instance.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return m_HashCode;
		}

		#endregion
	}
}
