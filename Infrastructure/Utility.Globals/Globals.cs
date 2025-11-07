using System.Reactive.Linq;
using System.Reactive.Subjects;
using Utility.Entities;
using Utility.Interfaces.NonGeneric.Dependencies;
using Utility.ServiceLocation;

namespace Utility
{
    public class Globals
    {
        private static readonly SynchronizationContext ui;
        private static Store store = new();

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
        public static ReplaySubject<Exception> Exceptions { get; } = new();
        public static ReplaySubject<Entities.Comms.Event> Events { get; } = new();

        public static IConnectableObservable<TimeSpan> Time { get; }
        public static IConnectableObservable<DateTime> Date { get; }

        public static IResolver Resolver => store;
        public static IRegister Register => store;

        public static Type[] Types { get; } = [.. System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())];
    }
}