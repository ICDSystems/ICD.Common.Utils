using System;
using ICD.Common.Properties;

namespace ICD.Common.Utils.EventArguments
{
	public sealed class XmlRecursionEventArgs : EventArgs
	{
		[PublicAPI]
		public string Outer { get; private set; }

		[PublicAPI]
		public string[] Path { get; private set; }

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="outer"></param>
		/// <param name="path"></param>
		public XmlRecursionEventArgs(string outer, string[] path)
		{
			Outer = outer;
			Path = path;
		}

		#endregion
	}
}
