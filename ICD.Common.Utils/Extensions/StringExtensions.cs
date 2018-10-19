using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;

namespace ICD.Common.Utils.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// Returns the first appearance of an item from the given sequence.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="items"></param>
		/// <param name="first"></param>
		/// <returns></returns>
		[PublicAPI]
		public static int IndexOf(this string extends, IEnumerable<string> items, out string first)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (items == null)
				throw new ArgumentNullException("items");

			int index = -1;
			first = null;

			foreach (string item in items)
			{
				int thisIndex = extends.IndexOf(item, StringComparison.Ordinal);
				if (thisIndex == -1)
					continue;

				if (index != -1 && thisIndex >= index)
					continue;

				index = thisIndex;
				first = item;
			}

			return index;
		}

		/// <summary>
		/// Returns true if the string starts with the given character.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="character"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool StartsWith(this string extends, char character)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.Length > 0 && character == extends[0];
		}

		/// <summary>
		/// Returns true if the string ends with the given character.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="character"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool EndsWith(this string extends, char character)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.Length > 0 && character == extends[extends.Length - 1];
		}

		/// <summary>
		/// Splits the string by the given delimiter, returning up to the given number of substrings.
		/// E.g. "a:b:c".Split(':', 2) returns ["a", "b:c"]
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="delimeter"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static IEnumerable<string> Split(this string extends, char delimeter, int count)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (count < 1)
				throw new ArgumentException("Value must be greater or equal to 1", "count");

			return SplitIterator(extends, delimeter, count);
		}

		/// <summary>
		/// Splits the string by the given delimiter, returning up to the given number of substrings.
		/// E.g. "a:b:c".Split(':', 2) returns ["a", "b:c"]
		/// </summary>
		/// <param name="value"></param>
		/// <param name="delimeter"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		private static IEnumerable<string> SplitIterator(string value, char delimeter, int count)
		{
			if (count < 2)
			{
				yield return value;
				yield break;
			}

			int index = value.IndexOf(delimeter);
			if (index < 0)
			{
				yield return value;
				yield break;
			}

			string first = value.Substring(0, index);
			string second = value.Substring(index + 1);
			count--;

			yield return first;
			foreach (string item in second.Split(delimeter, count))
				yield return item;
		}

		/// <summary>
		/// Splits a string into chunks of the given length.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="chunkSize"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<string> Split(this string extends, int chunkSize)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (chunkSize <= 0)
				throw new InvalidOperationException("chunkSize must be greater than 0");

			return Enumerable.Range(0, (int)Math.Ceiling(extends.Length / (double)chunkSize))
			                 .Select(i => extends.Substring(i * chunkSize, Math.Min(chunkSize, extends.Length - (i * chunkSize))));
		}

		/// <summary>
		/// Splits a string by a given substring.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="delimeter"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<string> Split(this string extends, string delimeter)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (delimeter == null)
				throw new ArgumentNullException("delimeter");

			return SplitIterator(extends, delimeter);
		}

		/// <summary>
		/// Splits a string by a given substring.
		/// Taken from
		/// https://social.msdn.microsoft.com/Forums/en-US/914a350f-e0e9-45e0-91a4-6b4b2168e780/string-split-function
		/// </summary>
		/// <param name="value"></param>
		/// <param name="delimeter"></param>
		/// <returns></returns>
		private static IEnumerable<string> SplitIterator(string value, string delimeter)
		{
			int dSum = 0;
			int sSum = 0;
			int length = value.Length;
			int delimiterLength = delimeter.Length;

			if (delimiterLength == 0 || length == 0 || length < delimiterLength)
			{
				yield return value;
				yield break;
			}

			char[] cd = delimeter.ToCharArray();
			char[] cs = value.ToCharArray();

			for (int i = 0; i < delimiterLength; i++)
			{
				dSum += cd[i];
				sSum += cs[i];
			}

			int start = 0;
			for (int i = start; i < length - delimiterLength; i++)
			{
				if (i >= start && dSum == sSum && value.Substring(i, delimiterLength) == delimeter)
				{
					yield return value.Substring(start, i - start);
					start = i + delimiterLength;
				}

				sSum += cs[i + delimiterLength] - cs[i];
			}

			if (dSum == sSum && value.Substring(length - delimiterLength, delimiterLength) == delimeter)
			{
				yield return value.Substring(start, length - delimiterLength - start);
				yield return string.Empty;
			}
			else
				yield return value.Substring(start, length - start);
		}

		/// <summary>
		/// Splits a string by the given substrings.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="delimeters"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<string> Split(this string extends, IEnumerable<string> delimeters)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (delimeters == null)
				throw new ArgumentNullException("delimeters");

			string[] delimitersArray = delimeters as string[] ?? delimeters.ToArray();
			return delimitersArray.Length == 0
				       ? new[] {extends}
				       : extends.Split(delimitersArray.First())
				                .SelectMany(s => s.Split(delimitersArray.Skip(1)));
		}

		/// <summary>
		/// Removes whitespace from the string.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string RemoveWhitespace(this string extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return new string(extends.Where(c => !char.IsWhiteSpace(c)).ToArray());
		}

		/// <summary>
		/// Removes the given characters from the string.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="characters"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string Remove(this string extends, IEnumerable<char> characters)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (characters == null)
				throw new ArgumentNullException("characters");

			return new string(extends.Where(c => !characters.Contains(c)).ToArray());
		}

		/// <summary>
		/// Returns true if the string only contains numbers.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool IsNumeric(this string extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.All(char.IsDigit);
		}

		/// <summary>
		/// Returns true if the string contains the given character.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="character"></param>
		/// <returns></returns>
		public static bool Contains(this string extends, char character)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.Contains(character.ToString());
		}
	}
}
