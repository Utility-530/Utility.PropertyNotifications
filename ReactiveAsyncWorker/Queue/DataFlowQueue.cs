
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Reactive.Subjects;
using ReactiveAsyncWorker.Model;
using UtilityEnum;

namespace ReactiveAsyncWorker
{
    public class StringDataFlowQueue : DataFlowQueue<StringTaskOutput>
    {
        public StringDataFlowQueue(
            IObservable<TaskItem<StringTaskOutput>> taskItems,
            IBasicObservableCollection<ProgressState> coll,
            IFactory<IWorkerItem<StringTaskOutput>, TaskItem<StringTaskOutput>> factory,
            ISchedulerWrapper schedulerWrapper,
            IObservable<ProcessState> processStates = null,
            CancellationTokenSource tokenSource = null) :
            base(taskItems, coll, factory, schedulerWrapper, processStates, tokenSource)
        {
        }
    }


    public class DataFlowQueue<T> : DataFlowQueue, IObserver<TaskItem<T>>, IObservable<T>
        where T : TaskOutput
    {
        private readonly ISubject<TaskItem<T>> taskItemsSubject = new ReplaySubject<TaskItem<T>>();
        private readonly ISubject<T> outPut = new ReplaySubject<T>();
        private readonly ActionBlock<Action> _block;

        public DataFlowQueue(
             IObservable<TaskItem<T>> taskItems,
             IBasicObservableCollection<ProgressState> basicCollection,
             IFactory<IWorkerItem<T>, TaskItem<T>> factory,
             ISchedulerWrapper schedulerWrapper,
             IObservable<ProcessState> processStates = default,             
             CancellationTokenSource tokenSource = default) : base(tokenSource)
        {
            tokenSource ??= new CancellationTokenSource();
            taskItems.Subscribe(this.taskItemsSubject.OnNext);
            processStates?.Subscribe(commands.OnNext);

            outPut.Subscribe(a =>
            {

            });

            progressStateSubject.Where(a=> a.State ==ProcessState.Terminated)
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
                    if (a.State == ProcessState.Terminated)
                        basicCollection.Remove(a);
                });

            var workerItemsObservable = taskItems?
                 .Select(t => factory.Create(t))
                 .DistinctUntilChanged(a => a.Key)
                 .SubscribeOn(schedulerWrapper.Scheduler)
                 .ObserveOn(schedulerWrapper.Scheduler)
                 .Do(workerItem =>
                 {
                     workerItem.Subscribe(a =>  progressStateSubject.OnNext(a));
                     workerItem.StateChanges.Subscribe(outPut.OnNext);
                 });


            workerItemsObservable.Zip(basicCollection, (a, b) => a)
                 .Subscribe(async item => await Start(item),
                 e =>
                 {
                 });

            commands
                .Subscribe(command =>
                {
                    //if (command == UtilityEnum.ProcessState.Blocked)

                    //else if (command == UtilityEnum.ProcessState.Ready)
                    //{
                    //    _backgroundWorker.RunWorkerAsync(wa);
                    //}
                    //else if (command == UtilityEnum.ProcessState.Running)

                    if (command == ProcessState.Terminated)
                        Cancel();
                    else
                        throw new ArgumentOutOfRangeException("argument should be nullable bool");
                });


            async Task<bool> Start(IWorkerItem<T> workerItem)
            {
                return await _block.SendAsync(() => workerItem.Start());
            }
        }

        public void OnNext(TaskItem<T> taskItem)
        {
            taskItemsSubject.OnNext(taskItem);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return outPut.Subscribe(observer);
        }
    }

    public class DataFlowQueue : IObservable<ProgressState>, IObserver<ProcessState>
    {
        protected readonly ISubject<ProgressState> progressStateSubject = new ReplaySubject<ProgressState>();
        protected readonly CancellationTokenSource _tokenSource;
        protected readonly ISubject<ProcessState> commands = new Subject<ProcessState>();

        public DataFlowQueue(CancellationTokenSource tokensource = default)
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

        public IDisposable Subscribe(IObserver<ProgressState> observer)
        {
            return progressStateSubject.Subscribe(observer);
        }
    }

}


