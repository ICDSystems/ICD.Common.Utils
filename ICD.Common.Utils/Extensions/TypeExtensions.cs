using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICD.Common.Utils.Collections;
#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif

namespace ICD.Common.Utils.Extensions
{
	public static class TypeExtensions
	{
		private static readonly IcdHashSet<Type> s_NumericTypes = new IcdHashSet<Type>
		{
			typeof(byte),
			typeof(decimal),
			typeof(double),
			typeof(float),
			typeof(int),
			typeof(long),
			typeof(sbyte),
			typeof(short),
			typeof(uint),
			typeof(ulong),
			typeof(ushort)
		};

		private static readonly IcdHashSet<Type> s_SignedNumericTypes = new IcdHashSet<Type>
		{
			typeof(decimal),
			typeof(double),
			typeof(float),
			typeof(int),
			typeof(long),
			typeof(sbyte),
			typeof(short),
		};

		private static readonly IcdHashSet<Type> s_DecimalNumericTypes = new IcdHashSet<Type>
		{
			typeof(decimal),
			typeof(double),
			typeof(float),
		};

		private static readonly IcdHashSet<Type> s_IntegerNumericTypes = new IcdHashSet<Type>
		{
			typeof(byte),
			typeof(int),
			typeof(long),
			typeof(sbyte),
			typeof(short),
			typeof(uint),
			typeof(ulong),
			typeof(ushort)
		};

		private static readonly Dictionary<Type, Type[]> s_TypeAllTypes;
		private static readonly Dictionary<Type, Type[]> s_TypeBaseTypes;
		private static readonly Dictionary<Type, Type[]> s_TypeImmediateInterfaces;
		private static readonly Dictionary<Type, Type[]> s_TypeMinimalInterfaces;
		private static readonly Dictionary<Type, string> s_TypeToMinimalName;
		private static readonly Dictionary<Type, string> s_TypeToNameWithoutAssemblyDetails;

		/// <summary>
		/// Static constructor.
		/// </summary>
		static TypeExtensions()
		{
			s_TypeAllTypes = new Dictionary<Type, Type[]>();
			s_TypeBaseTypes = new Dictionary<Type, Type[]>();
			s_TypeImmediateInterfaces = new Dictionary<Type, Type[]>();
			s_TypeMinimalInterfaces = new Dictionary<Type, Type[]>();
			s_TypeToMinimalName = new Dictionary<Type, string>();
			s_TypeToNameWithoutAssemblyDetails = new Dictionary<Type, string>();
		}

		/// <summary>
		/// Returns true if the given type can represent a null value.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static bool CanBeNull(this Type extends)
		{
			if (extends == null)
				throw new ArgumentException("extends");

			return !extends.IsValueType || Nullable.GetUnderlyingType(extends) != null;
		}

		/// <summary>
		/// Returns true if the given type is a numeric type.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static bool IsNumeric(this Type extends)
		{
			if (extends == null)
				throw new ArgumentException("extends");

			return s_NumericTypes.Contains(extends);
		}

		/// <summary>
		/// Returns true if the given type is a signed numeric type.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static bool IsSignedNumeric(this Type extends)
		{
			if (extends == null)
				throw new ArgumentException("extends");

			return s_SignedNumericTypes.Contains(extends);
		}

		/// <summary>
		/// Returns true if the given type is a non-integer numeric type.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static bool IsDecimalNumeric(this Type extends)
		{
			if (extends == null)
				throw new ArgumentException("extends");

			return s_DecimalNumericTypes.Contains(extends);
		}

		/// <summary>
		/// Returns true if the given type is an integer numeric type.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static bool IsIntegerNumeric(this Type extends)
		{
			if (extends == null)
				throw new ArgumentException("extends");

			return s_IntegerNumericTypes.Contains(extends);
		}

		/// <summary>
		/// Gets the Assembly containing the type.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static Assembly GetAssembly(this Type extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends
#if SIMPLSHARP
				.GetCType()
#else
				.GetTypeInfo()
#endif
				.Assembly;
		}

		/// <summary>
		/// Returns true if the type is assignable to the given type.
		/// </summary>
		/// <param name="from"></param>
		/// <returns></returns>
		public static bool IsAssignableTo<T>(this Type from)
		{
			if (from == null)
				throw new ArgumentNullException("from");

			return from.IsAssignableTo(typeof(T));
		}

		/// <summary>
		/// Returns true if the type is assignable to the given type.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public static bool IsAssignableTo(this Type from, Type to)
		{
			if (from == null)
				throw new ArgumentNullException("from");

			if (to == null)
				throw new ArgumentNullException("to");

			return to.IsAssignableFrom(from);
		}

		/// <summary>
		/// Returns the given type, all base types, and all implemented interfaces.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static IEnumerable<Type> GetAllTypes(this Type extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			Type[] types;
			if (!s_TypeAllTypes.TryGetValue(extends, out types))
			{
				types = extends.GetBaseTypes()
				               .Concat(extends.GetInterfaces())
				               .Prepend(extends)
				               .ToArray();

				s_TypeAllTypes[extends] = types;
			}

			return types;
		}

		/// <summary>
		/// Returns all base types for the given type.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static IEnumerable<Type> GetBaseTypes(this Type extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			Type[] types;
			if (!s_TypeBaseTypes.TryGetValue(extends, out types))
			{
				types = GetBaseTypesIterator(extends).ToArray();
				s_TypeBaseTypes[extends] = types;
			}

			return types;
		}

		private static IEnumerable<Type> GetBaseTypesIterator(Type type)
		{
			do
			{
				type = type
#if !SIMPLSHARP
					.GetTypeInfo()
#endif
					.BaseType;

				if (type != null)
					yield return type;
			} while (type != null);
		}

		/// <summary>
		/// Gets the interfaces that the given type implements that are not implemented further
		/// down the inheritance chain.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static IEnumerable<Type> GetImmediateInterfaces(this Type extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			Type[] immediateInterfaces;
			if (!s_TypeImmediateInterfaces.TryGetValue(extends, out immediateInterfaces))
			{
				IEnumerable<Type> allInterfaces = extends.GetInterfaces();

				IEnumerable<Type> childInterfaces =
					extends.GetAllTypes()
					       .Except(extends)
					       .SelectMany(t => t.GetImmediateInterfaces())
					       .Distinct();

				immediateInterfaces = allInterfaces.Except(childInterfaces).ToArray();

				s_TypeImmediateInterfaces[extends] = immediateInterfaces;
			}

			return immediateInterfaces;
		}

		/// <summary>
		/// Gets the smallest set of interfaces for the given type that cover all implemented interfaces.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static IEnumerable<Type> GetMinimalInterfaces(this Type extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			Type[] minimalInterfaces;
			if (!s_TypeMinimalInterfaces.TryGetValue(extends, out minimalInterfaces))
			{
				Type[] allInterfaces = extends.GetInterfaces();
				minimalInterfaces = allInterfaces.Except(allInterfaces.SelectMany(t => t.GetInterfaces()))
				                                 .ToArray();

				s_TypeMinimalInterfaces[extends] = minimalInterfaces;
			}

			return minimalInterfaces;
		}

		/// <summary>
		/// Gets the Type name without any trailing generic information.
		/// 
		/// E.g.
		///		List`1
		/// Becomes
		///		List
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static string GetNameWithoutGenericArity(this Type extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			string name = extends.Name;
			int index = name.IndexOf('`');
			return index == -1 ? name : name.Substring(0, index);
		}

		/// <summary>
		/// Gets the smallest possible string representation for the given type that
		/// can be converted back to a Type via Type.GetType(string).
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static string GetMinimalName(this Type extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			string name;
			if (!s_TypeToMinimalName.TryGetValue(extends, out name))
			{
				// Generics are a pain
				if (extends.IsGenericType)
				{
					string nameWithoutAssemblyDetails = Type.GetType(extends.FullName) == null
						                                    ? extends.GetNameWithoutAssemblyDetails()
						                                    : extends.FullName;
					int genericStart = nameWithoutAssemblyDetails.IndexOf('[');
					if (genericStart < 0)
					{
						name = nameWithoutAssemblyDetails;
					}
					else
					{
						string genericParameterNames =
							string.Join("],[", extends.GetGenericArguments().Select(t => t.GetMinimalName()).ToArray());
						int genericEnd = nameWithoutAssemblyDetails.LastIndexOf(']');

						name = new StringBuilder().Append(nameWithoutAssemblyDetails, 0, genericStart + 2)
						                          .Append(genericParameterNames)
						                          .Append(nameWithoutAssemblyDetails, genericEnd - 1,
						                                  nameWithoutAssemblyDetails.Length - genericEnd + 1)
						                          .ToString();
					}
				}
				else
				{
					name = Type.GetType(extends.FullName) == null
						? extends.GetNameWithoutAssemblyDetails()
						: extends.FullName;
				}

				s_TypeToMinimalName[extends] = name;
			}

			return name;
		}

		/// <summary>
		/// Gets the string representation for the type.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static string GetNameWithoutAssemblyDetails(this Type extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			string name;
			if (!s_TypeToNameWithoutAssemblyDetails.TryGetValue(extends, out name))
			{
				name = RemoveAssemblyDetails(extends.AssemblyQualifiedName);
				s_TypeToNameWithoutAssemblyDetails[extends] = name;
			}

			return name;
		}

		/// <summary>
		/// Taken from Newtonsoft.Json.Utilities.ReflectionUtils
		/// Removes the assembly details from a type assembly qualified name.
		/// </summary>
		/// <param name="fullyQualifiedTypeName"></param>
		/// <returns></returns>
		private static string RemoveAssemblyDetails(string fullyQualifiedTypeName)
		{
			StringBuilder builder = new StringBuilder();

			// loop through the type name and filter out qualified assembly details from nested type names
			bool writingAssemblyName = false;
			bool skippingAssemblyDetails = false;
			foreach (char current in fullyQualifiedTypeName)
			{
				switch (current)
				{
					case '[':
						writingAssemblyName = false;
						skippingAssemblyDetails = false;
						builder.Append(current);
						break;
					case ']':
						writingAssemblyName = false;
						skippingAssemblyDetails = false;
						builder.Append(current);
						break;
					case ',':
						if (!writingAssemblyName)
						{
							writingAssemblyName = true;
							builder.Append(current);
						}
						else
						{
							skippingAssemblyDetails = true;
						}
						break;
					default:
						if (!skippingAssemblyDetails)
						{
							builder.Append(current);
						}
						break;
				}
			}

			return builder.ToString();
		}

		/// <summary>
		/// Gets the type name as it would appear in code.
		/// </summary>
		/// <param name="extends">Type. May be generic or nullable</param>
		/// <returns>Full type name, fully qualified namespaces</returns>
		public static string GetSyntaxName(this Type extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			// Nullable
			Type nullableType = Nullable.GetUnderlyingType(extends);
			if (nullableType != null)
				return nullableType.GetSyntaxName() + "?";

			// Generic
			if (extends.IsGenericType)
			{
				StringBuilder sb = new StringBuilder(extends.Name.Substring(0, extends.Name.IndexOf('`')));
				sb.Append('<');

				bool first = true;
				foreach (Type t in extends.GetGenericArguments())
				{
					if (!first)
						sb.Append(',');
					sb.Append(t.GetSyntaxName());
					first = false;
				}

				sb.Append('>');

				return sb.ToString();
			}

			// Default
			switch (extends.Name)
			{
				case "String":
					return "string";
				case "Int32":
					return "int";
				case "Decimal":
					return "decimal";
				case "Object":
					return "object";
				case "Void":
					return "void";

				default:
					return extends.Name;
			}
		}
	}
}
