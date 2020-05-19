using System;

namespace ICD.Common.Utils.EventArguments
{
	public sealed class DateTimeNullableEventArgs : GenericEventArgs<DateTime?>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		public DateTimeNullableEventArgs(DateTime? data) : base(data)
		{
		}
	}
}
