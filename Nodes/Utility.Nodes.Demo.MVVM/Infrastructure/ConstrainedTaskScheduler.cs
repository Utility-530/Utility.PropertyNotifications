using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Utility.Nodes.Demo.MVVM
{

    // Provides a task scheduler that ensures a maximum concurrency level while
    // running on top of the thread pool.
    // <a href="https://www.codeproject.com/Articles/5274136/Customizing-the-TaskScheduler-Queue-Your-Task-Work"/>
    // from Microsoft @ https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskscheduler
    // adapted for my needs
    // after some research I found that for the access patterns used by the consumer
    // of this class, the locked LinkedList<T> is faster than other approaches
    // - honey the codewitch
    /// <summary>
    /// Represents a task scheduler that is bounded to a maximum concurrency
    /// <a href="https://www.codeproject.com/Articles/5274751/Understanding-the-SynchronizationContext-in-NET-wi"></a>
    /// </summary>
#if TASKSLIB
    public
#endif
    public class ConstrainedTaskScheduler : TaskScheduler
    {
        // Indicates whether the current thread is processing work items.
        [ThreadStatic]
        static bool _currentThreadIsProcessingItems;

        // The list of tasks to be executed
        readonly LinkedList<Task> _tasks = new LinkedList<Task>(); // protected by lock(_tasks)

        // The maximum concurrency level allowed by this scheduler.
        int _maximumTaskCount = Math.Max(1, Environment.ProcessorCount - 1);

        // Indicates the currently processing work items.
        int _pendingTaskCount = 0;

        // Indicates the waiting task count
        int _waitingTaskCount = 0;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        public ConstrainedTaskScheduler()
        {

        }
        /// <summary>
        /// Indicates the maximum number of concurrent tasks
        /// </summary>
        public int MaximumTaskCount
        {
            get
            {
                return _maximumTaskCount;
            }
            set
            {
                if (1 > value)
                    throw new ArgumentOutOfRangeException();
                // has to be thread safe:
                Interlocked.Exchange(ref _maximumTaskCount, value);
            }
        }
        /// <summary>
        /// Indicates how many tasks are queued to the system for execution. This is almost the same as running tasks, but it's more of an estimate
        /// </summary>
        public int PendingTaskCount
        {
            get
            {
                return _pendingTaskCount;
            }
        }
        /// <summary>
        /// Indicates the number of tasks waiting to be executed
        /// </summary>
        public int WaitingTaskCount
        {
            get
            {
                return _waitingTaskCount;
            }
        }
        /// <summary>
        /// Queues a task to the scheduler
        /// </summary>
        /// <param name="task">The task to queue</param>
        protected sealed override void QueueTask(Task task)
        {
            // Add the task to the list of tasks to be processed.  If there aren't enough
            // delegates currently queued or running to process tasks, schedule another.
            lock (_tasks)
            {
                _tasks.AddLast(task);
                ++_waitingTaskCount;
                if (_pendingTaskCount < _maximumTaskCount)
                {
                    ++_pendingTaskCount;
                    _NotifyThreadPoolOfPendingWork();
                }
            }
        }

        // Inform the ThreadPool that there's work to be executed for this scheduler.
        void _NotifyThreadPoolOfPendingWork()
        {
            ThreadPool.UnsafeQueueUserWorkItem((state) =>
            {
                // Note that the current thread is now processing work items.
                // This is necessary to enable inlining of tasks into this thread.
                _currentThreadIsProcessingItems = true;
                try
                {
                    // Process all available items in the queue.
                    while (true)
                    {
                        Task item;
                        lock (_tasks)
                        {
                            // When there are no more items to be processed,
                            // note that we're done processing, and get out.
                            if (0 == _tasks.Count)
                            {
                                --_pendingTaskCount;
                                break;
                            }

                            // Get the next item from the queue
                            item = _tasks.First.Value;
                            _tasks.RemoveFirst();
                            --_waitingTaskCount;
                        }

                        // Execute the task we pulled out of the queue
                        TryExecuteTask(item);

                    }
                }
                // We're done processing items on the current thread
                finally { _currentThreadIsProcessingItems = false; }
            }, null);
        }

        /// <summary>
        /// Attempts to execute the specified task on the current thread. 
        /// </summary>
        /// <param name="task">The task</param>
        /// <param name="taskWasPreviouslyQueued">Indicates whether the item was already queued</param>
        /// <returns>True if the task was executed, otherwise false</returns>
        protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // If this thread isn't already processing a task, we don't support inlining
            if (!_currentThreadIsProcessingItems) return false;

            // If the task was previously queued, remove it from the queue
            if (taskWasPreviouslyQueued)
                // Try to run the task.
                if (TryDequeue(task))
                {
                    return TryExecuteTask(task);
                }
                else
                    return false;
            else
                return TryExecuteTask(task);
        }
        /// <summary>
        /// Attempt to remove a previously scheduled task from the scheduler. 
        /// </summary>
        /// <param name="task">The task to remove</param>
        /// <returns>True if a task was removed, otherwise false</returns>
        protected sealed override bool TryDequeue(Task task)
        {
            lock (_tasks)
            {
                if (_tasks.Remove(task))
                {
                    --_waitingTaskCount;
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Indicates the maximum concurrency level supported by this scheduler.
        /// </summary>
        public sealed override int MaximumConcurrencyLevel { get { return _maximumTaskCount; } }

        /// <summary>
        /// Gets an enumerable of the tasks currently scheduled on this scheduler. 
        /// </summary>
        /// <returns>The scheduled tasks</returns>
        /// <remarks>Intended for use by debuggers</remarks>
        protected sealed override IEnumerable<Task> GetScheduledTasks()
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_tasks, ref lockTaken);
                if (lockTaken) return _tasks;
                else throw new NotSupportedException();
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_tasks);
            }
        }
    }
}
