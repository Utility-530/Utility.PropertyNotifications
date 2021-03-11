using DynamicData;
using ReactiveAsyncWorker;
using ReactiveAsyncWorker.Model;
using ReactiveAsyncWorker.ViewModel;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;


namespace ReactiveAsyncWorker.DemoApp.ViewModel
{
    public class DemoTPLViewModel
    {
        public DemoTPLViewModel(
            TPLConfigurationViewModel tplViewModel,
            TPLMainViewModel tplViewModel2,
            TPLOutputViewModel tPLOutputModel)
        {
            ConfigurationViewModel = tplViewModel;
            MainViewModel = tplViewModel2;
            OutputModel = tPLOutputModel;
        }

        public TPLConfigurationViewModel ConfigurationViewModel { get; }

        public TPLMainViewModel MainViewModel { get; }

        public TPLOutputViewModel OutputModel { get; }


        public class TPLConfigurationViewModel : IObservable<TaskItem<StringTaskOutput>>, IObservable<Capacity>
        {
            readonly ReplaySubject<TaskItem<StringTaskOutput>> replaySubject = new ReplaySubject<TaskItem<StringTaskOutput>>();
            readonly ReplaySubject<Capacity> capacitySubject = new ReplaySubject<Capacity>();

            public TPLConfigurationViewModel(IScheduler scheduler)
            {
                StartCommand = ReactiveCommand.Create(() =>
                {
                    var dis = Observable
                        .Interval(TimeSpan.FromSeconds(1))
                        .StartWith(-1)
                        .Take(4)
                        .Select(l => new TaskItem<StringTaskOutput>((l + 1).ToString() + " " + DateTime.Now.ToString("HH:mm:ss"), GetTask((int)l + 1)))
                        .SubscribeOnDispatcher()
                        .Subscribe(a =>
                        {
                            replaySubject.OnNext(a);

                        }, e => Debug.WriteLine(e.Message), () => { });

                }, outputScheduler: scheduler);

                StartManyCommand = ReactiveCommand.Create(() =>
                {
                    var dis = Observable
                        .Interval(TimeSpan.FromSeconds(0.1))
                        .StartWith(-1)
                        .Take(40)
                        .Select(l => new TaskItem<StringTaskOutput>((l + 1).ToString() + " " + DateTime.Now.ToString("HH:mm:ss"), GetTask((int)l + 1)))
                        .SubscribeOnDispatcher()
                        .Subscribe(a =>
                        {
                            replaySubject.OnNext(a);

                        }, e => Debug.WriteLine(e.Message), () => { });

                }, outputScheduler: scheduler);

                CapacityCommand = ReactiveCommand.Create<int, Capacity>(a => new Capacity((uint)a));

                CapacityCommand.Subscribe(capacitySubject.OnNext);

                static Task<StringTaskOutput> GetTask(int i)
                {
                    try
                    {
                        return new Task<StringTaskOutput>(() =>
                        {
                            Thread.Sleep(2000);
                            return new StringTaskOutput(new string(new[] { "task"[i % 4] }));
                        });// client.GetStringAsync("https://msdn.microsoft.com");
                    }
                    catch (Exception ex)
                    {
                        return Task.FromResult(new StringTaskOutput(ex.Message));
                    }
                }
            }

            public ReactiveCommand<Unit, Unit> StartCommand { get; }

            public ReactiveCommand<Unit, Unit> StartManyCommand { get; }

            public ReactiveCommand<int, Capacity> CapacityCommand { get; }


            public IDisposable Subscribe(IObserver<TaskItem<StringTaskOutput>> observer)
            {
                return replaySubject.Subscribe(observer);
            }

            public IDisposable Subscribe(IObserver<Capacity> observer)
            {
                return capacitySubject.Subscribe(observer.OnNext);
            }
        }


        public class TPLOutputViewModel
        {
            private readonly ReadOnlyObservableCollection<string> collection;

            public TPLOutputViewModel(IObservable<StringTaskOutput> observable, IScheduler scheduler)
            {
                observable
                    .Select(a => a.Value)
                    .ToObservableChangeSet()
                    .ObserveOn(scheduler)
                    .SubscribeOn(scheduler)
                    .Bind(out collection)
                    .Subscribe();
            }

            public ReadOnlyObservableCollection<string> Collection => collection;
        }


        public class TPLMainViewModel
        {
            public TPLMainViewModel(
                        IObservable<Capacity> capacityObservable,
                        LimitCollection<ProgressState> basicCollection,
                        MultiTaskViewModel service)
            {

                Collection = basicCollection;

                capacityObservable.Subscribe(basicCollection);

                Service = service;
            }

            public MultiTaskViewModel Service { get; }

            public LimitCollection Collection { get; }
        }
    }

}
