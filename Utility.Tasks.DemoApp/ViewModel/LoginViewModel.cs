using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using Utility.Infrastructure;

namespace Utility.Tasks.DemoApp.ViewModel
{
    public class LoginViewModel : ReactiveObject, IObservable<LoginRequest>, IObserver<ITaskOutput>, IDialogCommandViewModel, IActivatableViewModel
    {
        private readonly ReplaySubject<LoginRequest> loginRequest = new();
        private readonly ReplaySubject<ITaskOutput> taskOutputs = new();
        private readonly ObservableAsPropertyHelper<bool?> success;
        private readonly ReactiveCommand<Unit, Unit> check;
        private readonly Subject<bool?> successes = new();
        private readonly ObservableAsPropertyHelper<bool> isBusy;
        private readonly string key = "Login";
        private string passWord = "Gsddf";
        private string userName = "Usgdfd";

        public LoginViewModel( IObservable<ITaskOutput> observable, IScheduler scheduler)
        {
            observable.Subscribe(taskOutputs);

            //taskOutputs.Subscribe(a =>
            //{
            //});

            taskOutputs.OfType<UserAuthenticationTaskOutput>()
                .ObserveOn(scheduler)
                .Where(a => a.Value.UserName == userName)
                .Select(a => (bool?)a.Value.Result)
                .Merge(Activator.Activated.Select(a => (bool?)null))
                .Merge(Activator.Deactivated.Select(a => (bool?)null))
                .Subscribe(
                a => successes.OnNext(a),
                e => { },
                () => { });
            //.Publish()
            //.RefCount();

            successes.Subscribe(a =>
            {

            });

            success = successes.Select(a => a).ToProperty(this, a => a.Success);

            success.ThrownExceptions.Subscribe(a =>
            {

            });

            check = ReactiveCommand.Create(() => { });

            check
                .Select(_ => new LoginRequest(key, userName, passWord))
                .Subscribe(loginRequest.OnNext);

            isBusy = successes.Select(a => false)
                .Merge(check.Select(a => true))
                .ToProperty(this, c => c.IsBusy);
            this.key = key;
        }

        public string UserName { get => userName; set => this.RaiseAndSetIfChanged(ref userName, value); }

        public string PassWord { get => passWord; set => this.RaiseAndSetIfChanged(ref passWord, value); }

        public ICommand Check => check;

        public bool? Success => success.Value;

        public bool IsBusy => isBusy.Value;

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public void OnCompleted() => throw new NotImplementedException();
        public void OnError(Exception error) => throw new NotImplementedException();
        public void OnNext(ITaskOutput value) => taskOutputs.OnNext(value);

        public IDisposable Subscribe(IObserver<LoginRequest> observer)
        {
            return loginRequest.Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<CloseRequest> observer)
        {
            return successes.Where(a => a == true).Select(a => new CloseRequest(key, a.Value)).Subscribe(observer.OnNext);
        }
    }
}
