﻿using System;
using System.Collections.Generic;
using System.Linq;
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

		private static readonly Dictionary<Type, Type[]> s_TypeAllTypes;
		private static readonly Dictionary<Type, Type[]> s_TypeBaseTypes; 

		/// <summary>
		/// Static constructor.
		/// </summary>
		static TypeExtensions()
		{
			s_TypeAllTypes = new Dictionary<Type, Type[]>();
			s_TypeBaseTypes = new Dictionary<Type, Type[]>();
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

			if (!s_TypeAllTypes.ContainsKey(extends))
			{
				Type[] types =
					extends.GetBaseTypes()
					       .Concat(extends.GetInterfaces())
					       .Prepend(extends)
					       .ToArray();

				s_TypeAllTypes.Add(extends, types);
			}

			return s_TypeAllTypes[extends];
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

			if (!s_TypeBaseTypes.ContainsKey(extends))
			{
				Type[] types = GetBaseTypesInternal(extends).ToArray();
				s_TypeBaseTypes.Add(extends, types);
			}

			return s_TypeBaseTypes[extends];
		}

		private static IEnumerable<Type> GetBaseTypesInternal(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

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
	}
}
