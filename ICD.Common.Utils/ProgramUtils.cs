using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Services;
using ICD.Common.Services.Logging;
using ICD.Common.Utils.Extensions;
#if SIMPLSHARP
using Crestron.SimplSharp;
#endif

namespace ICD.Common.Utils
{
	public static class ProgramUtils
	{
		private const string APPLICATION_NAME_KEY = "Application Name";
		private const string APPLICATION_NAME_SIMPL_KEY = "System Name";
		private const string PROGRAM_FILE_KEY = "Program File";

		private const string COMPILED_ON_KEY = "Compiled On";
		private const string COMPILER_REVISION_KEY = "Compiler Revision";
		private const string COMPILER_REVISION_SIMPL_KEY = "Compiler Rev";

		private static Dictionary<string, string> s_ProgComments;

		#region Properties

		/// <summary>
		/// Lazy-load the prog-comments map.
		/// </summary>
		private static Dictionary<string, string> ProgComments
		{
			get { return s_ProgComments ?? (s_ProgComments = ParseProgComments()); }
		}

		/// <summary>
		/// Gets the program number.
		/// </summary>
		[PublicAPI]
		public static uint ProgramNumber
		{
			get
			{
#if SIMPLSHARP
				return InitialParametersClass.ApplicationNumber;
#else
				return 1;
#endif
			}
		}

		/// <summary>
		/// Gets the program number in the format XX, eg slot 1 is 01.
		/// </summary>
		[PublicAPI]
		public static string ProgramNumberFormatted { get { return string.Format("{0:D2}", ProgramNumber); } }

		/// <summary>
		/// Gets the compile date of the program.
		/// </summary>
		[PublicAPI]
		public static string CompiledDate { get { return ProgComments.GetDefault(COMPILED_ON_KEY, null); } }

		/// <summary>
		/// Gets the compiler revision version.
		/// </summary>
		[PublicAPI]
		public static Version CompilerRevision
		{
			get
			{
				string output;

				if (ProgComments.TryGetValue(COMPILER_REVISION_KEY, out output))
					return new Version(output);

				if (ProgComments.TryGetValue(COMPILER_REVISION_SIMPL_KEY, out output))
					return new Version(output);

				return new Version(0, 0);
			}
		}

		/// <summary>
		/// Gets the name of the program dll.
		/// </summary>
		[PublicAPI]
		public static string ProgramFile { get { return ProgComments.GetDefault(PROGRAM_FILE_KEY, null); } }

		/// <summary>
		/// Gets the name of the application.
		/// </summary>
		[PublicAPI]
		public static string ApplicationName
		{
			get
			{
				string output;

				if (ProgComments.TryGetValue(APPLICATION_NAME_KEY, out output))
					return output;

				ProgComments.TryGetValue(APPLICATION_NAME_SIMPL_KEY, out output);
				return output;
			}
		}

		#endregion

		/// <summary>
		/// Fakes program info, e.g. "Min Firmware Version      : 1.009.0029"
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		[PublicAPI]
		public static void PrintProgramInfoLine(string name, object value)
		{
			name = (name ?? string.Empty).Trim();

			switch (IcdEnvironment.RuntimeEnvironment)
			{
				case IcdEnvironment.eRuntimeEnvironment.SimplSharp:
					int length = Math.Min(13, name.Length);
					name = name.Substring(0, length).PadRight(13);
					break;

				case IcdEnvironment.eRuntimeEnvironment.SimplSharpPro:
					int proLength = Math.Min(26 - 1, name.Length);
					name = name.Substring(0, proLength).PadRight(26);
					break;

				case IcdEnvironment.eRuntimeEnvironment.Standard:
					name += ' ';
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			IcdConsole.PrintLine("{0}: {1}", name, value);
		}

		/// <summary>
		/// Parses the prog comments and pulls program information.
		/// </summary>
		private static Dictionary<string, string> ParseProgComments()
		{
			Dictionary<string, string> output = new Dictionary<string, string>();

			string progInfo = string.Empty;
			string command = string.Format("progcomments:{0}", ProgramNumber);

			if (!IcdConsole.SendControlSystemCommand(command, ref progInfo))
			{
				ServiceProvider.GetService<ILoggerService>().AddEntry(eSeverity.Warning, "Failed to parse prog comments");
				return output;
			}

			foreach (string line in progInfo.Split(new[] {"\n\r", "\r\n", "\n", "\r"}))
			{
				if (string.IsNullOrEmpty(line))
					continue;

				string[] pair = line.Split(':', 2).ToArray();

				if (pair.Length < 2)
				{
					ServiceProvider.GetService<ILoggerService>()
					               .AddEntry(eSeverity.Warning, "Failed to parse prog comments line - {0}", line);
					continue;
				}

				string key = pair[0].Trim();
				string value = pair[1].Trim();
				output[key] = value;
			}

			return output;
		}
	}
}
