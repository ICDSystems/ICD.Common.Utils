using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronXml;
#else
using System.Xml;
#endif

namespace ICD.Common.Utils.Xml
{
	public static class XmlReaderExtensions
	{
		#region Attributes

		/// <summary>
		/// Returns true if the attribute exists.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool HasAttribute(this IcdXmlReader extends, string name)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.GetAttribute(name) != null;
		}

		/// <summary>
		/// Gets the attributes for the current element without moving the reader.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<IcdXmlAttribute> GetAttributes(this IcdXmlReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (!extends.HasAttributes)
				return Enumerable.Empty<IcdXmlAttribute>();

			List<IcdXmlAttribute> attributes = new List<IcdXmlAttribute>();
			while (extends.MoveToNextAttribute())
				attributes.Add(new IcdXmlAttribute(extends.Name, extends.Value));

			// Move back to element.
			extends.MoveToElement();

			return attributes.ToArray();
		}

		/// <summary>
		///	Gets the value of the attribute with the given name.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		[PublicAPI]
		public static string GetAttributeAsString(this IcdXmlReader extends, string name)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			string output = extends.GetAttribute(name);
			if (output == null)
				throw new FormatException(string.Format("Missing attribute \"{0}\"", name));

			return output;
		}

		/// <summary>
		///	Gets the value of the attribute with the given name and returns as an integer.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		[PublicAPI]
		public static int GetAttributeAsInt(this IcdXmlReader extends, string name)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			string value = extends.GetAttributeAsString(name);
			return int.Parse(value);
		}

		/// <summary>
		/// Gets the value of the attribute with the given name and returns as a bool.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool GetAttributeAsBool(this IcdXmlReader extends, string name)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			string value = extends.GetAttributeAsString(name);
			return bool.Parse(value);
		}

		#endregion

		#region Recurse

		/// <summary>
		/// Recurses through the entire XML, calling the callback for each Element/Text node.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="callback"></param>
		[PublicAPI]
		public static void Recurse(this IcdXmlReader extends, Func<XmlRecursionEventArgs, bool> callback)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			XmlUtils.Recurse(extends.ReadString(), callback);
		}

		#endregion

		#region Skip

		/// <summary>
		/// Skips over insignificant whitespace.
		/// </summary>
		/// <param name="extends"></param>
		[PublicAPI]
		public static void SkipInsignificantWhitespace(this IcdXmlReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			while (extends.NodeType == XmlNodeType.Whitespace && extends.Read())
			{
			}
		}

		/// <summary>
		/// Continues reading until an element is reached.
		/// </summary>
		/// <param name="extends"></param>
		[PublicAPI]
		public static bool ReadToNextElement(this IcdXmlReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			while (extends.Read() && extends.NodeType != XmlNodeType.Element)
			{
			}

			return extends.NodeType == XmlNodeType.Element;
		}

		#endregion

		#region Get Child Element

		/// <summary>
		/// Returns true if the current node has child elements.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static bool HasChildElements(this IcdXmlReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			while (extends.Read() && extends.NodeType != XmlNodeType.EndElement)
			{
				if (extends.NodeType == XmlNodeType.Element)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Gets the child element with the given name under the current element.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="element"></param>
		/// <returns></returns>
		public static IcdXmlReader GetChildElement(this IcdXmlReader extends, string element)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			IcdXmlReader output;
			if (extends.GetChildElements(element).TryFirst(out output))
				return output;

			throw new FormatException("No child element with name " + element);
		}

		/// <summary>
		/// Gets the child elements for the current element.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static IEnumerable<IcdXmlReader> GetChildElements(this IcdXmlReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			foreach (IcdXmlReader output in extends.GetChildElementsAsString().Select(child => new IcdXmlReader(child)))
			{
				output.ReadToNextElement();
				yield return output;
			}
		}

		/// <summary>
		/// Gets the child elements for the current element.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="element"></param>
		/// <returns></returns>
		public static IEnumerable<IcdXmlReader> GetChildElements(this IcdXmlReader extends, string element)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			foreach (IcdXmlReader output in extends.GetChildElementsAsString(element).Select(child => new IcdXmlReader(child)))
			{
				output.ReadToNextElement();
				yield return output;
			}
		}

		/// <summary>
		/// Gets the child elements for the current element.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<string> GetChildElementsAsString(this IcdXmlReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			// Step into the first child.
			extends.ReadToNextElement();

			while (extends.NodeType == XmlNodeType.Element || extends.NodeType == XmlNodeType.Comment)
			{
				switch (extends.NodeType)
				{
					case XmlNodeType.Comment:
						extends.Skip();
						break;

					case XmlNodeType.Element:
						yield return extends.ReadOuterXml();
						break;
				}

				extends.SkipInsignificantWhitespace();
			}
		}

		/// <summary>
		/// Gets the child elements for the current element.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="element"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<string> GetChildElementsAsString(this IcdXmlReader extends, string element)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			// Step into the first child.
			extends.ReadToNextElement();

			while (extends.NodeType == XmlNodeType.Element || extends.NodeType == XmlNodeType.Comment)
			{
				switch (extends.NodeType)
				{
					case XmlNodeType.Comment:
						extends.Skip();
						break;

					case XmlNodeType.Element:
						string name = extends.Name;
						string output = extends.ReadOuterXml();
						if (name == element)
							yield return output;
						break;
				}

				extends.SkipInsignificantWhitespace();
			}
		}

		public static bool TryGetChildElementAsString(this IcdXmlReader extends, string element, out string output)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.GetChildElementsAsString(element).TryFirst(out output);
		}

		#endregion

		#region Read Element Content

		/// <summary>
		/// Parses the element content in the format 0xXX as a byte.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static byte ReadElementContentAsByte(this IcdXmlReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			string content = extends.ReadElementContentAsString();
			return StringUtils.FromIpIdString(content);
		}

		/// <summary>
		/// Parses the element content as a uint.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static uint ReadElementContentAsUint(this IcdXmlReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			string content = extends.ReadElementContentAsString();
			return uint.Parse(content);
		}

		/// <summary>
		/// Parses the element content as a uint.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static int ReadElementContentAsInt(this IcdXmlReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			string content = extends.ReadElementContentAsString();
			return int.Parse(content);
		}

		/// <summary>
		/// Parses the element content as a ushort.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static ushort ReadElementContentAsUShort(this IcdXmlReader extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			string content = extends.ReadElementContentAsString();
			return ushort.Parse(content);
		}

		/// <summary>
		/// Parses the element content as an enum.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="ignoreCase"></param>
		/// <returns></returns>
		[PublicAPI]
		public static T ReadElementContentAsEnum<T>(this IcdXmlReader extends, bool ignoreCase)
			where T : struct, IConvertible
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			string content = extends.ReadElementContentAsString();
			return EnumUtils.Parse<T>(content, ignoreCase);
		}

		#endregion
	}
}
