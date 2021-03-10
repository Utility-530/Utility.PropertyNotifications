
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

    public class StringDataFlowQueue : DataFlowQueue<StringTaskOutput>
    {
        public StringDataFlowQueue(
            IObservable<TaskItem<StringTaskOutput>> taskItems, 
            IBasicCollection<ProgressState> coll,
            IFactory<IWorkerItem<StringTaskOutput>, TaskItem<StringTaskOutput>> factory, 
            IObservable<ProcessState> processStates = null, 
            CancellationTokenSource tokenSource = null) : 
            base(taskItems, coll, factory, processStates, tokenSource)
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
             IBasicCollection<ProgressState> coll,
             IFactory<IWorkerItem<T>, TaskItem<T>> factory,
             IObservable<ProcessState> processStates = default,
             CancellationTokenSource tokenSource = default) : base(tokenSource)
        {
            tokenSource ??= new CancellationTokenSource();
            taskItems.Subscribe(this.taskItemsSubject.OnNext);
            processStates?.Subscribe(commands.OnNext);

            _block = new ActionBlock<Action>(act => act(),
                new ExecutionDataflowBlockOptions()
                {
                    TaskScheduler = TaskScheduler.Default,
                    BoundedCapacity = 1, // Cap the item count     
                    CancellationToken = tokenSource.Token, // Enable cancellation
                    MaxDegreeOfParallelism = Environment.ProcessorCount, // Parallelize on all cores
                });

            var dis = taskItems?
                 .Select(t => factory.Create(t))
                 .DistinctUntilChanged(a => a.Key)
                 .Do(workerItem =>
                 {
                     workerItem.StateChanges.Subscribe(outPut.OnNext);
                     workerItem.Subscribe(progressStateSubject.OnNext);
                 })
                 .Zip(coll, (a, b) => a)
                 .Subscribe(async item => await Start(item),
                 e =>
                 {
                 });

            progressStateSubject
                .Distinct(a => (a.Key, a.State))
                .Subscribe(a =>
                {
                    if (a.State == ProcessState.Running)
                        coll.Add(a);
                    if (a.State == ProcessState.Terminated)
                        coll.Remove(a);
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
}


