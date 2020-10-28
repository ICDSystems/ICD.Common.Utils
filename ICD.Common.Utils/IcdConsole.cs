using System;
using System.Linq;
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
		public enum eAccessLevel
		{
			Operator = 0,
			Programmer = 1,
			Administrator = 2
		}

		public static event EventHandler<StringEventArgs> OnConsolePrint;

		private static readonly SafeCriticalSection s_Section;

		/// <summary>
		/// Static constructor.
		/// </summary>
		static IcdConsole()
		{
			s_Section = new SafeCriticalSection();
		}

		/// <summary>
		/// Wraps CrestronConsole.ConsoleCommandResponse for S+ compatibility.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		[PublicAPI]
		public static void ConsoleCommandResponseLine(string message, params object[] args)
		{
			ConsoleCommandResponse(message + IcdEnvironment.NewLine, args);
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

#if SIMPLSHARP
			if (IcdEnvironment.RuntimeEnvironment == IcdEnvironment.eRuntimeEnvironment.SimplSharpPro)
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

			try
			{
#if SIMPLSHARP
				if (IcdEnvironment.RuntimeEnvironment != IcdEnvironment.eRuntimeEnvironment.SimplSharpProServer)
					CrestronConsole.PrintLine(message);
#else
				Console.WriteLine(message);
#endif
			}
			finally
			{
				s_Section.Leave();
			}

			OnConsolePrint.Raise(null, new StringEventArgs(message + IcdEnvironment.NewLine));
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

			try
			{
#if SIMPLSHARP
				if (IcdEnvironment.RuntimeEnvironment != IcdEnvironment.eRuntimeEnvironment.SimplSharpProServer)
					CrestronConsole.Print(message);
#else
				Console.Write(message);
#endif
			}
			finally
			{
				s_Section.Leave();
			}

			OnConsolePrint.Raise(null, new StringEventArgs(message));
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
			if (IcdEnvironment.RuntimeEnvironment == IcdEnvironment.eRuntimeEnvironment.SimplSharpProServer)
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
			if (IcdEnvironment.RuntimeEnvironment != IcdEnvironment.eRuntimeEnvironment.SimplSharpPro &&
				IcdEnvironment.RuntimeEnvironment != IcdEnvironment.eRuntimeEnvironment.SimplSharpProMono)
				return false;

			if (CrestronConsole.ConsoleRegistered)
				return false;

			CrestronConsole.AddNewConsoleCommand(str => callback(str), command, help, (ConsoleAccessLevelEnum)(int)accessLevel);
			return true;
#else
			return false;
#endif
		}
	}
}
