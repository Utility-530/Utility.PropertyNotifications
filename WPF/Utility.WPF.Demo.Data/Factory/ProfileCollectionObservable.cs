using Endless;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Threading;
using Utility.WPF.Demo.Data.Model;

namespace Utility.WPF.Demo.Data.Factory
{
    public sealed class ProfileCollectionObservable : IObservable<Profile>, IDisposable
    {
        private readonly Dispatcher dispatcher;
        private readonly int startCount;
        private readonly int speed;
        private IDisposable? observable;
        private readonly Lazy<ReadOnlyCollection<Profile>> pool = new(() => new(ProfileFactory.BuildPool()));
        private ReplaySubject<Profile> profiles = new();

        public ProfileCollectionObservable(int startCount = 0, int speed = 1)
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            this.startCount = startCount;
            this.speed = speed;
        }

        public IDisposable Subscribe(IObserver<Profile> observer)
        {
            observable ??= Observable
              .Interval(TimeSpan.FromSeconds(speed))
              .StartWith(Enumerable.Repeat(0L, startCount).ToArray())
              .Select(a => pool.Value.Random())
              .Subscribe(profiles);

            return profiles
                .Subscribe(a => dispatcher?.Invoke(() => observer.OnNext(a)));
        }

        public void Dispose()
        {
            observable?.Dispose();
        }
    }
}