#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using System;

namespace ICD.Common.Utils.Extensions
{
	public static class JsonTokenExtensions
	{
		/// <summary>
		/// Returns true if the JsonToken respresents a single data value
		/// rather than a complex type such as an object or an array.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static bool IsPrimitive(this JsonToken extends)
		{
			switch (extends)
			{
				case JsonToken.None:
				case JsonToken.StartObject:
				case JsonToken.StartArray:
				case JsonToken.StartConstructor:
				case JsonToken.EndObject:
				case JsonToken.EndArray:
				case JsonToken.EndConstructor:
				case JsonToken.Undefined:
					return false;

				case JsonToken.PropertyName:
				case JsonToken.Comment:
				case JsonToken.Raw:
				case JsonToken.Integer:
				case JsonToken.Float:
				case JsonToken.String:
				case JsonToken.Boolean:
				case JsonToken.Null:
				case JsonToken.Date:
				case JsonToken.Bytes:
					return true;

				default:
					throw new ArgumentOutOfRangeException("extends");
			}
		}
	}
}