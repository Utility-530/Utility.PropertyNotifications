using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using static Utility.Simulation.ViewModel.PlayerViewModel;
using static Utility.Simulation.ViewModel.RateViewModel;

namespace Utility.Simulation.Service
{
    public class PlayerEngineService : IObserver<CommandType>, IObserver<Rate>, IObservable<Engine.Instruction>
    {
        private readonly Subject<CommandType> commands = new();
        private readonly Subject<Rate> rates = new();
        private readonly Subject<Engine.Instruction> subject = new();

        public PlayerEngineService()
        {
            commands
                .CombineLatest(rates)
                .Select(commandType =>
               {
                   return commandType switch
                   {
                       (CommandType.Play, Rate { Value: { } value }) => new Engine.Instruction(Rate: value),
                       (CommandType.Pause, _) => new Engine.Instruction(Rate: 0),
                       (CommandType.Stop, _) => new Engine.Instruction(Rate: 0, Skip: new Skip(End.Start)),
                       (CommandType.Forward, _) => new Engine.Instruction(Rate: 0, Skip: new Skip(1)),
                       (CommandType.Repeat, _) => throw new Exception("Fsdsdsdf Repeat"),
                       _ => throw new Exception("YYYYdf"),
                   };
               }).Subscribe(a => subject.OnNext(a));
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(CommandType value)
        {
            commands.OnNext(value);
        }

        public void OnNext(Rate value)
        {
            rates.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<Engine.Instruction> observer)
        {
            return subject.Subscribe(observer);
        }
    }
}
