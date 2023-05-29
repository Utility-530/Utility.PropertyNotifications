using System;
using System.Linq;
using ReactiveUI;
using System.Reactive.Linq;

namespace Utility.Simulation.ViewModel
{
    public class RateViewModel : ReactiveObject, IObservable<RateViewModel.Rate>
    {
        public record Rate(double Value);

        private double value;

        public RateViewModel(double minimum, double maximum, double value)
        {
            Minimum = minimum;
            Maximum = maximum;
            Value = value;
        }

        public double Value { get => value; set => this.RaiseAndSetIfChanged(ref this.value, value); }

        public double Minimum { get; }

        public double Maximum { get; }

        public IDisposable Subscribe(IObserver<Rate> observer)
        {
            return this.WhenAnyValue(a => a.Value)
                .Buffer(TimeSpan.FromMilliseconds(300))
                .Select(a => a.LastOrDefault())
                .Where(a => a > 0)
                .Select(a => new Rate(Math.Pow(10, a))).Subscribe(observer);
        }
    }
}
