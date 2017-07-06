namespace ICD.Common.Utils.Xml
{
	/// <summary>
	/// IcdXmlAttribute represents an attribute="value" pair from xml.
	/// </summary>
	public struct IcdXmlAttribute
	{
		private readonly string m_Name;
		private readonly string m_Value;

		public string Name { get { return m_Name; } }
		public string Value { get { return m_Value; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public IcdXmlAttribute(string name, string value)
		{
			m_Name = name;
			m_Value = value;
		}

		/// <summary>
		/// Implementing default equality.
		/// </summary>
		/// <param name="a1"></param>
		/// <param name="a2"></param>
		/// <returns></returns>
		public static bool operator ==(IcdXmlAttribute a1, IcdXmlAttribute a2)
		{
			return a1.Equals(a2);
		}

		/// <summary>
		/// Implementing default inequality.
		/// </summary>
		/// <param name="a1"></param>
		/// <param name="a2"></param>
		/// <returns></returns>
		public static bool operator !=(IcdXmlAttribute a1, IcdXmlAttribute a2)
		{
			return !(a1 == a2);
		}

		/// <summary>
		/// Returns true if this instance is equal to the given object.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public override bool Equals(object other)
		{
			if (other == null || GetType() != other.GetType())
				return false;

			return GetHashCode() == ((IcdXmlAttribute)other).GetHashCode();
		}

		/// <summary>
		/// Gets the hashcode for this instance.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 23 + (m_Name == null ? 0 : m_Name.GetHashCode());
				hash = hash * 23 + (m_Value == null ? 0 : m_Value.GetHashCode());
				return hash;
			}
		}
	}
}
