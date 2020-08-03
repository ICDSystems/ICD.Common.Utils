using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

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

	public static class XmlRecursionEventArgsExtensions
	{
		/// <summary>
		/// Raises the event safely. Simply skips if the handler is null.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="sender"></param>
		/// <param name="outer"></param>
		/// <param name="path"></param>
		public static void Raise([CanBeNull]this EventHandler<XmlRecursionEventArgs> extends, object sender, string outer, string[] path)
		{
			extends.Raise(sender, new XmlRecursionEventArgs(outer, path));
		}
	}
}
