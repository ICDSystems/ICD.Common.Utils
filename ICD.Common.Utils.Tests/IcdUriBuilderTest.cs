using NUnit.Framework;

namespace ICD.Common.Utils.Tests
{
	[TestFixture]
	public sealed class IcdUriBuilderTest
	{
		#region Properties

		[TestCase("test")]
		public void FragmentTest(string fragment)
		{
			Assert.AreEqual(fragment, new IcdUriBuilder {Fragment = fragment}.Fragment);
		}

		[TestCase("test")]
		public void HostTest(string host)
		{
			Assert.AreEqual(host, new IcdUriBuilder {Host = host}.Host);
		}

		[TestCase("test")]
		public void PasswordTest(string fragment)
		{
			Assert.AreEqual(fragment, new IcdUriBuilder {Password = fragment}.Password);
		}

		[TestCase("test")]
		public void PathTest(string fragment)
		{
			Assert.AreEqual(fragment, new IcdUriBuilder {Path = fragment}.Path);
		}

		[TestCase((ushort)80)]
		public void PortTest(ushort port)
		{
			Assert.AreEqual(port, new IcdUriBuilder {Port = port}.Port);
		}

		[TestCase("?test", "test")]
		[TestCase("??test", "?test")]
		public void QueryTest(string expected, string query)
		{
			Assert.AreEqual(expected, new IcdUriBuilder {Query = query}.Query);
		}

		[TestCase("test")]
		public void SchemeTest(string scheme)
		{
			Assert.AreEqual(scheme, new IcdUriBuilder {Scheme = scheme}.Scheme);
		}

		[TestCase("test")]
		public void UserNameTest(string userName)
		{
			Assert.AreEqual(userName, new IcdUriBuilder {UserName = userName}.UserName);
		}

		[Test]
		public void UriTest()
		{
			Assert.Inconclusive();
		}

		#endregion

		[TestCase("/", null, null, null, null, (ushort)0, null, null, null)]
		[TestCase("http:///", null, null, null, null, (ushort)0, null, "http", null)]
		[TestCase("http://localhost:80/", null, "localhost", null, null, (ushort)80, null, "http", null)]
		[TestCase("http://username@localhost/", null, "localhost", null, null, (ushort)0, null, "http", "username")]
		[TestCase("http://localhost/", null, "localhost", "password", null, (ushort)0, null, "http", null)]
		[TestCase("https://localhost/", null, "localhost", null, null, (ushort)0, null, "https", null)]
		[TestCase("http://localhost/test", null, "localhost", null, "test", (ushort)0, null, "http", null)]
		[TestCase("http://localhost/test", null, "localhost", null, "/test", (ushort)0, null, "http", null)]
		[TestCase("http://localhost//test", null, "localhost", null, "//test", (ushort)0, null, "http", null)]
		[TestCase("http://localhost/test?a=b", null, "localhost", null, "/test", (ushort)0, "a=b", "http", null)]
		[TestCase("http://localhost/test??a=b", null, "localhost", null, "/test", (ushort)0, "?a=b", "http", null)]
		public void ToStringTest(string expected, string fragment, string address, string password, string path, ushort port,
		                         string query, string scheme, string userName)
		{
			IcdUriBuilder builder = new IcdUriBuilder
			{
				Fragment = fragment,
				Host = address,
				Password = password,
				Path = path,
				Port = port,
				Query = query,
				Scheme = scheme,
				UserName = userName
			};

			Assert.AreEqual(expected, builder.ToString());
		}
	}
}
