using System;
using System.Collections.Generic;
using ICD.Common.Utils.Attributes;

namespace ICD.Common.Utils.Xml
{
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class XmlConverterAttribute : AbstractIcdAttribute
    {
		private static readonly Dictionary<Type, IXmlConverter> s_InstanceTypeToConverter;
		private static readonly Dictionary<Type, IXmlConverter> s_ConverterTypeToConverter;

		private readonly Type m_ConverterType;

		/// <summary>
		/// Gets the converter type.
		/// </summary>
		public Type ConverterType { get { return m_ConverterType; } }

		/// <summary>
		/// Static constructor.
		/// </summary>
		static XmlConverterAttribute()
		{
			s_InstanceTypeToConverter = new Dictionary<Type, IXmlConverter>();
			s_ConverterTypeToConverter = new Dictionary<Type, IXmlConverter>();
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="converterType"></param>
		public XmlConverterAttribute(Type converterType)
		{
			m_ConverterType = converterType;
		}

		/// <summary>
		/// Gets the XML converter for the given instance.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static IXmlConverter GetConverterForInstance(object value)
		{
			return value == null ? DefaultXmlConverter.Instance(typeof(object)) : GetConverterForType(value.GetType());
		}

		/// <summary>
		/// Gets the XML converter for the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IXmlConverter GetConverterForType(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			IXmlConverter converter;
			if (!s_InstanceTypeToConverter.TryGetValue(type, out converter))
			{
				XmlConverterAttribute attribute = AttributeUtils.GetClassAttribute<XmlConverterAttribute>(type);
				Type converterType = attribute == null ? null : attribute.ConverterType;

				converter = converterType == null ? DefaultXmlConverter.Instance(type) : LazyLoadConverter(converterType);
				s_InstanceTypeToConverter[type] = converter;
			}

			return converter;
		}

		/// <summary>
		/// Lazy-loads the converter of the given type.
		/// </summary>
		/// <param name="converterType"></param>
		/// <returns></returns>
		private static IXmlConverter LazyLoadConverter(Type converterType)
		{
			if (converterType == null)
				throw new ArgumentNullException("converterType");

			IXmlConverter converter;
			if (!s_ConverterTypeToConverter.TryGetValue(converterType, out converter))
			{
				converter = ReflectionUtils.CreateInstance(converterType) as IXmlConverter;
				s_ConverterTypeToConverter[converterType] = converter;
			}

			return converter;
		}
	}
}
