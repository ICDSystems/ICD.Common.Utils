#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;

#else
using System.IO;
#endif

namespace ICD.Common.Utils.IO
{
	public sealed class IcdStringReader : IcdTextReader
	{
		public StringReader WrappedStringReader { get { return WrappedTextReader as StringReader; } }

		public IcdStringReader(string value)
			: this(new StringReader(value))
		{
		}

		private IcdStringReader(StringReader stringReader)
			: base(stringReader)
		{
		}
	}
}
