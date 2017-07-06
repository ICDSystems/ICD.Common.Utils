﻿using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils
{
	/// <summary>
	/// Utility methods for math operations.
	/// </summary>
	public static class MathUtils
	{
		/// <summary>
		/// Clamps the number between the two values.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static int Clamp(int number, int min, int max)
		{
			return (int)Clamp((double)number, min, max);
		}

		/// <summary>
		/// Clamps the number between the two values.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static ushort Clamp(ushort number, ushort min, ushort max)
		{
			return (ushort)Clamp((double)number, min, max);
		}

		/// <summary>
		/// Clamps the number between the two values.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static float Clamp(float number, float min, float max)
		{
			return (float)Clamp((double)number, min, max);
		}

		/// <summary>
		/// Clamps the number between the two values.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static double Clamp(double number, double min, double max)
		{
			return min < max
				       ? Math.Min(Math.Max(number, min), max)
				       : Math.Min(Math.Max(number, max), min);
		}

		/// <summary>
		/// Returns the value after the input range has been mapped to a new range
		/// </summary>
		/// <param name="inputStart">Input start.</param>
		/// <param name="inputEnd">Input end.</param>
		/// <param name="outputStart">Output start.</param>
		/// <param name="outputEnd">Output end.</param>
		/// <param name="value">Value.</param>
		/// <returns>The newly mapped value</returns>
		public static double MapRange(double inputStart, double inputEnd, double outputStart, double outputEnd, double value)
		{
			double slope = (outputEnd - outputStart) / (inputEnd - inputStart);
			return outputStart + slope * (value - inputStart);
		}

		/// <summary>
		/// Returns the value after the input range has been mapped to a new range
		/// </summary>
		/// <param name="inputStart">Input start.</param>
		/// <param name="inputEnd">Input end.</param>
		/// <param name="outputStart">Output start.</param>
		/// <param name="outputEnd">Output end.</param>
		/// <param name="value">Value.</param>
		/// <returns>The newly mapped value</returns>
		public static float MapRange(float inputStart, float inputEnd, float outputStart, float outputEnd, float value)
		{
			return (float)MapRange((double)inputStart, inputEnd, outputStart, outputEnd, value);
		}

		/// <summary>
		/// Returns the value after the input range has been mapped to a new range
		/// </summary>
		/// <param name="inputStart">Input start.</param>
		/// <param name="inputEnd">Input end.</param>
		/// <param name="outputStart">Output start.</param>
		/// <param name="outputEnd">Output end.</param>
		/// <param name="value">Value.</param>
		/// <returns>The newly mapped value</returns>
		public static int MapRange(int inputStart, int inputEnd, int outputStart, int outputEnd, int value)
		{
			return (int)MapRange((double)inputStart, inputEnd, outputStart, outputEnd, value);
		}

		/// <summary>
		/// Returns the value after the input range has been mapped to a new range
		/// </summary>
		/// <param name="inputStart">Input start.</param>
		/// <param name="inputEnd">Input end.</param>
		/// <param name="outputStart">Output start.</param>
		/// <param name="outputEnd">Output end.</param>
		/// <param name="value">Value.</param>
		/// <returns>The newly mapped value</returns>
		public static ushort MapRange(ushort inputStart, ushort inputEnd, ushort outputStart, ushort outputEnd, ushort value)
		{
			return (ushort)MapRange((double)inputStart, inputEnd, outputStart, outputEnd, value);
		}

		/// <summary>
		/// Maps the date in the given range to the float range 0.0f to 1.0f.
		/// 0.5f - The date is half way between the end points.
		/// less than 0.0f - the date is before the start.
		/// greater than 1.0f - the date is after the end.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static double MapRange(DateTime start, DateTime end, DateTime value)
		{
			return MapRange(new TimeSpan(start.Ticks).TotalSeconds,
			                new TimeSpan(end.Ticks).TotalSeconds,
			                0.0f, 1.0f, new TimeSpan(value.Ticks).TotalSeconds);
		}

		/// <summary>
		/// Gets the digit count for the given number.
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		public static int GetNumberOfDigits(int number)
		{
			int output = number.ToString().Length;
			if (number < 0)
				output--;

			return output;
		}

		/// <summary>
		/// Gets the digit count for the given number.
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		public static int GetNumberOfDigits(uint number)
		{
			return number.ToString().Length;
		}

		/// <summary>
		/// Takes a sequence of numbers:
		///		1, 3, 5, 6, 7, 8, 9, 10, 12
		/// And calculates the continuous ranges:
		///		(1, 1), (3, 3), (5, 10), (12, 12)
		/// </summary>
		public static IEnumerable<int[]> GetRanges(IEnumerable<int> numbers)
		{
			if (numbers == null)
				throw new ArgumentNullException("numbers");

			int[] currentRange = null;

			foreach (int number in numbers.Order())
			{
				if (currentRange == null)
					currentRange = new[] {number, number};
				else if (currentRange[1] == number - 1)
					currentRange = new[] {currentRange[0], number};
				else
				{
					yield return currentRange;
					currentRange = new[] {number, number};
				}
			}

			if (currentRange != null)
				yield return currentRange;
		}

		/// <summary>
		/// Rounds the given number to the nearest item in the given sequence.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="nearest"></param>
		/// <returns></returns>
		public static int RoundToNearest(int number, IEnumerable<int> nearest)
		{
			if (nearest == null)
				throw new ArgumentNullException("nearest");

			return nearest.Aggregate((x, y) => Math.Abs(x - number) < Math.Abs(y - number) ? x : y);
		}

		/// <summary>
		/// Gets a new, unique id given a sequence of existing ids.
		/// </summary>
		/// <param name="existingIds"></param>
		/// <returns></returns>
		public static int GetNewId(IEnumerable<int> existingIds)
		{
			if (existingIds == null)
				throw new ArgumentNullException("existingIds");

			IcdHashSet<int> existing = existingIds.ToHashSet();
			return Enumerable.Range(1, int.MaxValue).First(i => !existing.Contains(i));
		}
	}
}
