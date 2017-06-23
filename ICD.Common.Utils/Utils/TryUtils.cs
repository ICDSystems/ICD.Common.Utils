using System;

namespace ICD.Common.Utils
{
	public static class TryUtils
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TResult">return type</typeparam>
		/// <param name="function"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public static bool Try<TResult>(Func<TResult> function, out TResult val)
		{
			if (function == null)
				throw new ArgumentNullException("function");

			try
			{
				val = function();
				return true;
			}
			catch (Exception)
			{
				val = default(TResult);
				return false;
			}
		}

		public static bool Try<T1, TResult>(Func<T1, TResult> function, T1 param1, out TResult val)
		{
			if (function == null)
				throw new ArgumentNullException("function");

			return Try(() => function(param1), out val);
		}

		public static bool Try<T1, T2, TResult>(Func<T1, T2, TResult> function, T1 param1, T2 param2, out TResult val)
		{
			if (function == null)
				throw new ArgumentNullException("function");

			return Try(() => function(param1, param2), out val);
		}

		public static bool Try<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function, T1 param1, T2 param2, T3 param3,
		                                            out TResult val)
		{
			if (function == null)
				throw new ArgumentNullException("function");

			return Try(() => function(param1, param2, param3), out val);
		}
	}
}
