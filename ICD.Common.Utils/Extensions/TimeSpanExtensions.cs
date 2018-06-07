using System;
using System.Text;

namespace ICD.Common.Utils.Extensions
{
	public static class TimeSpanExtensions
	{
		public static string ToReadableString(this TimeSpan extends)
		{
			StringBuilder builder = new StringBuilder();

			if (extends.Days > 0)
				builder.AppendFormat("{0} days, ", extends.Days);
			if (extends.Hours > 0)
				builder.AppendFormat("{0} hours, ", extends.Hours);
			if (extends.Minutes > 0)
				builder.AppendFormat("{0} minutes, ", extends.Minutes);
			if (extends.Seconds > 0)
				builder.AppendFormat("{0} seconds, ", extends.Seconds);
			if (extends.Milliseconds > 0)
			{
				builder.AppendFormat("{0}.{1} ms", extends.Milliseconds,
				                     ((double)extends.Ticks / TimeSpan.TicksPerMillisecond) - extends.TotalMilliseconds);
			}

			return builder.ToString();
		}
	}
}
