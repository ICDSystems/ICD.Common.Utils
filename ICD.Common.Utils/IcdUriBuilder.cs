﻿using System;
using System.Text;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils
{
	/// <summary>
	/// Simple Compact Framework UriBuilder implementation.
	/// </summary>
	public sealed class IcdUriBuilder
	{
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
		public string Query { get; set; }

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
			: this((Uri)null)
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
				return;

			if (!uri.IsAbsoluteUri)
				uri = new Uri(Uri.UriSchemeHttp + Uri.SchemeDelimiter + uri);

			Fragment = uri.Fragment;
			Host = uri.Host;
			Password = uri.GetPassword();
			Path = uri.AbsolutePath;
			Port = (ushort)uri.Port;
			Query = uri.Query;
			Scheme = uri.Scheme;
			UserName = uri.GetUserName();
		}

		/// <summary>
		/// Builds the string representation for the URI.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			// URI = scheme:[//authority]path[?query][#fragment]
			// authority = [userinfo@]host[:port]
			// userinfo = username[:password]

			StringBuilder builder = new StringBuilder();

			// Scheme
			string scheme = string.IsNullOrEmpty(Scheme) ? Uri.UriSchemeHttp : Scheme;
			builder.Append(scheme);
			builder.Append(':');

			// Authority
			builder.Append("//");

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

			string host = string.IsNullOrEmpty(Host) ? "localhost" : Host;
			builder.Append(host);

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
			{
				builder.Append('?');
				builder.Append(Query);
			}

			// Fragment
			if (!string.IsNullOrEmpty(Fragment))
			{
				builder.Append('#');
				builder.Append(Fragment);
			}

			return builder.ToString();
		}
	}
}
