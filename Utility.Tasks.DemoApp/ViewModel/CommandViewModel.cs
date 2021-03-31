using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using Utility.Infrastructure;

namespace Utility.Tasks.DemoApp.ViewModel
{

    public record LoginDialogCommandViewModelConfiguration(bool CanOpen) : DialogCommandViewModelConfiguration(CanOpen);

    public record LoginRequest(string Key, string UserName, string Password):Request(Key);

    public class LoginDialogViewModel : DialogCommandViewModel<LoginViewModel>
    {
        protected readonly ReplaySubject<LoginRequest> loginRequestSubject = new();

        public LoginDialogViewModel(
            LoginViewModel loginViewModel,
            IObservable<LoginDialogCommandViewModelConfiguration> observable,
            IObservable<CloseRequest> isClosedObservable) :
            base(loginViewModel, observable, isClosedObservable)
        {          
        }

        public IDisposable Subscribe(IObserver<LoginRequest> observer)
        {
            return loginRequestSubject.Subscribe(observer);
        }
    }

    public record DialogCommandViewModelConfiguration(bool CanOpen) : CommandViewModelConfiguration(CanOpen);

    public record CommandViewModelConfiguration(bool CanOpen);

    public class DialogCommandViewModel<T> : DialogCommandViewModel where T : IDialogCommandViewModel
    {
        public DialogCommandViewModel(T viewModel, IObservable<CommandViewModelConfiguration> dialogViewModelConfiguration, IObservable<CloseRequest> isClosedObservable)
            : base(viewModel, dialogViewModelConfiguration.OfType<DialogCommandViewModelConfiguration>(), isClosedObservable)
        {

        }
    }

    public class DialogCommandViewModel : CommandViewModel, IObservable<IDialogCommandViewModel>, IObserver<CommandViewModelConfiguration>, IObserver<CloseRequest>
    {
        protected readonly ReplaySubject<IDialogCommandViewModel> viewModelSubject = new();
        protected readonly ReplaySubject<CloseRequest> isClosedSubject = new();
        protected readonly ReplaySubject<CommandViewModelConfiguration> dialogViewModelSubject;

        public DialogCommandViewModel(
            IDialogCommandViewModel viewModel,
            IObservable<CommandViewModelConfiguration> dialogViewModelConfiguration,
            IObservable<CloseRequest> isClosedObservable)
            : base(viewModel.GetType().Name, Create(out var command, out var subject, viewModel, dialogViewModelConfiguration))
        {
            isClosedObservable.Subscribe(isClosedSubject.OnNext);
            dialogViewModelSubject = subject;
            command.Subscribe(viewModelSubject);

            isClosedSubject.Subscribe(a =>
            {

            });
        }

        static ICommand Create(out ReactiveCommand<Unit, IDialogCommandViewModel> command,
            out ReplaySubject<CommandViewModelConfiguration> dialogViewModelSubject,
          IDialogCommandViewModel viewModel,
          IObservable<CommandViewModelConfiguration> dialogViewModelConfiguration)
        {
            dialogViewModelSubject = new();
            dialogViewModelConfiguration.Subscribe(dialogViewModelSubject);
            command = ReactiveCommand.Create<Unit, IDialogCommandViewModel>(a => viewModel, dialogViewModelConfiguration.Select(a => a.CanOpen));
            return command;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(CommandViewModelConfiguration value)
        {
            dialogViewModelSubject.OnNext(value);
        }

        public void OnNext(CloseRequest value)
        {
            isClosedSubject.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<IDialogCommandViewModel> observer)
        {
            return viewModelSubject.Subscribe(observer);
        }
    }

    public abstract class CommandViewModel
    {
        public CommandViewModel(string key, ICommand command)
        {
            this.Key = key;
            this.Command = command;
        }

        public string Key { get; }

        public ICommand Command { get; }

    }
}