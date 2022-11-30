using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Timers;

namespace ICD.Common.Utils
{
	/// <summary>
	/// Utilizes a priority queue to store items
	/// Dequeues items in priority order and processes
	/// them in a worker thread one at a time
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class ThreadedWorkerQueue<T> : IDisposable
	{
		/// <summary>
		/// Underlying Queue to hold items
		/// </summary>
		private readonly PriorityQueue<T> m_Queue;

		/// <summary>
		/// Bool to track if a thread is currently running to dequeue and process items
		/// </summary>
		private bool m_ProcessRunning;

		/// <summary>
		/// Critical section to lock the queue and the process running bool
		/// </summary>
		private readonly SafeCriticalSection m_QueueSection;

		/// <summary>
		/// Action that should be run for every item as it is dequeued
		/// </summary>
		private readonly Action<T> m_ProcessAction;

		/// <summary>
		/// Event used to block and wait for the queue to empty
		/// </summary>
		private readonly IcdManualResetEvent m_FlushEvent;

		/// <summary>
		/// If true, the queue will be processed.
		/// </summary>
		private bool m_RunProcess;

		private long m_BetweenTime;

		/// <summary>
		/// Time to wait between processing items
		/// If less than or equal to 0, items will be processed immediately
		/// </summary>
		[PublicAPI]
		public long BetweenTime
		{
			get { return m_BetweenTime; }
			set
			{
				if (value < 0)
					throw new InvalidOperationException("BetweenTime can't be negative");

				m_BetweenTime = value;
			}
		}

		/// <summary>
		/// Timer used to wait the BetweenTime
		/// </summary>
		private readonly SafeTimer m_BetweenTimeTimer;

		/// <summary>
		/// While true, the queue will be processed
		/// When set to false, the queue will be be stopped after any current processing is finished
		/// When set to true, the queue processing will be started in a new thread if there are any items in the queue
		/// 
		/// </summary>
		[PublicAPI]
		public bool RunProcess { get { return m_RunProcess; } }

		/// <summary>
		/// Gets the current count of the items in the queue
		/// </summary>
		public int Count
		{
			get { return m_QueueSection.Execute(() => m_Queue.Count); }
		}

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="processItemAction">Action to process the dequeued items</param>
		public ThreadedWorkerQueue([NotNull] Action<T> processItemAction)
			: this(processItemAction, true, 0)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="processItemAction">Action to process the dequeued items</param>
		/// <param name="runProcess">If true, queued items will be processed, if false, no processing will happen until runProcess is set</param>
		public ThreadedWorkerQueue([NotNull] Action<T> processItemAction, bool runProcess)
			: this(processItemAction, runProcess, 0)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="processItemAction">Action to process the dequeued items</param>
		/// <param name="runProcess">If true, queued items will be processed, if false, no processing will happen until runProcess is set</param>
		/// <param name="betweenTime">Time to wait between processing items</param>
		public ThreadedWorkerQueue([NotNull] Action<T> processItemAction, bool runProcess, long betweenTime)
		{
			if (processItemAction == null)
				throw new ArgumentNullException("processItemAction");

			if (betweenTime < 0)
				throw new ArgumentOutOfRangeException("betweenTime", "Between time can't be negative");
			
			m_Queue = new PriorityQueue<T>();
			m_QueueSection = new SafeCriticalSection();
			m_FlushEvent = new IcdManualResetEvent(true);
			m_RunProcess = runProcess;
			m_ProcessAction = processItemAction;
			m_BetweenTimeTimer = SafeTimer.Stopped(BetweenTimerCallback);

			BetweenTime = betweenTime;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Clears the collection.
		/// </summary>
		[PublicAPI]
		public void Clear()
		{
			m_QueueSection.Execute(() => m_Queue.Clear());
		}

		/// <summary>
		/// Blocks until the queue is empty
		/// </summary>
		/// <returns>true if the queue empties</returns>
		[PublicAPI]
		public bool WaitForFlush()
		{
			return m_FlushEvent.WaitOne();
		}

		/// <summary>
		/// Blocks until the queue is empty, or the timeout is reached
		/// </summary>
		/// <param name="timeout">Timeout in ms</param>
		/// <returns>true if the queue empties, false if the timeout is reached</returns>
		[PublicAPI]
		public bool WaitForFlush(int timeout)
		{
			return m_FlushEvent.WaitOne(timeout);
		}

		public void SetRunProcess(bool runProcess)
		{
			m_QueueSection.Enter();
			try
			{
				if (m_RunProcess == runProcess)
					return;

				m_RunProcess = runProcess;

				if (runProcess)
					EnableProcessQueue();

			}
			finally
			{
				m_QueueSection.Leave();
			}
		}

		public void Dispose()
		{
			m_QueueSection.Enter();
			try
			{
				SetRunProcess(false);
				m_Queue.Clear();
				m_BetweenTimeTimer.Stop();
				m_BetweenTimeTimer.Dispose();
				m_FlushEvent.Dispose();
			}
			finally
			{
				m_QueueSection.Leave();
			}
		}

		#endregion

		#region Enqueue Methods

		/// <summary>
		/// Adds the item to the end of the queue.
		/// </summary>
		/// <param name="item"></param>
		[PublicAPI]
		public void Enqueue([CanBeNull] T item)
		{
			Enqueue(item, () => m_Queue.Enqueue(item));
		}

		/// <summary>
		/// Adds the item to the queue with the given priority.
		/// Lower values are dequeued first.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="priority"></param>
		[PublicAPI]
		public void Enqueue([CanBeNull] T item, int priority)
		{
			Enqueue(item, () => m_Queue.Enqueue(item, priority));
		}

		/// <summary>
		/// Adds the item to the queue with the given priority at the given index.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="priority"></param>
		/// <param name="position"></param>
		[PublicAPI]
		public void Enqueue([CanBeNull] T item, int priority, int position)
		{
			Enqueue(item, () => m_Queue.Enqueue(item, priority, position));
		}

		/// <summary>
		/// Enqueues the item at the beginning of the queue.
		/// </summary>
		/// <param name="item"></param>
		[PublicAPI]
		public void EnqueueFirst([CanBeNull] T item)
		{
			Enqueue(item, () => m_Queue.EnqueueFirst(item));
		}

		/// <summary>
		/// Removes any items in the queue matching the predicate.
		/// Appends the given item at the end of the given priority level.
		/// This is useful for reducing duplication, or replacing items with something more pertinent.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="remove"></param>
		[PublicAPI]
		public void EnqueueRemove([CanBeNull] T item, [NotNull] Func<T, bool> remove)
		{
			Enqueue(item, () => m_Queue.EnqueueRemove(item, remove));
		}

		/// <summary>
		/// Removes any items in the queue matching the predicate.
		/// Appends the given item at the end of the given priority level.
		/// This is useful for reducing duplication, or replacing items with something more pertinent.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="remove"></param>
		/// <param name="priority"></param>
		[PublicAPI]
		public void EnqueueRemove([CanBeNull] T item, [NotNull] Func<T, bool> remove, int priority)
		{
			Enqueue(item, () => m_Queue.EnqueueRemove(item, remove, priority));
		}

		/// <summary>
		/// Removes any items in the queue matching the predicate.
		/// Appends the given item at the end of the given priority level.
		/// This is useful for reducing duplication, or replacing items with something more pertinent.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="remove"></param>
		/// <param name="priority"></param>
		/// <param name="deDuplicateToEndOfQueue"></param>
		[PublicAPI]
		public void EnqueueRemove([CanBeNull] T item, [NotNull] Func<T, bool> remove, int priority, bool deDuplicateToEndOfQueue)
		{
			Enqueue(item, () => m_Queue.EnqueueRemove(item, remove, priority, deDuplicateToEndOfQueue));
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Enqueues an item using the given enqueueAction
		/// Starts processing the queue if it's not already processing
		/// </summary>
		/// <param name="item"></param>
		/// <param name="enqueueAction"></param>
		private void Enqueue(T item, Action enqueueAction)
		{
			bool startWorkerThread;
			T nextItem = default(T);

			m_QueueSection.Enter();

			try
			{
				// Reset the flush event, to block the flush thread while something is in the queue/being processed
				m_FlushEvent.Reset();

				// We'll start a worker thread if the process isn't running and should be
				startWorkerThread = !m_ProcessRunning && RunProcess;

				if (startWorkerThread)
					m_ProcessRunning = true;


				if (m_Queue.Count == 0 && startWorkerThread)
				{
					// If the queue is empty and we're starting the worker thread
					// we won't bother adding an item just to dequeue it again
					nextItem = item;
				}
				else if (startWorkerThread)
				{
					// If the queue isn't empty and we are starting the worker thread
					// enqueue the item and dequeue the next item.
					enqueueAction();
					nextItem = m_Queue.Dequeue();
				}
				else
				{
					// If we're not starting the worker thread, just enqueue the item
					enqueueAction();
				}
			}
			finally
			{
				m_QueueSection.Leave();
			}

			if (startWorkerThread)
				ProcessQueue(nextItem);
		}

		/// <summary>
		/// Starts processing the queue by starting a thread
		/// When running this, m_ProcessRunning MUST be set to true already,
		/// and RunProcess should be checked before this
		/// </summary>
		/// <param name="item"></param>
		private void ProcessQueue(T item)
		{
			ThreadingUtils.SafeInvoke(() => ProcessQueueThread(item));
		}

		/// <summary>
		/// Processes the item, meant to be run in it's own thread
		/// When running this, m_ProcessRunning MUST be set to true already,
		/// and RunProcess should be checked before this
		/// </summary>
		/// <param name="item"></param>
		private void ProcessQueueThread(T item)
		{

			while (true)
			{
				// Run the process action
				// todo: Have exceptions raise an event in a new thread, maybe configurable exception handling
				try
				{
					m_ProcessAction(item);
				}
				catch (Exception e)
				{
					ServiceProvider.GetService<ILoggerService>().AddEntry(eSeverity.Error, e, "Exception in ThreadedWorkingQueue process action:{0}", e.Message);
				}

				m_QueueSection.Enter();
				try
				{
					bool runProcess = RunProcess;
					bool hasItem = m_Queue.Count > 0;
					long betweenTime = BetweenTime;

					// Stop processing and return if we don't have another item, or we aren't to keep running the process
					if (!runProcess || !hasItem)
					{
						m_ProcessRunning = false;
						if (!hasItem)
							m_FlushEvent.Set();
						return;
					}

					// If a between time is set, start the timer (and leave m_ProcessRunning set)
					if (betweenTime > 0)
					{
						m_BetweenTimeTimer.Reset(betweenTime);
						return;
					}

					item = m_Queue.Dequeue();
				}
				finally
				{
					m_QueueSection.Leave();
				}
			}
		}

		/// <summary>
		/// Starts processing an item if we should be
		/// Run by RunProcess being set to true
		/// </summary>
		private void EnableProcessQueue()
		{
			T item;
			m_QueueSection.Enter();
			try
			{
				if (!RunProcess || m_ProcessRunning || !m_Queue.TryDequeue(out item))
					return;

				m_ProcessRunning = true;
			}
			finally
			{
				m_QueueSection.Leave();
			}

			ProcessQueue(item);
		}

		private void BetweenTimerCallback()
		{
			T item;

			m_QueueSection.Enter();
			try
			{
				// Check to make sure RunProcess is still true
				// Try to dequeue item
				if (!RunProcess || !m_Queue.TryDequeue(out item))
				{
					m_ProcessRunning = false;
					return;
				}
			}
			finally
			{
				m_QueueSection.Leave();
			}

			// Process the dequeued item
			ProcessQueueThread(item);
		}

		#endregion
	}
}