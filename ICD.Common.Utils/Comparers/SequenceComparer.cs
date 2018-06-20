using System;
using System.Collections.Generic;

namespace ICD.Common.Utils.Comparers
{
	public sealed class SequenceComparer<T> : IComparer<IEnumerable<T>>
	{
		private readonly IComparer<T> m_ItemComparer;

		/// <summary>
		/// Constructor.
		/// </summary>
		public SequenceComparer()
			: this(Comparer<T>.Default)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="comparer"></param>
		public SequenceComparer(IComparer<T> comparer)
		{
			if (comparer == null)
				throw new ArgumentNullException("comparer");

			m_ItemComparer = comparer;
		}

		public int Compare(IEnumerable<T> x, IEnumerable<T> y)
		{
			if (x == null)
				throw new ArgumentNullException("x");

			if (y == null)
				throw new ArgumentNullException("y");

			using (IEnumerator<T> firstPos = x.GetEnumerator())
			{
				using (IEnumerator<T> secondPos = y.GetEnumerator())
				{
					bool hasFirst = firstPos.MoveNext();
					bool hasSecond = secondPos.MoveNext();

					while (hasFirst && hasSecond)
					{
						int result = m_ItemComparer.Compare(firstPos.Current, secondPos.Current);
						if (result != 0)
							return result;

						hasFirst = firstPos.MoveNext();
						hasSecond = secondPos.MoveNext();
					}

					if (!hasFirst && !hasSecond)
						return 0;

					if (!hasFirst)
						return -1;

					return 1;
				}
			}
		}
	}
}
