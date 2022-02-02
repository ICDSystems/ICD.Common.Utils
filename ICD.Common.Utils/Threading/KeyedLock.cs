#if !SIMPLSHARP
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ICD.Common.Properties;

namespace ICD.Common.Utils.Threading
{
	public sealed class KeyedLock<TKey>
	{
		private readonly Dictionary<TKey, (SemaphoreSlim, int)> m_PerKey;
		private readonly Queue<SemaphoreSlim> m_Pool;
		private readonly int m_PoolCapacity;

		public KeyedLock(IEqualityComparer<TKey> keyComparer = null, int poolCapacity = 10)
		{
			m_PerKey = new Dictionary<TKey, (SemaphoreSlim, int)>(keyComparer);
			m_Pool = new Queue<SemaphoreSlim>(poolCapacity);
			m_PoolCapacity = poolCapacity;
		}

		#region Methods

		[PublicAPI]
		public async Task<bool> WaitAsync(TKey key, int millisecondsTimeout,
		                                  CancellationToken cancellationToken = default)
		{
			var semaphore = GetSemaphore(key);
			bool entered = false;
			try
			{
				entered = await semaphore.WaitAsync(millisecondsTimeout,
				                                    cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				if (!entered) ReleaseSemaphore(key, entered: false);
			}

			return entered;
		}

		[PublicAPI]
		public Task WaitAsync(TKey key, CancellationToken cancellationToken = default)
			=> WaitAsync(key, Timeout.Infinite, cancellationToken);

		[PublicAPI]
		public bool Wait(TKey key, int millisecondsTimeout,
		                 CancellationToken cancellationToken = default)
		{
			var semaphore = GetSemaphore(key);
			bool entered = false;
			try
			{
				entered = semaphore.Wait(millisecondsTimeout, cancellationToken);
			}
			finally
			{
				if (!entered)
					ReleaseSemaphore(key, entered: false);
			}

			return entered;
		}

		[PublicAPI]
		public void Wait(TKey key, CancellationToken cancellationToken = default)
			=> Wait(key, Timeout.Infinite, cancellationToken);

		[PublicAPI]
		public void Release(TKey key) => ReleaseSemaphore(key, entered: true);

		#endregion

		#region Private Methods

		private SemaphoreSlim GetSemaphore(TKey key)
		{
			SemaphoreSlim semaphore;
			lock (m_PerKey)
			{
				if (m_PerKey.TryGetValue(key, out var entry))
				{
					int counter;
					(semaphore, counter) = entry;
					m_PerKey[key] = (semaphore, ++counter);
				}
				else
				{
					lock (m_Pool) semaphore = m_Pool.Count > 0 ? m_Pool.Dequeue() : null;
					if (semaphore == null) semaphore = new SemaphoreSlim(1, 1);
					m_PerKey[key] = (semaphore, 1);
				}
			}

			return semaphore;
		}

		private void ReleaseSemaphore(TKey key, bool entered)
		{
			SemaphoreSlim semaphore;
			int counter;
			lock (m_PerKey)
			{
				if (m_PerKey.TryGetValue(key, out var entry))
				{
					(semaphore, counter) = entry;
					counter--;
					if (counter == 0)
						m_PerKey.Remove(key);
					else
						m_PerKey[key] = (semaphore, counter);
				}
				else
				{
					throw new InvalidOperationException("Key not found.");
				}
			}

			if (entered)
				semaphore.Release();

			if (counter == 0)
			{
				Debug.Assert(semaphore.CurrentCount == 1);
				lock (m_Pool)
				{
					if (m_Pool.Count < m_PoolCapacity)
						m_Pool.Enqueue(semaphore);
				}
			}
		}

		#endregion
	}
}
#endif
