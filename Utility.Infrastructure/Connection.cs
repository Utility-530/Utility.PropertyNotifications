using DryIoc;
using System;
using Utility.Interfaces.NonGeneric;
using Utility.Models;

namespace Utility.Infrastructure
{
    public class Outputs
    {
        private IConnection[] connections;

        public Outputs(Predicate<Key> predicate, IConnection[] observers)
        {
            Predicate = predicate;
            this.connections = observers;
        }

        public Predicate<Key> Predicate { get; }

        public IConnection[] Connections => connections;

        public bool Match(IBase @base, IObserver observer)
        {
            if (Predicate(@base.Key))
                return Connections.Any(a => a.Equals(observer));
            return false;
        }
    }

    public record InitialisedEvent(object Source);

    public record BroadcastEvent(object Source);

    public class Connection<TObserver> : IConnection where TObserver : IObserver
    {
        private IContainer container;

        public Connection(IContainer container)
        {
            this.container = container;
        }

        public bool IsPriority { get; set; } = true;
        public bool SkipContext { get; set; } = true;
        public IEnumerable<IObserver> Observers => container.ResolveMany<TObserver>().Cast<IObserver>();

        public override string ToString()
        {
            return typeof(TObserver).Name;
        }
    }

    public interface IConnection
    {

        public bool IsPriority { get; set; }

        public bool SkipContext { get; set; }

        public IEnumerable<IObserver> Observers { get; }
    }
}
