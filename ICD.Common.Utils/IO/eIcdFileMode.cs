using System;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
#else
using System.IO;
#endif

namespace ICD.Common.Utils.IO
{
	/// <summary>
	/// Specifies how the operating system should open a file.
	/// </summary>
	public enum eIcdFileMode
	{
		CreateNew = 1,
		Create = 2,
		Open = 3,
		OpenOrCreate = 4,
		Truncate = 5,
		Append = 6,
	}

	public static class IcdFileModeExtensions
	{
		public static FileMode ToFileMode(this eIcdFileMode extends)
		{
			switch (extends)
			{
				case eIcdFileMode.CreateNew:
					return FileMode.CreateNew;
				case eIcdFileMode.Create:
					return FileMode.Create;
				case eIcdFileMode.Open:
					return FileMode.Open;
				case eIcdFileMode.OpenOrCreate:
					return FileMode.OpenOrCreate;
				case eIcdFileMode.Truncate:
					return FileMode.Truncate;
				case eIcdFileMode.Append:
					return FileMode.Append;
				default:
					throw new ArgumentOutOfRangeException("extends");
			}
		}
	}
}
