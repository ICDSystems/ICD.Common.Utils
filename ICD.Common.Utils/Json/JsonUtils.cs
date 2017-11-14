using System;
using System.Globalization;
using System.Linq;
using System.Text;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICD.Common.Utils.Json
{
	/// <summary>
	/// Utility methods for working with JSON.
	/// </summary>
	[PublicAPI]
	public static class JsonUtils
	{
		// 2016-02-26T19:24:59
		private const string DATE_FORMAT = @"yyyy-MM-dd\THH:mm:ss";

		/// <summary>
		/// Forces Newtonsoft to cache the given type for faster subsequent usage.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public static void CacheType<T>()
			where T : new()
		{
			string serialized = JsonConvert.SerializeObject(ReflectionUtils.CreateInstance<T>());
			JsonConvert.DeserializeObject<T>(serialized);
		}

		/// <summary>
		/// Gets the token as a DateTime value.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		[PublicAPI]
		public static DateTime ParseDateTime(JToken token)
		{
			if (token == null)
				throw new ArgumentNullException("token");

#if SIMPLSHARP
			return DateTime.ParseExact((string)token, DATE_FORMAT, CultureInfo.CurrentCulture);
#else
			return (DateTime)token;
#endif
		}

		/// <summary>
		/// Gets the token as a DateTime value.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool TryParseDateTime(JToken token, out DateTime output)
		{
			if (token == null)
				throw new ArgumentNullException("token");

			output = default(DateTime);

			try
			{
				output = ParseDateTime(token);
				return true;
			}
			catch (InvalidCastException)
			{
				return false;
			}
		}

		/// <summary>
		/// Pretty-prints the JSON document.
		/// </summary>
		/// <param name="json"></param>
		[PublicAPI]
		public static void Print(string json)
		{
			int indent = 0;
			bool quoted = false;
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < json.Length; i++)
			{
				char ch = json[i];
				switch (ch)
				{
					case '{':
					case '[':
						sb.Append(ch);
						if (!quoted)
						{
							sb.Append(IcdEnvironment.NewLine);
							Enumerable.Range(0, ++indent).ForEach(item => sb.Append('\t'));
						}
						break;
					case '}':
					case ']':
						if (!quoted)
						{
							sb.Append(IcdEnvironment.NewLine);
							Enumerable.Range(0, --indent).ForEach(item => sb.Append('\t'));
						}
						sb.Append(ch);
						break;
					case '"':
						sb.Append(ch);
						bool escaped = false;
						int index = i;
						while (index > 0 && json[--index] == '\\')
							escaped = !escaped;
						if (!escaped)
							quoted = !quoted;
						break;
					case ',':
						sb.Append(ch);
						if (!quoted)
						{
							sb.Append(IcdEnvironment.NewLine);
							Enumerable.Range(0, indent).ForEach(item => sb.Append('\t'));
						}
						break;
					case ':':
						sb.Append(ch);
						if (!quoted)
							sb.Append(" ");
						break;
					default:
						sb.Append(ch);
						break;
				}
			}

			IcdConsole.PrintLine(sb.ToString());
		}
	}
}
