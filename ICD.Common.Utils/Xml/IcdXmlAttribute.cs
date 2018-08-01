using System;

namespace ICD.Common.Utils.Xml
{
	/// <summary>
	/// IcdXmlAttribute represents an attribute="value" pair from xml.
	/// </summary>
	public struct IcdXmlAttribute : IEquatable<IcdXmlAttribute>
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
			return !a1.Equals(a2);
		}

		public bool Equals(IcdXmlAttribute other)
		{
			return m_Name == other.m_Name &&
			       m_Value == other.m_Value;
		}

		/// <summary>
		/// Returns true if this instance is equal to the given object.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public override bool Equals(object other)
		{
			return other is IcdXmlAttribute && Equals((IcdXmlAttribute)other);
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
