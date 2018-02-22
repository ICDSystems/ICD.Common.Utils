using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;

namespace ICD.Common.Utils.Extensions
{
	public static class DictionaryExtensions
	{
		/// <summary>
		/// Removes all of the given keys from the dictionary.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="extends"></param>
		/// <param name="keys"></param>
		public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> extends, IEnumerable<TKey> keys)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

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
		public static bool RemoveValue<TKey, TValue>(this IDictionary<TKey, TValue> extends, TValue value)
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
		public static void RemoveAllValues<TKey, TValue>(this IDictionary<TKey, TValue> extends, TValue value)
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
		[PublicAPI]
		public static TValue GetDefault<TKey, TValue>(this IDictionary<TKey, TValue> extends, TKey key)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			// ReSharper disable once CompareNonConstrainedGenericWithNull
			if (key == null)
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
		public static TValue GetDefault<TKey, TValue>(this IDictionary<TKey, TValue> extends, TKey key, TValue defaultValue)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			// ReSharper disable once CompareNonConstrainedGenericWithNull
			if (key == null)
				throw new ArgumentNullException("key");

			return extends.ContainsKey(key) ? extends[key] : defaultValue;
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
		public static TValue GetOrAddDefault<TKey, TValue>(this IDictionary<TKey, TValue> extends, TKey key,
		                                                   TValue defaultValue)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			// ReSharper disable once CompareNonConstrainedGenericWithNull
			if (key == null)
				throw new ArgumentNullException("key");

			extends[key] = extends.GetDefault(key, defaultValue);
			return extends[key];
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
		public static TKey GetKey<TKey, TValue>(this IDictionary<TKey, TValue> extends, TValue value)
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
		public static bool TryGetKey<TKey, TValue>(this IDictionary<TKey, TValue> extends, TValue value, out TKey key)
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
		public static IEnumerable<TKey> GetKeys<TKey, TValue>(this IDictionary<TKey, TValue> extends, TValue value)
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
		[PublicAPI]
		public static void Update<TKey, TValue>(this IDictionary<TKey, TValue> extends, IDictionary<TKey, TValue> other)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (other == null)
				throw new ArgumentNullException("other");

			foreach (KeyValuePair<TKey, TValue> pair in other)
				extends[pair.Key] = pair.Value;
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
		public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> extends, IEnumerable<TValue> items,
		                                          Func<TValue, TKey> getKey)
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
		public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> extends, IEnumerable<TKey> items,
		                                          Func<TKey, TValue> getValue)
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
		public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> extends,
		                                          IEnumerable<KeyValuePair<TKey, TValue>> items)
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
		public static bool DictionaryEqual<TKey, TValue>(this IDictionary<TKey, TValue> extends,
		                                                 IDictionary<TKey, TValue> other)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

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
		public static bool DictionaryEqual<TKey, TValue>(this IDictionary<TKey, TValue> extends,
		                                                 IDictionary<TKey, TValue> other,
		                                                 IEqualityComparer<TValue> valueComparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

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
		public static bool DictionaryEqual<TKey, TValue>(this IDictionary<TKey, TValue> extends,
		                                                 IDictionary<TKey, TValue> other,
		                                                 Func<TValue, TValue, bool> valueComparer)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (valueComparer == null)
				throw new ArgumentNullException("valueComparer");

			if (extends == other)
				return true;
			if (other == null)
				return false;
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
			this IEnumerable<KeyValuePair<TKey, TValue>> extends)
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
		public static IEnumerable<TValue> OrderValuesByKey<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> extends)
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
		public static Dictionary<TValue, TKey> ToInverse<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
		}
	}
}
