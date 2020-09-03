using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
		public static int IndexOf([NotNull] this string extends, [NotNull] IEnumerable<string> items, out string first)
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

				if (index == 0)
					break;
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
		public static bool StartsWith([NotNull] this string extends, char character)
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
		public static bool EndsWith([NotNull] this string extends, char character)
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
		[NotNull]
		public static IEnumerable<string> Split([NotNull] this string extends, char delimeter, int count)
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
		[NotNull]
		private static IEnumerable<string> SplitIterator([NotNull] string value, char delimeter, int count)
		{
			while (count > 1)
			{
				int index = value.IndexOf(delimeter);
				if (index < 0)
					break;

				yield return value.Substring(0, index);
				value = value.Substring(index + 1);
				count--;
			}

			yield return value;
		}

		/// <summary>
		/// Splits a string into chunks of the given length.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="chunkSize"></param>
		/// <returns></returns>
		[PublicAPI]
		[NotNull]
		public static IEnumerable<string> Split([NotNull] this string extends, int chunkSize)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (chunkSize <= 0)
				throw new InvalidOperationException("chunkSize must be greater than 0");

			return Enumerable.Range(0, (int)Math.Ceiling(extends.Length / (double)chunkSize))
			                 .Select(i => extends.Substring(i * chunkSize,
			                                                Math.Min(chunkSize, extends.Length - (i * chunkSize))));
		}

		/// <summary>
		/// Removes whitespace from the string.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		[NotNull]
		public static string RemoveWhitespace([NotNull] this string extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return new string(extends.Where(c => !char.IsWhiteSpace(c)).ToArray());
		}

		/// <summary>
		/// Replaces spans of whitespace with a single space.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		[NotNull]
		public static string RemoveDuplicateWhitespace([NotNull] this string extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return Regex.Replace(extends, @"\s+", " ");
		}

		/// <summary>
		/// Removes all occurrences of the given string.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		[NotNull]
		public static string Remove([NotNull] this string extends, [NotNull] string other)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (other == null)
				throw new ArgumentNullException("other");

			if (string.IsNullOrEmpty(other))
				return extends;

			int index;
			while ((index = extends.IndexOf(other, StringComparison.Ordinal)) >= 0)
				extends = extends.Remove(index, other.Length);

			return extends;
		}

		/// <summary>
		/// Removes all occurrences the given characters from the string.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="characters"></param>
		/// <returns></returns>
		[PublicAPI]
		[NotNull]
		public static string Remove([NotNull] this string extends, IEnumerable<char> characters)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (characters == null)
				throw new ArgumentNullException("characters");

			var cSet = characters.ToIcdHashSet();

			return new string(extends.Where(c => !cSet.Contains(c)).ToArray());
		}

		/// <summary>
		/// Returns true if the string only contains numbers.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool IsNumeric([NotNull] this string extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.AnyAndAll(char.IsDigit);
		}

		/// <summary>
		/// Returns true if the string contains the given character.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="character"></param>
		/// <returns></returns>
		public static bool Contains([NotNull] this string extends, char character)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.Contains(character.ToString());
		}

		/// <summary>
		/// Generates a hashcode that is consistent between program executions.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static int GetStableHashCode([NotNull] this string extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			unchecked
			{
				int hash1 = 5381;
				int hash2 = hash1;

				for (int i = 0; i < extends.Length && extends[i] != '\0'; i += 2)
				{
					hash1 = ((hash1 << 5) + hash1) ^ extends[i];
					if (i == extends.Length - 1 || extends[i + 1] == '\0')
						break;
					hash2 = ((hash2 << 5) + hash2) ^ extends[i + 1];
				}

				return hash1 + (hash2 * 1566083941);
			}
		}

		/// <summary>
		/// Strips all of the non-printable characters and control codes from the string.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static string ToPrintableCharacters([NotNull] this string extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			// Strip ANSI escape sequences
			extends = Regex.Replace(extends, AnsiUtils.ANSI_REGEX, string.Empty);

			// Strip control characters
			extends = Regex.Replace(extends, @"\p{C}+", string.Empty);

			return extends;
		}

		/// <summary>
		/// Gets the printable length of the string.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static int GetPrintableLength([NotNull] this string extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.ToPrintableCharacters().Length;
		}

		/// <summary>
		/// Pads the string to the number of printable characters.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string PadRightPrintable([NotNull] this string extends, int length)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			int printableLength = extends.GetPrintableLength();
			int actualLength = extends.Length;
			int delta = actualLength - printableLength;

			return extends.PadRight(length + delta);
		}
	}
}
