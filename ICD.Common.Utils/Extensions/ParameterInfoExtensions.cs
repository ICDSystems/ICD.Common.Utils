using System;
using ICD.Common.Properties;
#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif

namespace ICD.Common.Utils.Extensions
{
	public static class ParameterInfoExtensions
	{
		/// <summary>
		/// Returns true if the given parameter is an "out" parameter.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static bool GetIsOut([NotNull] this ParameterInfo extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

#if SIMPLSHARP
			return extends.Attributes.HasFlag(ParameterAttributes.Out);
#else
			return extends.IsOut;
#endif
		}
	}
}
