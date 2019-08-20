using System;
using System.Collections.Generic;
#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif
using ICD.Common.Properties;
using Newtonsoft.Json;

namespace ICD.Common.Utils.Json
{
	/// <summary>
	/// Simply reads/writes values from/to JSON as their string representation.
	/// </summary>
	[PublicAPI]
	public sealed class ToStringJsonConverter : JsonConverter
	{
		private static readonly Dictionary<Type, MethodInfo> s_ParseMethods; 

		public override bool CanRead { get { return true; } }

		/// <summary>
		/// Static constructor.
		/// </summary>
		static ToStringJsonConverter()
		{
			s_ParseMethods = new Dictionary<Type, MethodInfo>();
		}

		#region Methods

		public override bool CanConvert(Type objectType)
		{
			return true;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToString());
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			MethodInfo parse = GetParseMethod(objectType);
			if (parse == null)
				throw new ArgumentException(
				string.Format("{0} does not have a 'static {0} Parse(string)' method.", objectType.Name),
				"objectType");

			try
			{
				return parse.Invoke(null, new[] { reader.Value });
			}
			catch (TargetInvocationException e)
			{
				throw e.InnerException;
			}
		}

		#endregion

		[CanBeNull]
		private static MethodInfo GetParseMethod(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			MethodInfo output;
			if (!s_ParseMethods.TryGetValue(type, out output))
			{

				output = type
#if SIMPLSHARP
					.GetCType()
					.GetMethod("Parse",
					           BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
					           CType.DefaultBinder,
					           new CType[] {typeof(string)},
					           new ParameterModifier[] {});
#else
					.GetTypeInfo()
					.GetMethod("Parse",
					           BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
					           Type.DefaultBinder,
					           new[] {typeof(string)},
					           new ParameterModifier[] {});
#endif

				s_ParseMethods.Add(type, output);
			}

			return output;
		}
	}
}
