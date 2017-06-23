using System;
using System.Collections.Generic;
using System.Text;
using ICD.Common.Properties;

namespace ICD.Common.Utils
{
	/// <summary>
	/// Utils for printing various data types in human readable structures.
	/// </summary>
	public static class PrettyPrint
	{
		#region Methods

		[PublicAPI]
		public static void PrintLine<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
		{
			if (dictionary == null)
				throw new ArgumentNullException("dictionary");

			IcdConsole.PrintLine(ToString(dictionary));
		}

		[PublicAPI]
		public static void PrintLine<T>(IEnumerable<T> sequence)
		{
			if (sequence == null)
				throw new ArgumentNullException("sequence");

			IcdConsole.PrintLine(ToString(sequence));
		}

		[PublicAPI]
		public static string ToString<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
		{
			if (dictionary == null)
				throw new ArgumentNullException("dictionary");

			StringBuilder builder = new StringBuilder();
			builder.AppendLine("{");

			foreach (KeyValuePair<TKey, TValue> kvp in dictionary)
			{
				builder.Append('\t');
				builder.Append(ToString(kvp.Key));
				builder.Append(" : ");
				builder.Append(ToString(kvp.Value));
				builder.Append(',');
				builder.AppendLine();
			}

			builder.Append("}");
			return builder.ToString();
		}

		[PublicAPI]
		public static string ToString<T>(IEnumerable<T> sequence)
		{
			if (sequence == null)
				throw new ArgumentNullException("sequence");

			StringBuilder builder = new StringBuilder();
			builder.AppendLine("[");

			foreach (T item in sequence)
			{
				builder.Append('\t');
				builder.Append(ToString(item));
				builder.Append(',');
				builder.AppendLine();
			}

			builder.Append("]");
			return builder.ToString();
		}

		[PublicAPI]
		public static string ToString<T>(T value)
		{
// ReSharper disable once CompareNonConstrainedGenericWithNull
			return StringUtils.ToRepresentation(value == null ? null : value.ToString());
		}

		#endregion
	}
}
