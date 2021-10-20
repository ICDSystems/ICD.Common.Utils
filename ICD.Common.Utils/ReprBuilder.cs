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

		private readonly List<KeyValuePair<string, string>> m_PropertyOrder;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="instance"></param>
		public ReprBuilder(object instance)
		{
			m_Instance = instance;

			m_PropertyOrder = new List<KeyValuePair<string, string>>();
		}

		/// <summary>
		/// Adds the property with the given name and value to the builder.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public ReprBuilder AppendProperty(string name, object value)
		{
			string valueString = GetValueStringRepresentation(value);
			return AppendPropertyRaw(name, valueString);
		}

		/// <summary>
		/// Adds the property with the given name and value to the builder without any additional formatting.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public ReprBuilder AppendPropertyRaw(string name, string value)
		{
			m_PropertyOrder.Add(new KeyValuePair<string, string>(name, value));
			return this;
		}

		/// <summary>
		/// Returns the result of the builder.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (m_Instance == null)
				return GetValueStringRepresentation(null);

			StringBuilder builder = new StringBuilder();

			builder.Append(m_Instance.GetType().Name);
			builder.Append('(');

			for (int index = 0; index < m_PropertyOrder.Count; index++)
			{
				KeyValuePair<string, string> pair = m_PropertyOrder[index];

				builder.Append(pair.Key);
				builder.Append('=');
				builder.Append(pair.Value);

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
