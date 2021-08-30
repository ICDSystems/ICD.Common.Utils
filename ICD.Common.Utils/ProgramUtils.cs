using System;
using ICD.Common.Properties;

namespace ICD.Common.Utils
{
	public static partial class ProgramUtils
	{
		/// <summary>
		/// Gets the program number in the format XX, eg slot 1 is 01.
		/// </summary>
		[PublicAPI]
		public static string ProgramNumberFormatted { get { return string.Format("{0:D2}", ProgramNumber); } }

		/// <summary>
		/// Fakes program info, e.g. "Min Firmware Version      : 1.009.0029"
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		[PublicAPI]
		public static void PrintProgramInfoLine(string name, object value)
		{
			name = (name ?? string.Empty).Trim();

			switch (IcdEnvironment.Framework)
			{
				case IcdEnvironment.eFramework.Crestron:
					switch (IcdEnvironment.CrestronRuntimeEnvironment)
					{
						case IcdEnvironment.eCrestronRuntimeEnvironment.Simpl:
							int length = Math.Min(13, name.Length);
							name = name.Substring(0, length).PadRight(13);
							break;
						case IcdEnvironment.eCrestronRuntimeEnvironment.Appliance:
							int proLength = Math.Min(26 - 1, name.Length);
							name = name.Substring(0, proLength).PadRight(26);
							break;
						case IcdEnvironment.eCrestronRuntimeEnvironment.Server:
							// No console
							return;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case IcdEnvironment.eFramework.Framework:
				case IcdEnvironment.eFramework.Standard:
					name += ' ';
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			IcdConsole.PrintLine("{0}: {1}", name, value);
		}
	}
}
