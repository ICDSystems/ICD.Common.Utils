using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.IO;

namespace ICD.Common.Utils.Xml
{
	/// <summary>
	/// XmlUtils provides utility methods for working with XML.
	/// </summary>
	public static class XmlUtils
	{
		/// <summary>
		/// Returns the contents of the outermost element as a string.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string GetInnerXml(string xml)
		{
			using (IcdXmlReader reader = new IcdXmlReader(xml))
			{
				reader.SkipToNextElement();
				return reader.ReadInnerXml();
			}
		}

		#region Attributes

		/// <summary>
		/// Returns true if the attribute exists.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool HasAttribute(string xml, string name)
		{
			using (IcdXmlReader reader = new IcdXmlReader(xml))
				return reader.HasAttribute(name);
		}

		/// <summary>
		/// Gets the attributes for the current xml element.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<IcdXmlAttribute> GetAttributes(string xml)
		{
			using (IcdXmlReader reader = new IcdXmlReader(xml))
			{
				reader.SkipToNextElement();
				return reader.GetAttributes();
			}
		}

		/// <summary>
		/// Convenience method for getting attribute by name.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IcdXmlAttribute GetAttribute(string xml, string name)
		{
			IcdXmlAttribute output;
			if (GetAttributes(xml).TryFirst(a => a.Name == name, out output))
				return output;

			throw new KeyNotFoundException(string.Format("No attribute with name {0}", name));
		}

		/// <summary>
		///	Gets the value of the attribute with the given name.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string GetAttributeAsString(string xml, string name)
		{
			using (IcdXmlReader reader = new IcdXmlReader(xml))
			{
				reader.SkipToNextElement();
				return reader.GetAttributeAsString(name);
			}
		}

		/// <summary>
		///	Gets the value of the attribute with the given name and returns as an integer.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		[PublicAPI]
		public static int GetAttributeAsInt(string xml, string name)
		{
			using (IcdXmlReader reader = new IcdXmlReader(xml))
			{
				reader.SkipToNextElement();
				return reader.GetAttributeAsInt(name);
			}
		}

		/// <summary>
		/// Gets the value of the attribute with the given name and returns as a bool.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool GetAttributeAsBool(string xml, string name)
		{
			using (IcdXmlReader reader = new IcdXmlReader(xml))
			{
				reader.SkipToNextElement();
				return reader.GetAttributeAsBool(name);
			}
		}

		#endregion

		#region Recurse

		/// <summary>
		/// Recurses through the entire XML, calling the callback for each Element/Text node.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="callback"></param>
		[PublicAPI]
		public static void Recurse(string xml, Action<XmlRecursionEventArgs> callback)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");

			Recurse(xml, new Stack<string>(), callback);
		}

		/// <summary>
		/// Recurses through the entire XML, calling the callback for each Element/Text node.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="path"></param>
		/// <param name="callback"></param>
		private static void Recurse(string xml, Stack<string> path, Action<XmlRecursionEventArgs> callback)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			if (callback == null)
				throw new ArgumentNullException("callback");

			IcdXmlReader childReader;

			try
			{
				childReader = new IcdXmlReader(xml);
				childReader.SkipToNextElement();
			}
			catch (IcdXmlException)
			{
				return;
			}

			path.Push(childReader.Name);
			string[] pathOutput = path.Reverse().ToArray();

			callback(new XmlRecursionEventArgs(xml, pathOutput));

			foreach (string child in childReader.GetChildElementsAsString())
				Recurse(child, path, callback);

			path.Pop();
			childReader.Dispose();
		}

		#endregion

		#region Get Child Element

		/// <summary>
		/// Returns true if the given xml element contains at least 1 child element.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static bool HasChildElements(string xml)
		{
			using (IcdXmlReader reader = new IcdXmlReader(xml))
			{
				reader.SkipToNextElement();
				return reader.HasChildElements();
			}
		}

		/// <summary>
		/// Gets the child elements from the topmost element in the xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<IcdXmlReader> GetChildElements(string xml)
		{
			IcdXmlReader reader = new IcdXmlReader(xml);
			reader.SkipToNextElement();

			foreach (IcdXmlReader child in reader.GetChildElements())
				yield return child;

			reader.Dispose();
		}

		/// <summary>
		/// Returns the child elements with the given name.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="element"></param>
		[PublicAPI]
		public static IEnumerable<string> GetChildElementsAsString(string xml, string element)
		{
			using (IcdXmlReader reader = new IcdXmlReader(xml))
			{
				reader.SkipToNextElement();

				foreach (IcdXmlReader child in reader.GetChildElements())
				{
					string output = null;
					if (child.Name == element)
						output = child.ReadOuterXml();

					child.Dispose();

					if (output != null)
						yield return output;
				}
			}
		}

		/// <summary>
		/// Gets the child elements for the current element.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<string> GetChildElementsAsString(string xml)
		{
			using (IcdXmlReader reader = new IcdXmlReader(xml))
			{
				reader.SkipToNextElement();
				foreach (string item in reader.GetChildElementsAsString())
					yield return item;
			}
		}

		/// <summary>
		/// Gets the immediate child element with the given name.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="element"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string GetChildElementAsString(string xml, string element)
		{
			string output;

			if (!TryGetChildElementAsString(xml, element, out output))
				throw new FormatException(string.Format("No child element named {0}", element));
			return output;
		}

		/// <summary>
		/// Gets the immediate child element with the given name.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="element"></param>
		/// <param name="output"></param>
		/// <returns>Whether or not the method succeeded</returns>
		[PublicAPI]
		public static bool TryGetChildElementAsString(string xml, string element, out string output)
		{
			using (IcdXmlReader reader = new IcdXmlReader(xml))
			{
				reader.SkipToNextElement();

				foreach (IcdXmlReader child in reader.GetChildElements())
				{
					output = null;
					if (child.Name == element)
						output = child.ReadOuterXml();

					child.Dispose();

					if (output != null)
						return true;
				}
			}
			output = null;
			return false;
		}

		#endregion

		#region Read Child Element

		/// <summary>
		/// Gets the content of an immediate child.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="childElement"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string ReadChildElementContentAsString(string xml, string childElement)
		{
			string child = GetChildElementAsString(xml, childElement);
			using (IcdXmlReader reader = new IcdXmlReader(child))
			{
				reader.SkipToNextElement();
				return reader.ReadElementContentAsString();
			}
		}

		/// <summary>
		/// Gets the content of an immediate child.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="childElement"></param>
		/// <returns></returns>
		[PublicAPI]
		public static int ReadChildElementContentAsInt(string xml, string childElement)
		{
			string child = GetChildElementAsString(xml, childElement);
			using (IcdXmlReader reader = new IcdXmlReader(child))
			{
				reader.SkipToNextElement();
				return reader.ReadElementContentAsInt();
			}
		}

		/// <summary>
		/// Gets the content of an immediate child.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="childElement"></param>
		/// <returns></returns>
		[PublicAPI]
		public static uint ReadChildElementContentAsUint(string xml, string childElement)
		{
			string child = GetChildElementAsString(xml, childElement);
			using (IcdXmlReader reader = new IcdXmlReader(child))
			{
				reader.SkipToNextElement();
				return reader.ReadElementContentAsUint();
			}
		}

		/// <summary>
		/// Gets the content of an immediate child.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="childElement"></param>
		/// <returns></returns>
		[PublicAPI]
		public static long ReadChildElementContentAsLong(string xml, string childElement)
		{
			string child = GetChildElementAsString(xml, childElement);
			using (IcdXmlReader reader = new IcdXmlReader(child))
			{
				reader.SkipToNextElement();
				return reader.ReadElementContentAsLong();
			}
		}

		/// <summary>
		/// Gets the content of an immediate child.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="childElement"></param>
		/// <returns></returns>
		[PublicAPI]
		public static ushort ReadChildElementContentAsUShort(string xml, string childElement)
		{
			string child = GetChildElementAsString(xml, childElement);
			using (IcdXmlReader reader = new IcdXmlReader(child))
			{
				reader.SkipToNextElement();
				return reader.ReadElementContentAsUShort();
			}
		}

		/// <summary>
		/// Gets the content of an immediate child.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="childElement"></param>
		/// <returns></returns>
		[PublicAPI]
		private static float? ReadChildElementContentAsFloat(string xml, string childElement)
		{
			string child = GetChildElementAsString(xml, childElement);
			using (IcdXmlReader reader = new IcdXmlReader(child))
			{
				reader.SkipToNextElement();
				return reader.ReadElementContentAsFloat();
			}
		}

		/// <summary>
		/// Gets the content of an immediate child.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="childElement"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool ReadChildElementContentAsBoolean(string xml, string childElement)
		{
			// IcdXmlReader.ReadElementContentAsBoolean() is too case sensitive
			string output = ReadChildElementContentAsString(xml, childElement);
			return bool.Parse(output);
		}

		/// <summary>
		/// Gets the content of an immediate child.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="childElement"></param>
		/// <returns></returns>
		[PublicAPI]
		public static byte ReadChildElementContentAsByte(string xml, string childElement)
		{
			string child = GetChildElementAsString(xml, childElement);
			using (IcdXmlReader reader = new IcdXmlReader(child))
			{
				reader.SkipToNextElement();
				return reader.ReadElementContentAsByte();
			}
		}

		/// <summary>
		/// Gets the content of an immediate child.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="childElement"></param>
		/// <param name="ignoreCase"></param>
		/// <returns></returns>
		[PublicAPI]
		public static T ReadChildElementContentAsEnum<T>(string xml, string childElement, bool ignoreCase)
		{
			if (!EnumUtils.IsEnumType<T>())
				throw new ArgumentException(string.Format("{0} is not an enum", typeof(T).Name));

			string child = GetChildElementAsString(xml, childElement);
			using (IcdXmlReader reader = new IcdXmlReader(child))
			{
				reader.SkipToNextElement();
				return reader.ReadElementContentAsEnum<T>(ignoreCase);
			}
		}

		#endregion

		#region Try Read Child Element

		/// <summary>
		/// Gets the content of the immediate child. Returns null if the child element was not found.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="childElement"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string TryReadChildElementContentAsString(string xml, string childElement)
		{
			try
			{
				return ReadChildElementContentAsString(xml, childElement);
			}
			catch (FormatException)
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the content of the immediate child. Returns null if the child element was not found.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="childElement"></param>
		/// <returns></returns>
		[PublicAPI]
		public static int? TryReadChildElementContentAsInt(string xml, string childElement)
		{
			try
			{
				return ReadChildElementContentAsInt(xml, childElement);
			}
			catch (FormatException)
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the content of the immediate child. Returns null if the child element was not found.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="childElement"></param>
		/// <returns></returns>
		[PublicAPI]
		public static long? TryReadChildElementContentAsLong(string xml, string childElement)
		{
			try
			{
				return ReadChildElementContentAsLong(xml, childElement);
			}
			catch (FormatException)
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the content of the immediate child. Returns null if the child element was not found.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="childElement"></param>
		/// <returns></returns>
		[PublicAPI]
		public static ushort? TryReadChildElementContentAsUShort(string xml, string childElement)
		{
			try
			{
				return ReadChildElementContentAsUShort(xml, childElement);
			}
			catch (FormatException)
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the content of the immediate child. Returns null if the child element was not found.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="childElement"></param>
		/// <returns></returns>
		[PublicAPI]
		public static float? TryReadChildElementContentAsFloat(string xml, string childElement)
		{
			try
			{
				return ReadChildElementContentAsFloat(xml, childElement);
			}
			catch (FormatException)
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the content of an immediate child. Returns null if the child element was not found.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="childElement"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool? TryReadChildElementContentAsBoolean(string xml, string childElement)
		{
			try
			{
				return ReadChildElementContentAsBoolean(xml, childElement);
			}
			catch (FormatException)
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the content of the immediate child. Returns null if the child element was not found.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="childElement"></param>
		/// <returns></returns>
		[PublicAPI]
		public static byte? TryReadChildElementContentAsByte(string xml, string childElement)
		{
			try
			{
				return ReadChildElementContentAsByte(xml, childElement);
			}
			catch (FormatException)
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the content of the immediate child. Returns default if the child element could not be parsed.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="xml"></param>
		/// <param name="childElement"></param>
		/// <param name="ignoreCase"></param>
		/// <returns></returns>
		[PublicAPI]
		public static T? TryReadChildElementContentAsEnum<T>(string xml, string childElement, bool ignoreCase)
			where T : struct
		{
			if (!EnumUtils.IsEnumType<T>())
				throw new ArgumentException(string.Format("{0} is not an enum", typeof(T).Name));

			T output;
			return TryReadChildElementContentAsEnum(xml, childElement, ignoreCase, out output) ? output : (T?)null;
		}

		/// <summary>
		/// Gets the content of the immediate child. Returns false if the child element could not be parsed.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="xml"></param>
		/// <param name="childElement"></param>
		/// <param name="ignoreCase"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool TryReadChildElementContentAsEnum<T>(string xml, string childElement, bool ignoreCase, out T output)
		{
			if (!EnumUtils.IsEnumType<T>())
				throw new ArgumentException(string.Format("{0} is not an enum", typeof(T).Name));

			output = default(T);

			try
			{
				output = ReadChildElementContentAsEnum<T>(xml, childElement, ignoreCase);
				return true;
			}
			// Null xml
			catch (ArgumentException)
			{
			}
			catch (FormatException)
			{
			}

			return false;
		}

		#endregion

		#region Read Element Content

		/// <summary>
		/// Returns the content for a single element.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string ReadElementContent(string xml)
		{
			using (IcdXmlReader reader = new IcdXmlReader(xml))
			{
				reader.SkipToNextElement();
				return reader.ReadElementContentAsString();
			}
		}

		/// <summary>
		/// Parses the element content as a uint.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI]
		public static uint ReadElementContentAsUint(string xml)
		{
			using (IcdXmlReader reader = new IcdXmlReader(xml))
			{
				reader.Read();
				return reader.ReadElementContentAsUint();
			}
		}

		/// <summary>
		/// Parses the element content as a uint.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI]
		public static int ReadElementContentAsInt(string xml)
		{
			using (IcdXmlReader reader = new IcdXmlReader(xml))
			{
				reader.Read();
				return reader.ReadElementContentAsInt();
			}
		}

		/// <summary>
		/// Parses the element content as a uint.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI]
		public static ushort ReadElementContentAsUShort(string xml)
		{
			using (IcdXmlReader reader = new IcdXmlReader(xml))
			{
				reader.Read();
				return reader.ReadElementContentAsUShort();
			}
		}

		/// <summary>
		/// Parses the element content as an enum.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="xml"></param>
		/// <param name="ignoreCase"></param>
		/// <returns></returns>
		[PublicAPI]
		public static T ReadElementContentAsEnum<T>(string xml, bool ignoreCase)
		{
			if (!EnumUtils.IsEnumType<T>())
				throw new ArgumentException(string.Format("{0} is not an enum", typeof(T).Name));

			using (IcdXmlReader reader = new IcdXmlReader(xml))
			{
				reader.Read();
				return reader.ReadElementContentAsEnum<T>(ignoreCase);
			}
		}

		#endregion

		#region Try Read Element Content

		/// <summary>
		/// Parses the element content as a uint.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI]
		public static uint? TryReadElementContentAsUint(string xml)
		{
			IcdXmlReader reader = new IcdXmlReader(xml);
			reader.Read();
			try
			{
				return reader.ReadElementContentAsUint();
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Parses the element content as a uint.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI]
		public static int? TryReadElementContentAsInt(string xml)
		{
			IcdXmlReader reader = new IcdXmlReader(xml);
			reader.Read();
			try
			{
				return reader.ReadElementContentAsInt();
			}
			catch
			{
				return null;
			}
		}

		#endregion

		/// <summary>
		/// Returns the name of the first element in the given xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		/// <exception cref="FormatException">No element in the xml</exception>
		[PublicAPI]
		public static string ReadElementName(string xml)
		{
			using (IcdXmlReader reader = new IcdXmlReader(xml))
			{
				if (!reader.SkipToNextElement())
					throw new FormatException("Unable to read element name, no element in given xml");
				return reader.Name;
			}
		}

		#region Read Content

		/// <summary>
		/// Calls readKey and readValue for each key/value pair in the dict.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="rootElement"></param>
		/// <param name="childElement"></param>
		/// <param name="keyElement"></param>
		/// <param name="valueElement"></param>
		/// <param name="readKey"></param>
		/// <param name="readValue"></param>
		public static IEnumerable<KeyValuePair<TKey, TValue>> ReadDictFromXml<TKey, TValue>(
			string xml, string rootElement, string childElement, string keyElement, string valueElement,
			Func<string, TKey> readKey, Func<string, TValue> readValue)
		{
			if (readKey == null)
				throw new ArgumentNullException("readKey");

			if (readValue == null)
				throw new ArgumentNullException("readValue");

			Func<string, KeyValuePair<TKey, TValue>> readChild =
				child =>
				{
					string key;
					string value;

					TryGetChildElementAsString(child, keyElement, out key);
					TryGetChildElementAsString(child, valueElement, out value);

					return new KeyValuePair<TKey, TValue>(readKey(key), readValue(value));
				};

			return ReadListFromXml(xml, rootElement, childElement, readChild);
		}

		/// <summary>
		/// Calls childElementCallback for each item in the list.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="rootElement"></param>
		/// <param name="childElement"></param>
		/// <param name="readChild"></param>
		public static IEnumerable<T> ReadListFromXml<T>(string xml, string rootElement, string childElement,
		                                                Func<string, T> readChild)
		{
			if (readChild == null)
				throw new ArgumentNullException("readChild");

			if (!TryGetChildElementAsString(xml, rootElement, out xml))
				yield break;

			foreach (string child in GetChildElementsAsString(xml, childElement))
				yield return readChild(child);
		}

		#endregion

		#region Write Content

		/// <summary>
		/// Serializes the given list to XML using IcdXmlConvert to write each key and value.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="writer"></param>
		/// <param name="dict"></param>
		/// <param name="rootElement"></param>
		/// <param name="childElement"></param>
		/// <param name="keyElement"></param>
		/// <param name="valueElement"></param>
		public static void WriteDictToXml<TKey, TValue>(IcdXmlTextWriter writer, IEnumerable<KeyValuePair<TKey, TValue>> dict,
		                                                string rootElement, string childElement, string keyElement,
		                                                string valueElement)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			if (dict == null)
				throw new ArgumentNullException("dict");

			Action<TKey> writeKey = key => writer.WriteElementString(keyElement, IcdXmlConvert.ToString(key));
			Action<TValue> writeValue = value => writer.WriteElementString(valueElement, IcdXmlConvert.ToString(value));

			WriteDictToXml(writer, dict, rootElement, childElement, writeKey, writeValue);
		}

		/// <summary>
		/// Serializes the given list to XML using the actions to write each item.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="writer"></param>
		/// <param name="dict"></param>
		/// <param name="rootElement"></param>
		/// <param name="childElement"></param>
		/// <param name="writeKey"></param>
		/// <param name="writeValue"></param>
		public static void WriteDictToXml<TKey, TValue>(IcdXmlTextWriter writer, IEnumerable<KeyValuePair<TKey, TValue>> dict,
		                                                string rootElement, string childElement, Action<TKey> writeKey,
		                                                Action<TValue> writeValue)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			if (dict == null)
				throw new ArgumentNullException("dict");

			if (writeKey == null)
				throw new ArgumentNullException("writeKey");

			if (writeValue == null)
				throw new ArgumentNullException("writeValue");

			Action<KeyValuePair<TKey, TValue>> writeItem =
				pair =>
				{
					writer.WriteStartElement(childElement);
					{
						writeKey(pair.Key);
						writeValue(pair.Value);
					}
					writer.WriteEndElement();
				};

			WriteListToXml(writer, dict, rootElement, writeItem);
		}

		/// <summary>
		/// Serializes the given list to XML using IcdXmlConvert to write each item.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="writer"></param>
		/// <param name="list"></param>
		/// <param name="listElement"></param>
		/// <param name="childElement"></param>
		public static void WriteListToXml<T>(IcdXmlTextWriter writer, IEnumerable<T> list, string listElement,
		                                     string childElement)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			if (list == null)
				throw new ArgumentNullException("list");

			Action<T> writeItem = child => writer.WriteElementString(childElement, IcdXmlConvert.ToString(child));
			WriteListToXml(writer, list, listElement, writeItem);
		}

		/// <summary>
		/// Serializes the given list to XML using the writeItem action to write each item.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="writer"></param>
		/// <param name="list"></param>
		/// <param name="listElement"></param>
		/// <param name="writeItem"></param>
		public static void WriteListToXml<T>(IcdXmlTextWriter writer, IEnumerable<T> list, string listElement,
		                                     Action<T> writeItem)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			if (list == null)
				throw new ArgumentNullException("list");

			if (writeItem == null)
				throw new ArgumentNullException("writeItem");

			writer.WriteStartElement(listElement);
			{
				foreach (T child in list)
					writeItem(child);
			}
			writer.WriteEndElement();
		}

		#endregion

		/// <summary>
		/// Returns true if the given xml is valid.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool IsValidXml(string xml)
		{
			try
			{
				IcdXmlDocument document = new IcdXmlDocument();
				document.LoadXml(xml);
				return true;
			}
			catch (IcdXmlException)
			{
				return false;
			}
		}

		/// <summary>
		/// Prints the xml document.
		/// </summary>
		/// <param name="xml"></param>
		[PublicAPI]
		public static void Print(string xml)
		{
			string result = Format(xml);
			IcdConsole.PrintLine(result);
		}

		/// <summary>
		/// Formats the given xml string into a human readable structure with indentations.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string Format(string xml)
		{
			using (IcdMemoryStream mStream = new IcdMemoryStream())
			{
				using (IcdXmlTextWriter writer = new IcdXmlTextWriter(mStream, Encoding.UTF8))
				{
					IcdXmlDocument document = new IcdXmlDocument();

					// Load the XmlDocument with the XML.
					document.LoadXml(xml);

					// Write the XML into a formatting IcdXmlTextWriter
					document.WriteContentTo(writer);
					writer.Flush();
					mStream.Flush();

					// Have to rewind the MemoryStream in order to read its contents.
					mStream.Position = 0;
					return new IcdStreamReader(mStream).ReadToEnd();
				}
			}
		}
	}
}
