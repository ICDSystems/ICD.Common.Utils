using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils
{
	public static class StringUtils
	{
		private const ushort ASCII_READABLE_START = 32;
		private const ushort ASCII_READABLE_END = 126;

		/// <summary>
		/// Returns the number as a string in the format "\\x00"
		/// </summary>
		/// <param name="numeric"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string ToHexLiteral(int numeric)
		{
			return string.Format("\\x{0:X2}", numeric);
		}

		/// <summary>
		/// Returns the character as a string in the format "\\x00"
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string ToHexLiteral(char c)
		{
			return ToHexLiteral(Convert.ToInt32(c));
		}

		/// <summary>
		/// Returns a string as a string in the format "\\x00\\x00..."
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string ToHexLiteral(string input)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			string[] strings = input.Select(c => ToHexLiteral(c)).ToArray();
			return string.Join("", strings);
		}

		/// <summary>
		/// Converts a character in the string format "\\x00" to char.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[PublicAPI]
		public static char FromHexLiteralCharacter(string data)
		{
			if (data == null)
				throw new ArgumentNullException("data");

			if (data.Length != 4)
				throw new ArgumentException("Expecting data in \x00 format of 4 characters", "data");

			string hexValue = data.Substring(2);
			return (char)Convert.ToByte(hexValue, 16);
		}

		/// <summary>
		/// Converts a string in the format "\\x00\\x00..." to hex representation.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string FromHexLiteral(string data)
		{
			if (data == null)
				throw new ArgumentNullException("data");

			return string.Join("", data.Split(4).Select(s => FromHexLiteralCharacter(s).ToString()).ToArray());
		}

		/// <summary>
		/// Converts the char to a human readable character, otherwise returns a hex string
		/// in the format "\x00"
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string ToMixedReadableHexLiteral(char c)
		{
			int numeric = Convert.ToInt32(c);

			if (numeric >= ASCII_READABLE_START && numeric <= ASCII_READABLE_END)
				return c.ToString();

			return ToHexLiteral(c);
		}

		/// <summary>
		/// Converts the input string to a string in the format "\x00\x00" with human
		/// readable characters where possible e.g. "Hello World!x\0D"
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string ToMixedReadableHexLiteral(string input)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			string[] strings = input.Select(c => ToMixedReadableHexLiteral(c)).ToArray();
			return string.Join("", strings);
		}

		/// <summary>
		/// Converts bytes to an ascii string.
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string ToString(IEnumerable<byte> bytes)
		{
			if (bytes == null)
				throw new ArgumentNullException("bytes");

			byte[] cast = bytes as byte[] ?? bytes.ToArray();
			return Encoding.GetEncoding(28591).GetString(cast, 0, cast.Length);
		}

		/// <summary>
		/// Converts bytes to an ascii string.
		/// </summary>
		/// <param name="bytes"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string ToString(IEnumerable<byte> bytes, int length)
		{
			if (bytes == null)
				throw new ArgumentNullException("bytes");

			byte[] cast = bytes as byte[] ?? bytes.ToArray();
			return Encoding.GetEncoding(28591).GetString(cast, 0, length);
		}

		/// <summary>
		/// Converts an ascii string to bytes.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		[PublicAPI]
		public static byte[] ToBytes(string input)
		{
			return Encoding.GetEncoding(28591).GetBytes(input);
		}

		/// <summary>
		/// Attempts to parse the string as an integer.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool TryParse(string value, out int result)
		{
			return TryConvert(Convert.ToInt32, value, out result);
		}

		/// <summary>
		/// Attempts to parse the string as an unsigned integer.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool TryParse(string value, out uint result)
		{
			return TryConvert(Convert.ToUInt32, value, out result);
		}

		/// <summary>
		/// Attempts to parse the string as a float.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool TryParse(string value, out float result)
		{
			return TryConvert(Convert.ToSingle, value, out result);
		}

		/// <summary>
		/// Attempts to parse the string as a bool.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool TryParse(string value, out bool result)
		{
			return TryConvert(Convert.ToBoolean, value, out result);
		}

		/// <summary>
		/// Attempts to parse the string via the given conversion function.
		/// </summary>
		/// <param name="convertFunc"></param>
		/// <param name="value"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		private static bool TryConvert<T>(Func<string, T> convertFunc, string value, out T result)
		{
			if (convertFunc == null)
				throw new ArgumentNullException("convertFunc");

			result = default(T);
			bool retVal = false;

			try
			{
				result = convertFunc(value);
				retVal = true;
			}
			catch (FormatException)
			{
			}
			catch (InvalidCastException)
			{
			}

			return retVal;
		}

		/// <summary>
		/// Returns the object.ToString() with spaces before capital letters.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string NiceName(object obj)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");

			return NiceName(obj.ToString());
		}

		/// <summary>
		/// Inserts spaces before capital letters.
		/// 
		/// http://stackoverflow.com/questions/4488969/split-a-string-by-capital-letters
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static string NiceName(string name)
		{
			Regex regex = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

			return regex.Replace(name, " ");
		}

		/// <summary>
		/// String.Format({0:######}) is unreliable if the number starts with a 0.
		/// This method fills an input pattern #### from right to left with characters
		/// from the number.
		/// </summary>
		/// <param name="phoneFormat"></param>
		/// <param name="number"></param>
		/// <returns></returns>
		public static string SafeNumericFormat(string phoneFormat, string number)
		{
			if (phoneFormat == null)
				throw new ArgumentNullException("phoneFormat");

			if (number == null)
				throw new ArgumentNullException("number");

			phoneFormat = Reverse(phoneFormat);
			number = Reverse(number);

			StringBuilder builder = new StringBuilder();

			int index = 0;
			foreach (char c in phoneFormat)
			{
				if (index >= number.Length)
				{
					if (c == '#')
						break;

					builder.Append(c);
					continue;
				}

				if (c == '#')
				{
					builder.Append(number[index]);
					index++;
				}
				else
					builder.Append(c);
			}

			return Reverse(builder.ToString()).TrimStart();
		}

		/// <summary>
		/// Reverses the string.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string Reverse(string input)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			char[] charArray = input.ToCharArray();
			Array.Reverse(charArray);
			return new string(charArray);
		}

		/// <summary>
		/// Repeats the char the given number of times.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static string Repeat(char input, int count)
		{
			return Repeat(input.ToString(), count);
		}

		/// <summary>
		/// Repeats the string the given number of times.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string Repeat(string input, int count)
		{
			return count == 0 ? string.Empty : new StringBuilder().Insert(0, input, count).ToString();
		}

		/// <summary>
		/// Returns each item.ToString() in the format "[item1, item2, item3...]"
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string ArrayFormat<T>(IEnumerable<T> items)
		{
			if (items == null)
				throw new ArgumentNullException("items");

			return string.Format("[{0}]", string.Join(", ", items.Select(i => i.ToString()).ToArray()));
		}

		/// <summary>
		/// Given a sequence of numbers, generates a human readable list of numeric ranges.
		/// E.g. [1, 2, 3, 5, 6] becomes "[1-3, 5-6]".
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string ArrayRangeFormat(IEnumerable<int> items)
		{
			if (items == null)
				throw new ArgumentNullException("items");

			string[] ranges = MathUtils.GetRanges(items)
			                           .Select(r => r[0] == r[1]
				                                        ? r[0].ToString()
				                                        : string.Format("{0}-{1}", r[0], r[1]))
			                           .ToArray();

			return ArrayFormat(ranges);
		}

		/// <summary>
		/// Given a sequence of numbers, generates a human readable list of numeric ranges.
		/// E.g. [1, 2, 3, 5, 6] becomes "[1-3, 5-6]".
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string ArrayRangeFormat(IEnumerable<ushort> items)
		{
			if (items == null)
				throw new ArgumentNullException("items");

			return ArrayRangeFormat(items.Select(i => (int)i));
		}

		/// <summary>
		/// Returns a pair of numbers in the format [a - b]
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string RangeFormat(object a, object b)
		{
			return string.Format("[{0} - {1}]", a, b);
		}

		/// <summary>
		/// Capitalizes the first character of the string.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string UppercaseFirst(string input)
		{
			if (string.IsNullOrEmpty(input))
				return input;

			return char.ToUpper(input[0]) + input.Substring(1);
		}

		/// <summary>
		/// Formats an IPID to "0xFF"
		/// </summary>
		/// <param name="ipid"></param>
		/// <returns></returns>
		public static string ToIpIdString(byte ipid)
		{
			return string.Format("0x{0:X2}", ipid);
		}

		/// <summary>
		/// Formats "0xFF" to an IPID.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte FromIpIdString(string value)
		{
			value = value.Replace("0x", "");
			return Convert.ToByte(value, 16);
		}

		/// <summary>
		/// Removes all whitespace from the string.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string RemoveWhitespace(string text)
		{
			return text == null ? null : new string(text.Where(c => !Char.IsWhiteSpace(c)).ToArray());
		}

		/// <summary>
		/// Returns the password as a series of *s
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		public static string PasswordFormat(string password)
		{
			return password == null ? null : Repeat('*', password.Length);
		}

		/// <summary>
		/// Pads the given string with quotations for readable type clarity. If the string is null, returns "NULL".
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToRepresentation(string value)
		{
			return value == null ? "NULL" : string.Format("\"{0}\"", value);
		}

		/// <summary>
		/// Returns the items in the format "x, y, and z"
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public static string SerialComma(IEnumerable<string> items)
		{
			if (items == null)
				throw new ArgumentNullException("items");

			string previous = null;
			StringBuilder builder = new StringBuilder();

			foreach (string item in items)
			{
				if (previous != null)
					builder.AppendFormat("{0}, ", previous);
				previous = item;
			}

			if (previous != null)
			{
				if (builder.Length > 0)
					builder.AppendFormat("and {0}", previous);
				else
					builder.Append(previous);
			}

			return builder.ToString();
		}
	}
}
