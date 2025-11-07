using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace Utility
{
    public class SynchronizationContextScheduler(SynchronizationContext context) : IScheduler
    {
        private readonly SynchronizationContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public DateTimeOffset Now => DateTimeOffset.Now;

        public IDisposable Schedule<TState>(
            TState state, Func<IScheduler, TState, IDisposable> action)
        {
            var d = new SingleAssignmentDisposable();
            _context.Post(_ => d.Disposable = action(this, state), null);
            return d;
        }

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            var timer = new Timer(_ =>
            {
                _context.Post(_ => action(this, state), null);
            }, null, dueTime.Millisecond, Timeout.Infinite);

            return Disposable.Create(() => timer.Dispose());
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            var timer = new Timer(_ =>
            {
                _context.Post(_ => action(this, state), null);
            }, null, dueTime.Milliseconds, Timeout.Infinite);

            return Disposable.Create(() => timer.Dispose());
        }

        public static SynchronizationContextScheduler Instance => new(SynchronizationContext.Current);
    }
}