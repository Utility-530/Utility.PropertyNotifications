using DryIoc;
using System;
using Utility.Interfaces.NonGeneric;
using Utility.Models;

namespace Utility.Infrastructure
{
    public class Outputs
    {
        private IConnection[] connections;

        public Outputs(Key key, IConnection[] observers)
        {
            Key = key;
            this.connections = observers;
        }

        public Key Key { get; }

        public IConnection[] Connections => connections;

        public bool Match(Key key)
        { 
            return Key.Type.IsAssignableFrom(key.Type);
        }
    }

    public record Event(object Source);

    public record InitialisedEvent(object Source) : Event(Source);

    public record BroadcastEvent(object Source, bool Success) : Event(Source);

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
