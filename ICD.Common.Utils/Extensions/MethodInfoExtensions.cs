using System;
using System.Text;
using ICD.Common.Properties;
#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif

namespace ICD.Common.Utils.Extensions
{
	public static class MethodInfoExtensions
	{
		/// <summary>
		/// Return the method signature as a string.
		/// </summary>
		/// <param name="method">The Method</param>
		/// <returns>Method signature</returns>
		public static string GetSignature([NotNull]this MethodInfo method)
		{
			if (method == null)
				throw new ArgumentNullException("method");

			return method.GetSignature(false);
		}

		/// <summary>
		/// Return the method signature as a string.
		/// </summary>
		/// <param name="method">The Method</param>
		/// <param name="callable">Return as a callable string(public void a(string b) would return a(b))</param>
		/// <returns>Method signature</returns>
		public static string GetSignature([NotNull]this MethodInfo method, bool callable)
		{
			if (method == null)
				throw new ArgumentNullException("method");

			bool firstParam = true;
			StringBuilder sigBuilder = new StringBuilder();

			if (callable == false)
			{
				if (method.IsPublic)
					sigBuilder.Append("public ");
				else if (method.IsPrivate)
					sigBuilder.Append("private ");
				else if (method.IsAssembly)
					sigBuilder.Append("internal ");
				if (method.IsFamily)
					sigBuilder.Append("protected ");
				if (method.IsStatic)
					sigBuilder.Append("static ");

				Type returnType = method.ReturnType;

				sigBuilder.Append(returnType.GetSyntaxName());
				sigBuilder.Append(' ');
			}

			sigBuilder.Append(method.Name);

			// Add method generics
			if (method.IsGenericMethod)
			{
				sigBuilder.Append("<");
				foreach (Type g in method.GetGenericArguments())
				{
					if (firstParam)
						firstParam = false;
					else
						sigBuilder.Append(", ");

					sigBuilder.Append(g.GetSyntaxName());
				}
				sigBuilder.Append(">");
			}

			sigBuilder.Append("(");

			firstParam = true;

#if !SIMPLSHARP
			bool secondParam = false;
#endif

			foreach (ParameterInfo param in method.GetParameters())
			{
				if (firstParam)
				{
					firstParam = false;

#if SIMPLSHARP
					// TODO - RestrictionViolationException: System.Runtime.CompilerServices.ExtensionAttribute - Not allowed due to restrictions
#else
					if (method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false))
					{
						if (callable)
						{
							secondParam = true;
							continue;
						}
						sigBuilder.Append("this ");
					}
				}
				else if (secondParam)
				{
					secondParam = false;
#endif
				}
				
				else
					sigBuilder.Append(", ");

				if (param.ParameterType.IsByRef)
					sigBuilder.Append("ref ");
				else if (param.GetIsOut())
					sigBuilder.Append("out ");
				if (!callable)
				{
					Type parameterType = param.ParameterType;

					sigBuilder.Append(parameterType.GetSyntaxName());
					sigBuilder.Append(' ');
				}

				sigBuilder.Append(param.Name);
			}

			sigBuilder.Append(")");

			return sigBuilder.ToString();
		}

		/// <summary>
		/// Cross-platform shim for getting MethodInfo for the delegate.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static MethodInfo GetMethodInfo([NotNull]this Delegate extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

#if SIMPLSHARP
			return extends.GetMethod();
#else
			return extends.Method;
#endif
		}
	}
}
