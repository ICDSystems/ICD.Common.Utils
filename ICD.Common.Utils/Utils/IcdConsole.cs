using System;
#if SIMPLSHARP
using Crestron.SimplSharp;
#endif
using ICD.Common.Properties;

namespace ICD.Common.Utils
{
	public sealed class IcdConsole
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
			message = string.Format(message, args);

#if SIMPLSHARP
			try
			{
				CrestronConsole.ConsoleCommandResponse(message);
			}
			catch (NotSupportedException)
			{
				CrestronConsole.Print(message);
			}
#else
			System.Console.Write(message, args);
#endif
		}

		public static void PrintLine(string message)
		{
#if SIMPLSHARP
			CrestronConsole.PrintLine(message);
#else
			System.Console.WriteLine(message);
#endif
		}

		public static void PrintLine(string message, params object[] args)
		{
#if SIMPLSHARP
			CrestronConsole.PrintLine(message, args);
#else
			System.Console.WriteLine(message, args);
#endif
		}

		public static void Print(string message)
		{
#if SIMPLSHARP
			CrestronConsole.Print(message);
#else
            System.Console.Write(message);
#endif
		}

		public static void Print(string message, params object[] args)
		{
#if SIMPLSHARP
			CrestronConsole.Print(message, args);
#else
            System.Console.Write(message, args);
#endif
		}

		public static bool SendControlSystemCommand(string command, ref string result)
		{
#if SIMPLSHARP
			return CrestronConsole.SendControlSystemCommand(command, ref result);
#else
            result = string.Empty;
			return false;
#endif
		}

		public static bool AddNewConsoleCommand(Action<string> callback, string command, string help, eAccessLevel accessLevel)
		{
#if SIMPLSHARP
			// Avoid crashing Simpl applications
			if (IcdEnvironment.RuntimeEnvironment == IcdEnvironment.eRuntimeEnvironment.SimplSharp)
				return false;

			if (CrestronConsole.ConsoleRegistered)
				return false;

			CrestronConsole.AddNewConsoleCommand(str => callback(str), command, help, (ConsoleAccessLevelEnum)(int)accessLevel);
#endif
			return true;
		}
	}
}