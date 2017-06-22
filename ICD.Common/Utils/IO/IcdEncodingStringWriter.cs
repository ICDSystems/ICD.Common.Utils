#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
#else
using System.IO;
#endif
using System;
using System.Text;

namespace ICD.Common.Utils.IO
{
	public sealed class IcdEncodingStringWriter : IcdStringWriter
	{
		public IcdEncodingStringWriter(StringBuilder output, Encoding encoding)
			: base(new EncodingStringWriter(output, encoding))
		{
		}

		private sealed class EncodingStringWriter : StringWriter
		{
			private readonly Encoding m_Encoding;

			public override Encoding Encoding { get { return m_Encoding; } }

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="builder"></param>
			/// <param name="encoding"></param>
			public EncodingStringWriter(StringBuilder builder, Encoding encoding)
				: base(builder)
			{
				if (encoding == null)
					throw new ArgumentNullException("encoding");

				m_Encoding = encoding;
			}
		}
	}
}
