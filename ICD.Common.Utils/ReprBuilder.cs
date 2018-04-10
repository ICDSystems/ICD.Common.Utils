using System.Collections.Generic;
using System.Text;

namespace ICD.Common.Utils
{
	public delegate void AddReprPropertyDelegate(string name, object value);

	/// <summary>
	/// Simple class for building a string representation of an object.
	/// </summary>
	public sealed class ReprBuilder
	{
		private readonly object m_Instance;

		private readonly List<string> m_PropertyOrder;
		private readonly Dictionary<string, object> m_PropertyValues;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="instance"></param>
		public ReprBuilder(object instance)
		{
			m_Instance = instance;

			m_PropertyOrder = new List<string>();
			m_PropertyValues = new Dictionary<string, object>();
		}

		/// <summary>
		/// Adds the property with the given name and value to the 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void AppendProperty(string name, object value)
		{
			m_PropertyOrder.Remove(name);
			m_PropertyOrder.Add(name);

			m_PropertyValues[name] = value;
		}

		/// <summary>
		/// Returns the result of the builder.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (m_Instance == null)
				return GetValueStringRepresentation(m_Instance);

			StringBuilder builder = new StringBuilder();

			builder.Append(m_Instance.GetType().Name);
			builder.Append('(');

			for (int index = 0; index < m_PropertyOrder.Count; index++)
			{
				string property = m_PropertyOrder[index];
				builder.Append(property);
				builder.Append('=');

				object value = m_PropertyValues[property];
				string valueString = GetValueStringRepresentation(value);
				builder.Append(valueString);

				if (index < m_PropertyOrder.Count - 1)
					builder.Append(", ");
			}

			builder.Append(')');

			return builder.ToString();
		}

		private static string GetValueStringRepresentation(object value)
		{
			if (value == null || value is string)
				return StringUtils.ToRepresentation(value as string);

			return value.ToString();
		}
	}
}
