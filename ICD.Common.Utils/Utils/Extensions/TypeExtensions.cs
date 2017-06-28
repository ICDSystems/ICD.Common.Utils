using System;

namespace ICD.Common.Utils.Extensions
{
	public static class TypeExtensions
	{
		public static bool IsAssignableTo(this Type from, Type to)
		{
			return to.IsAssignableFrom(from);
		}
	}
}