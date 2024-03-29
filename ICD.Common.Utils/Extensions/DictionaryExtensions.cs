﻿using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;

namespace ICD.Common.Utils.Extensions
{
	public static class DictionaryExtensions
	{
		/// <summary>
		/// Removes the key from the dictionary, outputting the value.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool Remove<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> extends,
		                                        [NotNull] TKey key, out TValue value)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

// ReSharper disable CompareNonConstrainedGenericWithNull
			if (key == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("key");

			return extends.TryGetValue(key, out value) && extends.Remove(key);
		}

		/// <summary>
		/// Removes all of the given keys from the dictionary.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="keys"></param>
		public static void RemoveAll<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> extends,
		                                           [NotNull] IEnumerable<TKey> keys)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (keys == null)
				throw new ArgumentNullException("keys");

			foreach (TKey key in keys)
				extends.Remove(key);
		}

		/// <summary>
		/// Removes the first key with a value matching the given value.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="value"></param>
		/// <returns>False if value is not found in the dictionary.</returns>
		[PublicAPI]
		public static bool RemoveValue<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> extends, TValue value)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			TKey key;
			return extends.TryGetKey(value, out key) && extends.Remove(key);
		}

		/// <summary>
		/// Removes all keys with the given value.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="value"></param>
		[PublicAPI]
		public static void RemoveAllValues<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> extends, TValue value)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			foreach (TKey key in extends.GetKeys(value).ToArray())
				extends.Remove(key);
		}

		/// <summary>
		/// If the key is present in the dictionary return the value, otherwise returns default value.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		[CanBeNull]
		[PublicAPI]
		public static TValue GetDefault<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> extends,
		                                              [NotNull] TKey key)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

// ReSharper disable CompareNonConstrainedGenericWithNull
			if (key == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("key");

			return extends.GetDefault(key, default(TValue));
		}

		/// <summary>
		/// If the key is present in the dictionary return the value, otherwise return the default value.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		[PublicAPI]
		public static TValue GetDefault<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> extends,
		                                              [NotNull] TKey key,
		                                              TValue defaultValue)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

// ReSharper disable CompareNonConstrainedGenericWithNull
			if (key == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("key");

			TValue value;
			return extends.TryGetValue(key, out value) ? value : defaultValue;
		}

		/// <summary>
		/// If the key is present in the dictionary return the value, otherwise add the default value to the dictionary and return it.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		[PublicAPI]
		public static TValue GetOrAddDefault<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> extends,
		                                                   [NotNull] TKey key, TValue defaultValue)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

// ReSharper disable CompareNonConstrainedGenericWithNull
			if (key == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("key");

			TValue value = extends.GetDefault(key, defaultValue);
			extends[key] = value;

			return value;
		}

		/// <summary>
		/// If the key is present in the dictionary return the value, otherwise add a new value to the dictionary and return it.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		[PublicAPI]
		public static TValue GetOrAddNew<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> extends,
													   [NotNull] TKey key)
			where TValue : new()
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			// ReSharper disable CompareNonConstrainedGenericWithNull
			if (key == null)
				// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("key");

			return extends.GetOrAddNew(key, () => ReflectionUtils.CreateInstance<TValue>());
		}

		/// <summary>
		/// If the key is present in the dictionary return the value, otherwise add a new value to the dictionary and return it.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="key"></param>
		/// <param name="valueFunc"></param>
		/// <returns></returns>
		[PublicAPI]
		public static TValue GetOrAddNew<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> extends,
													   [NotNull] TKey key,
													   [NotNull] Func<TValue> valueFunc)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			// ReSharper disable CompareNonConstrainedGenericWithNull
			if (key == null)
				// ReSharper restore CompareNonConstrainedGenericWithNull
				throw new ArgumentNullException("key");

			if (valueFunc == null)
				throw new ArgumentNullException("valueFunc");

			TValue value;
			if (!extends.TryGetValue(key, out value))
			{
				value = valueFunc();
				extends.Add(key, value);
			}

			return value;
		}

		/// <summary>
		/// Gets a key for the given value.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException">The value does not exist in the dictionary.</exception>
		[PublicAPI]
		public static TKey GetKey<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> extends, TValue value)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			TKey output;
			if (extends.TryGetKey(value, out output))
				return output;

			string message = string.Format("Unable to find Key with Value matching {0}", value);
			throw new KeyNotFoundException(message);
		}

		/// <summary>
		/// Attempts to get the first key with the given value.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="value"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool TryGetKey<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> extends, TValue value,
		                                           out TKey key)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.GetKeys(value).TryElementAt(0, out key);
		}

		/// <summary>
		/// Gets the keys that match the given value.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<TKey> GetKeys<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> extends,
		                                                      TValue value)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.Where(kvp => EqualityComparer<TValue>.Default.Equals(kvp.Value, value))
			              .Select(kvp => kvp.Key);
		}

		/// <summary>
		/// Updates the dictionary with items from the other dictionary.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool Update<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> extends,
		                                        [NotNull] IEnumerable<KeyValuePair<TKey, TValue>> other)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (other == null)
				throw new ArgumentNullException("other");

			return extends.Update(other, EqualityComparer<TValue>.Default);
		}

		/// <summary>
		/// Updates the dictionary with items from the other dictionary.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="other"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool Update<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> extends,
		                                        [NotNull] IEnumerable<KeyValuePair<TKey, TValue>> other,
		                                        [NotNull] IEqualityComparer<TValue> comparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (other == null)
				throw new ArgumentNullException("other");

			if (comparer == null)
				throw new ArgumentNullException("comparer");

			bool change = false;

			foreach (KeyValuePair<TKey, TValue> pair in other)
			{
				TValue value;
				if (extends.TryGetValue(pair.Key, out value) && comparer.Equals(pair.Value, value))
					continue;

				extends[pair.Key] = pair.Value;
				change = true;
			}

			return change;
		}

		/// <summary>
		/// Adds the sequence of items to the dictionary.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="items"></param>
		[PublicAPI]
		public static void AddRange<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> extends,
		                                          [NotNull] IEnumerable<KeyValuePair<TKey, TValue>> items)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (items == null)
				throw new ArgumentNullException("items");

			foreach (KeyValuePair<TKey, TValue> item in items)
				extends.Add(item);
		}

		/// <summary>
		/// Adds the sequence of items to the dictionary.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="items"></param>
		/// <param name="getKey"></param>
		[PublicAPI]
		public static void AddRange<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> extends,
		                                          [NotNull] IEnumerable<TValue> items,
		                                          [NotNull] Func<TValue, TKey> getKey)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (items == null)
				throw new ArgumentNullException("items");

			if (getKey == null)
				throw new ArgumentNullException("getKey");

			foreach (TValue item in items)
				extends.Add(getKey(item), item);
		}

		/// <summary>
		/// Adds the sequence of items to the dictionary.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="items"></param>
		/// <param name="getValue"></param>
		[PublicAPI]
		public static void AddRange<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> extends,
		                                          [NotNull] IEnumerable<TKey> items,
		                                          [NotNull] Func<TKey, TValue> getValue)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (items == null)
				throw new ArgumentNullException("items");

			if (getValue == null)
				throw new ArgumentNullException("getValue");

			foreach (TKey item in items)
				extends.Add(item, getValue(item));
		}

		/// <summary>
		/// Adds the sequence of items to the dictionary.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="items"></param>
		[PublicAPI]
		public static void AddRange<TKey, TValue>([NotNull] this Dictionary<TKey, TValue> extends,
		                                          [NotNull] IEnumerable<KeyValuePair<TKey, TValue>> items)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (items == null)
				throw new ArgumentNullException("items");

			foreach (KeyValuePair<TKey, TValue> item in items)
				extends.Add(item.Key, item.Value);
		}

		/// <summary>
		/// Compares the keys and values of the dictionary to determine equality.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool DictionaryEqual<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> extends,
		                                                 [NotNull] IDictionary<TKey, TValue> other)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (other == null)
				throw new ArgumentNullException("other");

			return extends.DictionaryEqual(other, EqualityComparer<TValue>.Default);
		}

		/// <summary>
		/// Compares the keys and values of the dictionary to determine equality.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="other"></param>
		/// <param name="valueComparer"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool DictionaryEqual<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> extends,
		                                                 [NotNull] IDictionary<TKey, TValue> other,
		                                                 [NotNull] IEqualityComparer<TValue> valueComparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (other == null)
				throw new ArgumentNullException("other");

			if (valueComparer == null)
				throw new ArgumentNullException("valueComparer");

			return extends.DictionaryEqual(other, valueComparer.Equals);
		}

		/// <summary>
		/// Compares the keys and values of the dictionary to determine equality.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="other"></param>
		/// <param name="valueComparer"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool DictionaryEqual<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> extends,
		                                                 [NotNull] IDictionary<TKey, TValue> other,
		                                                 [NotNull] Func<TValue, TValue, bool> valueComparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (other == null)
				throw new ArgumentNullException("other");

			if (valueComparer == null)
				throw new ArgumentNullException("valueComparer");

			if (extends == other)
				return true;
			if (extends.Count != other.Count)
				return false;

			foreach (KeyValuePair<TKey, TValue> kvp in extends)
			{
				TValue secondValue;
				if (!other.TryGetValue(kvp.Key, out secondValue))
					return false;
				if (!valueComparer(kvp.Value, secondValue))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Returns the KeyValuePairs in key order.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<KeyValuePair<TKey, TValue>> OrderByKey<TKey, TValue>(
			[NotNull] this IEnumerable<KeyValuePair<TKey, TValue>> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.OrderBy(kvp => kvp.Key);
		}

		/// <summary>
		/// Returns a sequence of values ordered by the dictionary keys.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<TValue> OrderValuesByKey<TKey, TValue>(
			[NotNull] this IEnumerable<KeyValuePair<TKey, TValue>> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.OrderByKey().Select(kvp => kvp.Value);
		}

		/// <summary>
		/// Returns an inverse mapping of TValue -> TKey.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static Dictionary<TValue, List<TKey>> ToInverse<TKey, TValue>(
			[NotNull] this IEnumerable<KeyValuePair<TKey, TValue>> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			Dictionary<TValue, List<TKey>> output = new Dictionary<TValue, List<TKey>>();

			foreach (KeyValuePair<TKey, TValue> kvp in extends)
			{
				List<TKey> keys;
				if (!output.TryGetValue(kvp.Value, out keys))
				{
					keys = new List<TKey>();
					output.Add(kvp.Value, keys);
				}

				keys.Add(kvp.Key);
			}

			return output;
		}

		/// <summary>
		/// Turns an enumerable of KeyValuePairs back into a dictionary
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
			[NotNull] this IEnumerable<KeyValuePair<TKey, TValue>> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.ToDictionary(x => x.Key, x => x.Value);
		}
	}
}
