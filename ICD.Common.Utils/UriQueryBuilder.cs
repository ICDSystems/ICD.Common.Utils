using System.Collections.Generic;
using System.Text;

namespace ICD.Common.Utils
{
	public sealed class UriQueryBuilder
	{
		private readonly Dictionary<string, string> m_Parameters;

		public UriQueryBuilder()
		{
			m_Parameters = new Dictionary<string, string>();
		}

		public UriQueryBuilder Append(string key, string value)
		{
			m_Parameters.Add(key, value);
			return this;
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder("?");

			bool first = true;

			foreach (KeyValuePair<string, string> kvp in m_Parameters)
			{
				if (!first)
					builder.Append('&');
				first = false;

				builder.Append(kvp.Key);
				builder.Append('=');
				builder.Append(kvp.Value);
			}

			return builder.ToString();
		}
	}
}