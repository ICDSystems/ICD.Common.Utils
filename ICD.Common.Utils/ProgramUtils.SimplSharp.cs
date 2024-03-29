﻿using System.Globalization;
using ICD.Common.Utils.IO;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
#if !NETSTANDARD
using Crestron.SimplSharp;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICD.Common.Utils
{
	public static partial class ProgramUtils
	{
		private const string APPLICATION_NAME_KEY = "Application Name";
		private const string APPLICATION_NAME_SIMPL_KEY = "System Name";
		private const string PROGRAM_FILE_KEY = "Program File";

		private const string COMPILED_ON_KEY = "Compiled On";
		private const string COMPILER_REVISION_KEY = "Compiler Revision";
		private const string COMPILER_REVISION_SIMPL_KEY = "Compiler Rev";

		private static Dictionary<string, string> s_ProgComments;

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
		public static uint ProgramNumber { get { return InitialParametersClass.ApplicationNumber; } }

		/// <summary>
		/// Gets the compile date of the program.
		/// </summary>
		[PublicAPI]
		public static DateTime CompiledDate
		{
			get
			{
				string dateString;
				if (!ProgComments.TryGetValue(COMPILED_ON_KEY, out dateString))
					return DateTime.MinValue;

				// Crestron writes compile time in system local time with no sense of localization...
				try
				{
					return DateTime.Parse(dateString).ToUniversalTime();
				}
				catch (FormatException)
				{
				}

				// Try again with dd/mm/yyyy
				try
				{
					return DateTime.ParseExact(dateString, "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture).ToUniversalTime();
				}
				catch (FormatException)
				{
				}

				return DateTime.MinValue;
			}
		}

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

		/// <summary>
		/// Returns the date and time the program was installed.
		/// </summary>
		/// <returns></returns>
		[PublicAPI]
		public static DateTime ProgramInstallDate
		{
			get { return IcdFile.GetCreationTime(PathUtils.Join(PathUtils.ProgramPath, ProgramFile)).ToUniversalTime(); }
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

			foreach (string line in progInfo.Split(new[] {'\r', '\n'}))
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

#endif
