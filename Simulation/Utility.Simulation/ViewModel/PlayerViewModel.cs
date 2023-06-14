using System;
using System.Data;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows.Input;
using ReactiveUI;
using Utility.Simulation.Service;

namespace Utility.Simulation.ViewModel
{
    public enum PlayerState
    {
        Playing, Stopped
    }

    public record Command(CommandType Type);
    public enum CommandType
    {
        Play, Pause, Repeat, Backward, Forward, Stop
    }
    public partial class PlayerViewModel : ReactiveObject, IObservable<Command>, IObserver<Engine.Position>
    {
        readonly Subject<Command> subject = new();
        readonly Subject<Engine.Position> positionSubject = new();
        private PlayerState state = PlayerState.Stopped;



        public PlayerViewModel(SynchronizationContext context)
        {
            var playCommand = new Jellyfish.RelayCommand((a) => subject.OnNext(new Command(CommandType.Play)));
            var pauseCommand = new Jellyfish.RelayCommand((a) => subject.OnNext(new Command(CommandType.Pause)));
            var stopCommand = new Jellyfish.RelayCommand((a) => subject.OnNext(new Command(CommandType.Stop)));
            var forwardCommand = new Jellyfish.RelayCommand((a) => subject.OnNext(new Command(CommandType.Forward)));

            //var repeatCommand = ReactiveCommand.Create(() => CommandType.Repeat);
            //var backCommand = ReactiveCommand.Create(() => CommandType.Backward, Observable.Return(false));

            //playCommand
            //    .Merge(pauseCommand)
            //    .Merge(forwardCommand)
            //    .Merge(stopCommand)
            //    .Subscribe(a =>
            //    {
            //        context.Send(add =>
            //        {
            //            subject.OnNext(a);
            //        }, null);
            //    });
            //.Merge(repeatCommand)
            //.Merge(backCommand)

            PlayCommand = playCommand;
            PauseCommand = pauseCommand;
            StopCommand = stopCommand;
            PlayCommand = playCommand;
            ForwardCommand = forwardCommand;

            _ = positionSubject
                .SubscribeOn(context)
                .Subscribe(a =>
                {
                    var state = a.IsRunning ? PlayerState.Playing : PlayerState.Stopped;
                    if (state != State)
                        State = state;

                });
        }

        public ICommand PlayCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand ForwardCommand { get; }
        public ICommand StopCommand { get; }

        //public ICommand RepeatCommand { get; }
        //public ICommand BackCommand { get; }

        public PlayerState State { get => state; private set => this.RaiseAndSetIfChanged(ref state, value); }

        public IDisposable Subscribe(IObserver<Command> observer)
        {
            return subject.Subscribe(observer);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Engine.Position value)
        {
            positionSubject.OnNext(value);
        }
    }
}
