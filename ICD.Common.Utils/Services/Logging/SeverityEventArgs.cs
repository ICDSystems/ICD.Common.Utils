﻿using ICD.Common.Utils.EventArguments;

namespace ICD.Common.Utils.Services.Logging
{
	public sealed class SeverityEventArgs : GenericEventArgs<eSeverity>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		public SeverityEventArgs(eSeverity data)
			: base(data)
		{
		}
	}
}
