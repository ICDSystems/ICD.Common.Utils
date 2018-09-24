using System.Collections;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Common.Utils.Email
{
	/// <summary>
	/// Stores addresses/paths and provides them in a ; delimited string.
	/// </summary>
	public sealed class EmailStringCollection : ICollection<string>
	{
		private const char DELIMITER = ';';

		private readonly List<string> m_Items;

		/// <summary>
		/// Gets the number of items.
		/// </summary>
		public int Count { get { return m_Items.Count; } }

		public bool IsReadOnly { get { return false; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		public EmailStringCollection()
		{
			m_Items = new List<string>();
		}

		#region Methods

		/// <summary>
		/// Adds the item to the collection.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		[PublicAPI]
		public bool Add(string item)
		{
			if (m_Items.Contains(item))
				return false;

			m_Items.Add(item);
			return true;
		}

		/// <summary>
		/// Removes the item from the collection.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		[PublicAPI]
		public bool Remove(string item)
		{
			if (!m_Items.Contains(item))
				return false;

			m_Items.Remove(item);
			return true;
		}

		/// <summary>
		/// Clears all items.
		/// </summary>
		public void Clear()
		{
			m_Items.Clear();
		}

		/// <summary>
		/// Returns true if the collection contains the item.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		[PublicAPI]
		public bool Contains(string item)
		{
			return m_Items.Contains(item);
		}

		/// <summary>
		/// Gets the items as a ; delimited string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Join(DELIMITER.ToString(), m_Items.ToArray());
		}

		#endregion

		#region Collection

		void ICollection<string>.Add(string item)
		{
			Add(item);
		}

		/// <summary>
		/// Adds multiple email addresses, seperated by ;
		/// </summary>
		/// <param name="items"></param>
		public void AddMany(string items)
		{
			items = items.RemoveWhitespace();
			string [] splitItems = items.Split(DELIMITER);
			foreach (string item in splitItems)
				Add(item);
		}

		public void CopyTo(string[] array, int arrayIndex)
		{
			m_Items.CopyTo(array, arrayIndex);
		}

		#endregion

		#region Enumerable

		public IEnumerator<string> GetEnumerator()
		{
			return m_Items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}