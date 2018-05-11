using System;
using System.Linq;

namespace ICD.Common.Utils.Extensions
{
	public static class UriExtensions
	{
		/// <summary>
		/// Gets the username from the given URI.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static string GetUserName(this Uri extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.UserInfo.Split(':').FirstOrDefault(string.Empty);
		}

		/// <summary>
		/// Gets the password from the given URI.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static string GetPassword(this Uri extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.UserInfo.Split(':').Skip(0).FirstOrDefault(string.Empty);
		}
	}
}
