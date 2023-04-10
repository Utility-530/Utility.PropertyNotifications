using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace UtilityWpf.Service
{
    public class TimerSingleton
    {
        public IConnectableObservable<TimeSpan> Time { get; }
        public IConnectableObservable<DateTime> Date { get; }

        private static TimerSingleton? instance;

        private TimerSingleton()
        {
            DateTime d = DateTime.Now;
            var interval = Observable.Interval(TimeSpan.FromSeconds(5))
                            .StartWith(default(long));

            Time = interval
                      .Select(_ => DateTime.Now - d)
                      .ObserveOn(RxApp.MainThreadScheduler)
                      .Publish();

            Date = interval
                        .Select(_ => DateTime.Now)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Publish();
        }

        public static TimerSingleton Instance => instance ??= new TimerSingleton();
    }
}