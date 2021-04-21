using System;
using System.Linq;
using System.Text.RegularExpressions;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
#if SIMPLSHARP
using Crestron.SimplSharp;
#else
using System.Diagnostics;
#endif

namespace ICD.Common.Utils
{
	public static class IcdConsole
	{
		private const string NEWLINE = "\r\n";
		public enum eAccessLevel
		{
			Operator = 0,
			Programmer = 1,
			Administrator = 2
		}

		public static event EventHandler<StringEventArgs> OnConsolePrint;

		private static readonly SafeCriticalSection s_Section;

		private static readonly Regex s_NewLineRegex;

		/// <summary>
		/// Static constructor.
		/// </summary>
		static IcdConsole()
		{
			s_Section = new SafeCriticalSection();
			s_NewLineRegex = new Regex("(?!<\r)\n");
		}

		/// <summary>
		/// Wraps CrestronConsole.ConsoleCommandResponse for S+ compatibility.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		[PublicAPI]
		public static void ConsoleCommandResponseLine(string message, params object[] args)
		{
			ConsoleCommandResponse(message + NEWLINE, args);
		}

		/// <summary>
		/// Wraps CrestronConsole.ConsoleCommandResponse for S+ compatibility.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		[PublicAPI]
		public static void ConsoleCommandResponse(string message, params object[] args)
		{
			if (args != null && args.Any())
				message = string.Format(message, args);

			message = FixLineEndings(message);

#if SIMPLSHARP
			if (IcdEnvironment.CrestronRuntimeEnvironment == IcdEnvironment.eCrestronRuntimeEnvironment.Appliance)
			{
				try
				{
					CrestronConsole.ConsoleCommandResponse(message);
				}
				catch (NotSupportedException)
				{
					PrintLine(message);
				}
				return;
			}
#endif

			PrintLine(message);
		}

		public static void PrintLine(string message)
		{
			s_Section.Enter();

			string fixedMessage = FixLineEndings(message);

			try
			{
#if SIMPLSHARP
				if (IcdEnvironment.CrestronRuntimeEnvironment != IcdEnvironment.eCrestronRuntimeEnvironment.Server)
					CrestronConsole.PrintLine(fixedMessage);
#else
				Trace.WriteLine(AnsiUtils.StripAnsi(fixedMessage));
				Console.WriteLine(fixedMessage);
#endif
			}
			finally
			{
				s_Section.Leave();
			}

			OnConsolePrint.Raise(null, new StringEventArgs(message + NEWLINE));
		}

		public static void PrintLine(string message, params object[] args)
		{
			message = string.Format(message, args);
			PrintLine(message);
		}

		public static void PrintLine(eConsoleColor color, string message)
		{
			string ansi = color.FormatAnsi(message);
			PrintLine(ansi);
		}

		public static void PrintLine(eConsoleColor color, string message, params object[] args)
		{
			message = string.Format(message, args);
			PrintLine(color, message);
		}

		public static void Print(string message)
		{
			s_Section.Enter();

			string fixedMessage = FixLineEndings(message);

			try
			{
#if SIMPLSHARP
				if (IcdEnvironment.CrestronRuntimeEnvironment != IcdEnvironment.eCrestronRuntimeEnvironment.Server)
					CrestronConsole.Print(fixedMessage);
#else
				Trace.Write(AnsiUtils.StripAnsi(fixedMessage));
				Console.Write(fixedMessage);
#endif
			}
			finally
			{
				s_Section.Leave();
			}

			OnConsolePrint.Raise(null, new StringEventArgs(fixedMessage));
		}

		public static void Print(string message, params object[] args)
		{
			message = string.Format(message, args);
			Print(message);
		}

		public static void Print(eConsoleColor color, string message)
		{
			string ansi = color.FormatAnsi(message);
			Print(ansi);
		}

		public static void Print(eConsoleColor color, string message, params object[] args)
		{
			message = string.Format(message, args);
			Print(color, message);
		}

		public static bool SendControlSystemCommand(string command, ref string result)
		{
#if SIMPLSHARP
			// No console on VC4
			if (IcdEnvironment.CrestronRuntimeEnvironment == IcdEnvironment.eCrestronRuntimeEnvironment.Server)
				return false;

			return CrestronConsole.SendControlSystemCommand(command, ref result);
#else
			return false;
#endif
		}

		public static bool AddNewConsoleCommand(Action<string> callback, string command, string help, eAccessLevel accessLevel)
		{
#if SIMPLSHARP
			// Avoid crashing Simpl applications
			if (IcdEnvironment.CrestronRuntimeEnvironment != IcdEnvironment.eCrestronRuntimeEnvironment.Appliance)
				return false;

			if (CrestronConsole.ConsoleRegistered)
				return false;

			CrestronConsole.AddNewConsoleCommand(str => callback(str), command, help, (ConsoleAccessLevelEnum)(int)accessLevel);
			return true;
#else
			return false;
#endif
		}

		/// <summary>
		/// Code running on SimplSharpProMono uses \n for newline (due to linux environment),
		/// Which causes console output to be unreadable on most SSH clients.  This converts those
		/// endings to \r\n, since Crestron's SSH server doesn't do it automatically.
		/// This is a hack until Crestron fixes their SSH server.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		private static string FixLineEndings(string input)
		{
			if (IcdEnvironment.CrestronSeries != IcdEnvironment.eCrestronSeries.FourSeries)
				return input;

			return s_NewLineRegex.Replace(input, NEWLINE);
		}
	}
}
