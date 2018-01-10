﻿using System;
using System.Collections.Generic;

namespace ICD.Common.Utils.EventArguments
{
	public sealed class TcpReceiveEventArgs : EventArgs
	{
		private readonly uint m_ClientId;
		private readonly string m_Data;

		public uint ClientId { get { return m_ClientId; } }

		public string Data { get { return m_Data; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="clientId"></param>
		/// <param name="data"></param>
		public TcpReceiveEventArgs(uint clientId, IEnumerable<byte> data)
		{
			if (data == null)
				throw new ArgumentNullException("data");

			m_ClientId = clientId;
			m_Data = StringUtils.ToString(data);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="clientId"></param>
		/// <param name="data"></param>
		/// <param name="length"></param>
		public TcpReceiveEventArgs(uint clientId, IEnumerable<byte> data, int length)
		{
			if (data == null)
				throw new ArgumentNullException("data");

			m_ClientId = clientId;
			m_Data = StringUtils.ToString(data, length);
		}
	}
}
