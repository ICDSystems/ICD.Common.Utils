using System;
using System.Linq;
using ICD.Common.Properties;

namespace ICD.Common.Utils.Extensions
{
	public static class UriExtensions
	{
		/// <summary>
		/// Gets the username from the given URI.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[NotNull]
		public static string GetUserName([NotNull] this Uri extends)
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
		[NotNull]
		public static string GetPassword([NotNull] this Uri extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.UserInfo.Split(':').Skip(1).FirstOrDefault(string.Empty);
		}

		/// <summary>
		/// Returns true if the URI matches the default http://localhost/
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static bool GetIsDefault([NotNull] this Uri extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.ToString() == "http://localhost/";
		}

		/// <summary>
		/// Returns the string representation of the given URI, replacing the password with asterixes.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static string ToPrivateString([NotNull] this Uri extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			IcdUriBuilder builder = new IcdUriBuilder(extends);
			builder.Password = builder.Password == null ? null : StringUtils.PasswordFormat(builder.Password);
			
			return builder.ToString();
		}
	}
}
