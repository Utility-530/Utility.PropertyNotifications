using System;
using System.Threading;
using System.Windows.Input;
using System.Reactive.Linq;
using ReactiveAsyncWorker;
using System.Reactive.Subjects;
using System.Reactive.Concurrency;
using ReactiveUI;

namespace DemoApp
{


    public class MainViewModel
    {
        ISubject<int> _itemsCount = new Subject<int>();

        public MainViewModel()
        {
            var scheduler = new SynchronizationContextScheduler(SynchronizationContext.Current);
            StartCommand = ReactiveCommand.Create(() => _itemsCount.OnNext(5), outputScheduler: scheduler);

            //Service = new BackgroundWorkerQueue(new SynchronizationContextScheduler(SynchronizationContext.Current));

            Helper.GetNewItems(_itemsCount)
                .Switch()
                .Subscribe(newItem =>
                {
                   // Service.OnNext(newItem);
                });

            //  RestartCommand.Execute(5);            
        }

        public ICommand StartCommand { get; }

      //  public BackgroundWorkerQueue Service { get; }

        class Helper
        {
            public static IObservable<IObservable<WorkerArgument<object>>> GetNewItems(IObservable<int> itemcounts)
            {
                Func<WorkerArgument<object>> func = () => new WorkerArgument<object>
                {
                    Iterations = 4,
                    Delay = 500,
                    MethodContainer = new DummyMethodContainer(),
                    Timeout = TimeSpan.FromSeconds(4),
                    Key = Guid.NewGuid().ToString().Remove(8)
                };

                return itemcounts.Select(_ => Observable.Generate(0, value => value < _, value => value + 1, value => func(), i => TimeSpan.FromSeconds(i)));
            }
        }
    }
}
