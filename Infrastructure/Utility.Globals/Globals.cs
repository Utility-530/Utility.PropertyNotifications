using System.Reactive.Linq;
using System.Reactive.Subjects;
using Utility.Entities;

namespace Utility
{
    public class Globals
    {
        private static readonly SynchronizationContext ui;

        static Globals()
        {
            DateTime d = DateTime.Now;
            var interval = Observable.Interval(TimeSpan.FromSeconds(5))
                            .StartWith(default(long));
            Time = interval
                      .Select(_ => DateTime.Now - d)
                      //.ObserveOn()
                      .Publish();

            Date = interval
                        .Select(_ => DateTime.Now)                        
                        .Publish();

            ui = SynchronizationContext.Current ?? throw new Exception($"Expected {nameof(SynchronizationContext)}!");
        }


        public static SynchronizationContext UI => ui;        

        public static ReplaySubject<Log> Logs { get; } = new();

        public static IConnectableObservable<TimeSpan> Time { get; }
        public static IConnectableObservable<DateTime> Date { get; }

    }
}
