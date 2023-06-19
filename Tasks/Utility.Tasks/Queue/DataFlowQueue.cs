
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Reactive.Subjects;
using Utility.Tasks.Model;
using Utility.Enums;
using System.Collections.Generic;
using Utility.Infrastructure;
using Utility.Progressions;

namespace Utility.Tasks
{
    // To avoid circular-dependency have to make IObserver<ITaskItem> a property
    public class DataFlowQueue : BaseDataFlowQueue, /*IObserver<ITaskItem>, */IObservable<ITaskOutput>
    {
        private readonly ReplaySubject<IWorkerItem> taskItemsSubject = new();
        private readonly ReplaySubject<ITaskOutput> output = new();
        private readonly ActionBlock<Action> _block;

        public DataFlowQueue(
             //IEnumerable<IObservable<ITaskItem>> taskItems,
             IBasicObservableCollection<IProgressState> basicCollection,
             ISchedulerWrapper schedulerWrapper,
             IObservable<ProcessState> processStates = default,
             CancellationTokenSource tokenSource = default) : base(tokenSource)
        {
            tokenSource ??= new CancellationTokenSource();
            //taskItems.ToObservable().SelectMany(a => a).Subscribe(this.taskItemsSubject.OnNext);
            processStates?.Subscribe(commands.OnNext);


            output.Subscribe(a =>
            {

            });

            progressStateSubject
                .Where(a => a.State == ProcessState.Terminated)
                .Subscribe(a =>
                {

                });

            _block = new ActionBlock<Action>(act => act(),
                new ExecutionDataflowBlockOptions()
                {
                    TaskScheduler = TaskScheduler.Default,
                    BoundedCapacity = 1, // Cap the item count     
                    CancellationToken = tokenSource.Token, // Enable cancellation
                    MaxDegreeOfParallelism = Environment.ProcessorCount, // Parallelize on all cores
                });

            progressStateSubject
                .Distinct(a => (a.Key, a.State))
                .Subscribe(a =>
                {
                    if (a.State == ProcessState.Ready)
                        basicCollection.Add(a);      
                    if (a.State == ProcessState.Created)
                        basicCollection.Remove(a);
                    if (a.State == ProcessState.Terminated)
                        basicCollection.Remove(a);   
                    if (a.State == ProcessState.Blocked)
                        basicCollection.Remove(a);
                    if (a.State == ProcessState.None)
                        basicCollection.Remove(a);
                });

            var workerItemsObservable =
                taskItemsSubject?
                 .Select(t => t)
                 .DistinctUntilChanged(a => a.Key)
                 .SubscribeOn(schedulerWrapper.Scheduler)
                 .ObserveOn(schedulerWrapper.Scheduler)
                 .Do(workerItem =>
                 {
                     workerItem.Subscribe(a => progressStateSubject.OnNext(a));
                     workerItem.Select(a => a.Output).Where(a => a != null).Subscribe(output.OnNext);
                 });

            _ = workerItemsObservable
                 .Zip(basicCollection, (a, b) => a)
                 .Subscribe(async item =>
                 {
                     if (await StartAsync(item) == false)
                     {
                         throw new NotImplementedException();
                     }
                 },
                 e =>
                 {
                 });

            _ = commands
                .Subscribe(command =>
                {
                    //if (command == Utility.Enums.ProcessState.Blocked)

                    //else if (command == Utility.Enums.ProcessState.Ready)
                    //{
                    //    _backgroundWorker.RunWorkerAsync(wa);
                    //}
                    //else if (command == Utility.Enums.ProcessState.Running)

                    if (command == ProcessState.Terminated)
                        Cancel();
                    else
                        throw new ArgumentOutOfRangeException("argument should be nullable bool");
                });


            async Task<bool> StartAsync(IWorkerItem workerItem)
            {
                return await _block.SendAsync(() => workerItem.Start());
            }
        }

        //public void OnNext(IWorkerItem taskItem)
        //{
        //    taskItemsSubject.OnNext(taskItem);
        //}

        public IEnumerable<IObservable<IWorkerItem>> Observable { get => new[] { taskItemsSubject }; set => value.ToObservable().SelectMany(a => a).Subscribe(taskItemsSubject); }

        public IDisposable Subscribe(IObserver<ITaskOutput> observer)
        {
            return output.Subscribe(observer);
        }
    }

    public class BaseDataFlowQueue : IObservable<IProgressState>, IObserver<ProcessState>
    {
        protected readonly ReplaySubject<IProgressState> progressStateSubject = new();
        protected readonly CancellationTokenSource _tokenSource;
        protected readonly ISubject<ProcessState> commands = new Subject<ProcessState>();

        public BaseDataFlowQueue(CancellationTokenSource tokensource = default)
        {
            _tokenSource = tokensource ?? new CancellationTokenSource();
        }

        protected void Cancel()
        {
            _tokenSource.Cancel();
            // _block.Complete();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(ProcessState value)
        {
            commands.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<IProgressState> observer)
        {
            return progressStateSubject.Subscribe(observer);
        }
    }

}


