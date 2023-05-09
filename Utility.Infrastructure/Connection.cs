using DryIoc;
using Jellyfish;
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

    public record BroadcastEvent(object Source, object Target) : Event(Source);
    public record BroadcastSuccessEvent(object Source, object Target) : BroadcastEvent(Source, Target);
    public record BroadcastFailureEvent(object Source, object Target) : BroadcastEvent(Source, Target);

    public class Connection<TObserver> : BaseViewModel, IConnection where TObserver : IObserver
    {
        private IContainer container;
        private bool isPriority = true;
        private bool skipContext = true;

        public Connection(IContainer container)
        {
            this.container = container;
        }

        public bool IsPriority { get => isPriority; set => Set(ref isPriority, value); }

        public bool SkipContext { get => skipContext; set => Set(ref skipContext, value); }

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
