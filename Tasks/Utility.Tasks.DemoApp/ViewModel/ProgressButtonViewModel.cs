using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using Utility.Infrastructure;
using Utility.Interface;
using Utility.Models;

namespace Utility.Tasks.DemoApp.ViewModel
{
    public class ProgressButtonViewModel<T, TInner> : ReactiveObject, IObservable<Request>,
        IObserver<ITaskOutput<TInner>>, IDialogCommandViewModel, IActivatableViewModel
        where T : ITaskOutput<TInner>
        where TInner : IResult
    {
        private readonly ReplaySubject<Request> loginRequest = new();
        private readonly ReplaySubject<ITaskOutput<TInner>> taskOutputs = new();
        private readonly ObservableAsPropertyHelper<bool?> success;
        private readonly ReactiveCommand<Unit, Unit> check;
        private readonly Subject<IsCompleteEvent?> successes = new();
        private readonly ObservableAsPropertyHelper<bool> isBusy;

        public ProgressButtonViewModel(string key, IObservable<ITaskOutput<TInner>> observable, IScheduler scheduler)
        {
            observable.Subscribe(taskOutputs);

            taskOutputs.OfType<T>()
                .ObserveOn(scheduler)
                .Where(a => a.Key == key)
                .Select(a => (bool?)a.Value.Result)
                .Merge(Activator.Activated.Select(a => (bool?)null))
                .Merge(Activator.Deactivated.Select(a => (bool?)null))
                .Subscribe(
                a => successes.OnNext(new IsCompleteEvent(key, a)),
                e => { },
                () => { });
            //.Publish()
            //.RefCount();

            successes.Subscribe(a =>
            {

            });

            success = successes.Select(a => a.Value).ToProperty(this, a => a.Success);

            success.ThrownExceptions.Subscribe(a =>
            {

            });

            check = ReactiveCommand.Create(() => { });

            //check
            //    .Select(_ => new LoginRequest(userName, passWord))
            //    .Subscribe(loginRequest.OnNext);

            isBusy = successes.Select(a => false)
                .Merge(check.Select(a => true))
                .ToProperty(this, c => c.IsBusy);
        }

        //public string UserName { get => userName; set => this.RaiseAndSetIfChanged(ref userName, value); }

        //public string PassWord { get => passWord; set => this.RaiseAndSetIfChanged(ref passWord, value); }

        public ICommand Check => check;

        public bool? Success => success.Value;

        public bool IsBusy => isBusy.Value;

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public void OnCompleted() => throw new NotImplementedException();
        public void OnError(Exception error) => throw new NotImplementedException();
        public void OnNext(ITaskOutput<TInner> value) => taskOutputs.OnNext(value);

        public IDisposable Subscribe(IObserver<Request> observer)
        {
            return loginRequest.Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<CloseRequest> observer)
        {
            return successes.Where(a => a.Value == true).Select(a => new CloseRequest(a.Key, true)).Subscribe(observer.OnNext);
        }
    }
}
