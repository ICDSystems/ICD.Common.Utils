using System;
using ICD.Common.Utils.Extensions;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Extensions
{
	[TestFixture]
	public sealed class UriExtensionsTest
	{
		[TestCase("http://username:password@test.com/", "username")]
		public void GetUserName(string uriString, string expected)
		{
			Assert.AreEqual(expected, new Uri(uriString).GetUserName());
		}

		[TestCase("http://username:password@test.com/", "password")]
		public void GetPassword(string uriString, string expected)
		{
			Assert.AreEqual(expected, new Uri(uriString).GetPassword());
		}

		[TestCase("http://www.test.com/a/b/c", "http://www.test.com/a/b/")]
		[TestCase("http://www.test.com/a/b/", "http://www.test.com/a/")]
		[TestCase("http://www.test.com/", "http://www.test.com/")]
		public void GetParentUri(string uriString, string expected)
		{
			Assert.AreEqual(expected, new Uri(uriString).GetParentUri().ToString());
		}
	}
}
