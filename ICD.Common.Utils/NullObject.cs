using System;
using System.Collections.Generic;

namespace ICD.Common.Utils
{
	/// <summary>
	/// Convenience wrapper for supporting null keys in hash tables.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public struct NullObject<T> : IEquatable<NullObject<T>>, IComparable<NullObject<T>>
	{
		#region Properties

		public T Item { get; private set; }

		public bool IsNull { get; private set; }

		public static NullObject<T> Null { get { return new NullObject<T>(); } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="item"></param>
		public NullObject(T item)
// ReSharper disable CompareNonConstrainedGenericWithNull
			: this(item, item == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="isnull"></param>
		private NullObject(T item, bool isnull)
			: this()
		{
			IsNull = isnull;
			Item = item;
		}

		#endregion

		public override string ToString()
		{
// ReSharper disable CompareNonConstrainedGenericWithNull
			return Item == null ? "NULL" : Item.ToString();
// ReSharper restore CompareNonConstrainedGenericWithNull
		}

		#region Casting

		public static implicit operator T(NullObject<T> nullObject)
		{
			return nullObject.Item;
		}

		public static implicit operator NullObject<T>(T item)
		{
			return new NullObject<T>(item);
		}

		#endregion

		#region Equality

		public override bool Equals(object obj)
		{
			if (obj == null)
				return IsNull;

			if (!(obj is NullObject<T>))
				return false;

			return Equals((NullObject<T>)obj);
		}

		public bool Equals(NullObject<T> other)
		{
			if (IsNull)
				return other.IsNull;

			return !other.IsNull && Item.Equals(other.Item);
		}

		public override int GetHashCode()
		{
			if (IsNull)
				return 0;

			var result = Item.GetHashCode();

			if (result >= 0)
				result++;

			return result;
		}

		#endregion

		#region Comparable

		public int CompareTo(NullObject<T> other)
		{
			if (IsNull && other.IsNull)
				return 0;

			if (IsNull)
				return -1;

			return Comparer<T>.Default.Compare(Item, other.Item);
		}

		#endregion
	}
}
