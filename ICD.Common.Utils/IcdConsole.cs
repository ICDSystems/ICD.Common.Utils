using System;
using System.Linq;
using ICD.Common.Properties;
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
			try
			{
				CrestronConsole.ConsoleCommandResponse(message);
			}
			catch (NotSupportedException)
			{
				Print(message);
			}
#else
			Print(message);
#endif
		}

		public static void PrintLine(string message)
		{
#if SIMPLSHARP
			CrestronConsole.PrintLine(message);
#else
			Console.WriteLine(message);
#endif
		}

		public static void PrintLine(string message, params object[] args)
		{
			message = string.Format(message, args);
			PrintLine(message);
		}

		public static void PrintLine(eConsoleColor color, string message)
		{
#if SIMPLSHARP
			PrintLine(color.FormatAnsi(message));
#else
			System.Console.ForegroundColor = color.ToForegroundConsoleColor();
			System.Console.BackgroundColor = color.ToBackgroundConsoleColor();
			System.Console.WriteLine(message);
			System.Console.ResetColor();
#endif
		}

		public static void PrintLine(eConsoleColor color, string message, params object[] args)
		{
			message = string.Format(message, args);
			PrintLine(color, message);
		}

		public static void Print(string message)
		{
#if SIMPLSHARP
			CrestronConsole.Print(message);
#else
            Console.Write(message);
#endif
		}

		public static void Print(string message, params object[] args)
		{
			message = string.Format(message, args);
			Print(message);
		}

		public static void Print(eConsoleColor color, string message)
		{
#if SIMPLSHARP
			Print(color.FormatAnsi(message));
#else
			System.Console.ForegroundColor = color.ToForegroundConsoleColor();
			System.Console.BackgroundColor = color.ToBackgroundConsoleColor();
			System.Console.Write(message);
			System.Console.ResetColor();
#endif
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
			if (IcdEnvironment.RuntimeEnvironment == IcdEnvironment.eRuntimeEnvironment.SimplSharpProMono)
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
			if (IcdEnvironment.RuntimeEnvironment != IcdEnvironment.eRuntimeEnvironment.SimplSharpPro)
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
