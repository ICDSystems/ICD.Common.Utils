using System;
using ICD.Common.Properties;

namespace ICD.Common.Attributes.Properties
{
	/// <summary>
	/// Provides information on a settings property.
	/// </summary>
	[PublicAPI]
	public sealed class SettingsProperty : Attribute
	{
		public enum ePropertyType
		{
			[PublicAPI] Default,
			[PublicAPI] PortId,
			[PublicAPI] DeviceId,
			[PublicAPI] Ipid,
			[PublicAPI] Enum
		}

		private readonly ePropertyType m_PropertyType;

		/// <summary>
		/// Gets the property type.
		/// </summary>
		[PublicAPI]
		public ePropertyType PropertyType { get { return m_PropertyType; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="propertyType"></param>
		public SettingsProperty(ePropertyType propertyType)
		{
			m_PropertyType = propertyType;
		}
	}
}
