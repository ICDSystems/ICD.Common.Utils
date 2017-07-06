using System.Text;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
#else
using System.IO;
#endif

namespace ICD.Common.Utils.IO
{
	public class IcdStringWriter : IcdTextWriter
	{
		public StringWriter WrappedStringWriter { get { return WrappedTextWriter as StringWriter; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="stringWriter"></param>
		public IcdStringWriter(StringWriter stringWriter)
			: base(stringWriter)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="stringBuilder"></param>
		public IcdStringWriter(StringBuilder stringBuilder)
			: this(new StringWriter(stringBuilder))
		{
		}
	}
}
