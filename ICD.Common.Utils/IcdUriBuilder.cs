using System;
#if !SIMPLSHARP
using System.Linq;
#endif
using System.Text;
using System.Text.RegularExpressions;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils
{
	/// <summary>
	/// Simple Compact Framework UriBuilder implementation.
	/// </summary>
	public sealed class IcdUriBuilder
	{
		private string m_Query;

		#region Properties

		/// <summary>
		/// Gets or sets the fragment portion of the URI.
		/// </summary>
		public string Fragment { get; set; }

		/// <summary>
		/// Gets or sets the Domain Name System (DNS) host name or IP address of a server.
		/// </summary>
		public string Host { get; set; }

		/// <summary>
		/// Gets or sets the password associated with the user that accesses the URI.
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the path to the resource referenced by the URI.
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// Gets or sets the port number of the URI.
		/// </summary>
		public ushort Port { get; set; }

		/// <summary>
		/// Gets or sets any query information included in the URI. 
		/// </summary>
		public string Query { get { return string.IsNullOrEmpty(m_Query) ? string.Empty : "?" + m_Query; } set { m_Query = value; } }

		/// <summary>
		/// Gets or sets the scheme name of the URI.
		/// </summary>
		public string Scheme { get; set; }

		/// <summary>
		/// The user name associated with the user that accesses the URI.
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// Gets the Uri instance constructed by the specified UriBuilder instance.
		/// </summary>
		public Uri Uri { get { return new Uri(ToString()); } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public IcdUriBuilder()
			: this(new Uri("http://localhost/"))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="uri"></param>
		public IcdUriBuilder(string uri)
			: this(new Uri(uri, UriKind.RelativeOrAbsolute))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="uri"></param>
		public IcdUriBuilder(Uri uri)
		{
			if (uri == null)
				throw new ArgumentNullException("uri");

			if (!uri.IsAbsoluteUri)
				uri = new Uri(Uri.UriSchemeHttp + Uri.SchemeDelimiter + uri);

			Fragment = uri.Fragment;
			Host = uri.Host;
			Password = uri.GetPassword();
			Path = uri.AbsolutePath;
			Port = (ushort)uri.Port;
			Query = uri.Query.TrimStart('?');
			Scheme = uri.Scheme;
			UserName = uri.GetUserName();
		}

		/// <summary>
		/// Builds the string representation for the URI.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			// URI = [scheme://][authority]path[?query][#fragment]
			// authority = [userinfo@]host[:port]
			// userinfo = username[:password]

			StringBuilder builder = new StringBuilder();

			// Scheme
			if (!string.IsNullOrEmpty(Scheme))
			{
				builder.Append(Scheme);
				builder.Append("://");
			}

			// Authority
			if (!string.IsNullOrEmpty(UserName))
			{
				builder.Append(UserName);

				if (!string.IsNullOrEmpty(Password))
				{
					builder.Append(':');
					builder.Append(Password);
				}

				builder.Append('@');
			}

			builder.Append(Host);

			if (Port != 0)
			{
				builder.Append(':');
				builder.Append(Port);
			}

			// Path
			if (string.IsNullOrEmpty(Path) || !Path.StartsWith("/"))
				builder.Append('/');
			builder.Append(Path);

			// Query
			if (!string.IsNullOrEmpty(Query))
				builder.Append(Query);

			// Fragment
			if (!string.IsNullOrEmpty(Fragment))
			{
				builder.Append('#');
				builder.Append(Fragment);
			}

			return builder.ToString();
		} 

		/// <summary>
		/// Appends the given path to the current path, ensuring only one separator between parts.
		/// </summary>
		/// <param name="parts"></param>
		/// <returns></returns>
		public void AppendPath(params string[] parts)
		{
			parts = parts.Prepend(Path).ToArray(parts.Length + 1);
			Path = Combine(parts);
		}

		#region Flurl

		// The following region is taken from Flurl https://github.com/tmenier/Flurl

		/*
			MIT License

			Copyright (c) 2018 Todd Menier

			Permission is hereby granted, free of charge, to any person obtaining a copy
			of this software and associated documentation files (the "Software"), to deal
			in the Software without restriction, including without limitation the rights
			to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
			copies of the Software, and to permit persons to whom the Software is
			furnished to do so, subject to the following conditions:

			The above copyright notice and this permission notice shall be included in all
			copies or substantial portions of the Software.

			THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
			IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
			FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
			AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
			LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
			OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
			SOFTWARE.
		 */

		/// <summary>
		/// Basically a Path.Combine for URLs. Ensures exactly one '/' seperates each segment,
		/// and exactly on '&amp;' seperates each query parameter.
		/// URL-encodes illegal characters but not reserved characters.
		/// </summary>
		/// <param name="parts">URL parts to combine.</param>
		public static string Combine(params string[] parts)
		{
			if (parts == null)
				throw new ArgumentNullException("parts");

			string result = "";
			bool inQuery = false, inFragment = false;

			foreach (var part in parts)
			{
				if (string.IsNullOrEmpty(part))
					continue;

				if (result.EndsWith("?") || part.StartsWith("?"))
					result = CombineEnsureSingleSeperator(result, part, '?');
				else if (result.EndsWith("#") || part.StartsWith("#"))
					result = CombineEnsureSingleSeperator(result, part, '#');
				else if (inFragment)
					result += part;
				else if (inQuery)
					result = CombineEnsureSingleSeperator(result, part, '&');
				else
					result = CombineEnsureSingleSeperator(result, part, '/');

				if (part.Contains("#"))
				{
					inQuery = false;
					inFragment = true;
				}
				else if (!inFragment && part.Contains("?"))
				{
					inQuery = true;
				}
			}
			return EncodeIllegalCharacters(result, false);
		}

		private static string CombineEnsureSingleSeperator(string a, string b, char seperator)
		{
			if (string.IsNullOrEmpty(a)) return b;
			if (string.IsNullOrEmpty(b)) return a;
			return a.TrimEnd(seperator) + seperator + b.TrimStart(seperator);
		}

		/// <summary>
		/// URL-encodes characters in a string that are neither reserved nor unreserved. Avoids encoding reserved characters such as '/' and '?'. Avoids encoding '%' if it begins a %-hex-hex sequence (i.e. avoids double-encoding).
		/// </summary>
		/// <param name="s">The string to encode.</param>
		/// <param name="encodeSpaceAsPlus">If true, spaces will be encoded as + signs. Otherwise, they'll be encoded as %20.</param>
		/// <returns>The encoded URL.</returns>
		private static string EncodeIllegalCharacters(string s, bool encodeSpaceAsPlus) {
			if (string.IsNullOrEmpty(s))
				return s;

			if (encodeSpaceAsPlus)
				s = s.Replace(" ", "+");

			// Uri.EscapeUriString mostly does what we want - encodes illegal characters only - but it has a quirk
			// in that % isn't illegal if it's the start of a %-encoded sequence https://stackoverflow.com/a/47636037/62600

			// no % characters, so avoid the regex overhead
			if (!s.Contains("%"))
				return Uri.EscapeUriString(s);

			// pick out all %-hex-hex matches and avoid double-encoding 
			return Regex.Replace(s, "(.*?)((%[0-9A-Fa-f]{2})|$)", c => {
				var a = c.Groups[1].Value; // group 1 is a sequence with no %-encoding - encode illegal characters
				var b = c.Groups[2].Value; // group 2 is a valid 3-character %-encoded sequence - leave it alone!
				return Uri.EscapeUriString(a) + b;
			});
		}

		#endregion
	}
}
