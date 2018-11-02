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
			var cSet = characters.ToIcdHashSet();

			return new string(extends.Where(c => !cSet.Contains(c)).ToArray());
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
