using System;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
#else
using System.IO;
#endif

namespace ICD.Common.Utils.IO
{
	public enum eSeekOrigin
	{
		Begin,
		Current,
		End,
	}

	public static class SeekOriginExtensions
	{
		/// <summary>
		/// Converts the seek origin enum to a system seek origin.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static SeekOrigin ToSeekOrigin(this eSeekOrigin extends)
		{
			switch (extends)
			{
				case eSeekOrigin.Begin:
					return SeekOrigin.Begin;
				case eSeekOrigin.Current:
					return SeekOrigin.Current;
				case eSeekOrigin.End:
					return SeekOrigin.End;

				default:
					throw new ArgumentOutOfRangeException("extends");
			}
		}
	}
}
