using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using ReactiveUI;
using Utility.Simulation.Service;

namespace Utility.Simulation.ViewModel
{
    public class ProgressViewModel : ReactiveObject, IObserver<Engine.Position>
    {
        private readonly Subject<Engine.Position> subject = new();
        private double value;

        public ProgressViewModel(SynchronizationContext scheduler, DateTime start, DateTime end)
        {
            Minimum = (start - DateTime.UnixEpoch).TotalDays;
            Maximum = (end - DateTime.UnixEpoch).TotalDays;

            subject
                .SubscribeOn(scheduler)
                .Subscribe(value =>
            {
                var diff = (value.Current - DateTime.UnixEpoch).TotalDays;
                if (diff != this.value)
                    Value = diff;
            });
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
            subject.OnNext(value);
        }

        public double Minimum { get; }

        public double Maximum { get; }

        public double Value { get => value; private set => this.RaiseAndSetIfChanged(ref this.value, value); }
    }
}
