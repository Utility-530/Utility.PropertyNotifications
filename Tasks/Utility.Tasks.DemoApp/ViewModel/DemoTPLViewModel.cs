using DynamicData;
using Utility.Tasks.Model;
using Utility.Tasks.ViewModel;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Utility.Progress;
using Utility.Infrastructure;

namespace Utility.Tasks.DemoApp.ViewModel
{
    public class DemoTPLViewModel
    {
        public DemoTPLViewModel(
            LoginDialogViewModel loginDialogViewModel,
            TPLConfigurationViewModel tplViewModel,
            TPLMainViewModel tplViewModel2,
            TPLOutputViewModel tPLOutputModel,
            MultiProgressViewModel progressViewModel)
        {
            LoginDialogViewModel = loginDialogViewModel;
            ConfigurationViewModel = tplViewModel;
            MainViewModel = tplViewModel2;
            OutputModel = tPLOutputModel;
            ProgressViewModel = progressViewModel;
        }

        public LoginDialogViewModel LoginDialogViewModel { get; }

        public TPLConfigurationViewModel ConfigurationViewModel { get; }

        public TPLMainViewModel MainViewModel { get; }

        public TPLOutputViewModel OutputModel { get; }

        public MultiProgressViewModel ProgressViewModel { get; }

        public class TPLConfigurationViewModel : IObservable<IWorkerItem>, IObservable<Capacity>, IObserver<TaskChangeRequest>
        {
            readonly ReplaySubject<IWorkerItem> replaySubject = new();
            readonly ReplaySubject<Capacity> capacitySubject = new();
            readonly ReplaySubject<TaskChangeRequest> taskChangeSubject = new();
            readonly TimeSpan sleepTime = TimeSpan.FromMilliseconds(1000);
            public TPLConfigurationViewModel(IScheduler scheduler)
            {
                StartCommand = ReactiveCommand.Create(() =>
                {
                    var dis = Observable
                        .Interval(TimeSpan.FromSeconds(1))
                        .StartWith(-1)
                        .Take(4)
                        .Select(l =>
                        {
                            var source = new CancellationTokenSource();

                            int id = (int)l + 1;
                            var key = id.ToString() + " " + DateTime.Now.ToString("HH:mm:ss");
                            return new AsyncWorkerItem(key, taskChangeSubject, GetTask(id, sleepTime, source.Token), sleepTime, source, false);
                        }).SubscribeOnDispatcher()
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
                        .Select(l =>
                        {
                            var source = new CancellationTokenSource();
                            var key = (l + 1).ToString() + " " + DateTime.Now.ToString("HH:mm:ss");
                            return new AsyncWorkerItem(key, taskChangeSubject, GetTask((int)l + 1, sleepTime, source.Token), sleepTime, source);
                        })
                        .SubscribeOnDispatcher()
                        .Subscribe(a =>
                        {
                            replaySubject.OnNext(a);
                        }, e => Debug.WriteLine(e.Message), () => { });

                }, outputScheduler: scheduler);

                CapacityCommand = ReactiveCommand.Create<int, Capacity>(a => new Capacity((uint)a));

                CapacityCommand.Subscribe(capacitySubject.OnNext);

                static Task<ITaskOutput> GetTask(int i, TimeSpan sleepTime, CancellationToken token)
                {
                    var key = new string(new[] { "task"[i % 4] });
                    try
                    {
                        return new Task<ITaskOutput>(() =>
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                //token.ThrowIfCancellationRequested();
                                if (token.IsCancellationRequested)
                                {

                                    return new StringTaskOutput(key, string.Empty, true);
                                }
                                Thread.Sleep(sleepTime);
                            }
                         
                            return new StringTaskOutput(key, key);
                        }, token);// client.GetStringAsync("https://msdn.microsoft.com");
                    }
                    catch (Exception ex)
                    {
                        return Task.FromResult<ITaskOutput>(new StringTaskOutput(key, ex.Message));
                    }
                }
            }

            public ReactiveCommand<Unit, Unit> StartCommand { get; }

            public ReactiveCommand<Unit, Unit> StartManyCommand { get; }

            public ReactiveCommand<int, Capacity> CapacityCommand { get; }

            //public IObservable<TaskChangeRequest> TaskChangeRequests { get => taskChangeSubject; set => value.Subscribe(taskChangeSubject); }

            public void OnCompleted()
            {
                throw new NotImplementedException();
            }

            public void OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            public void OnNext(TaskChangeRequest value)
            {
                taskChangeSubject.OnNext(value);
            }

            public IDisposable Subscribe(IObserver<IWorkerItem> observer)
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

            public TPLOutputViewModel(IObservable<ITaskOutput> observable, IScheduler scheduler)
            {
                observable

                    .Select(a => JsonConvert.SerializeObject(a.Value))
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
                        LimitCollection<IProgressState> basicCollection,
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
