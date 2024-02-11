using System;
using System.Reactive.Subjects;
using Utility.Infrastructure;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes.Demo
{
    public class EventListener : IEventListener, IObservable<object>
    {
        ReplaySubject<object> replay = new();

        private EventListener()
        {
        }

        public void Send(IEvent @event)
        {
            if (@event is DoubleClickChange clickChange)
            {
                replay.OnNext(clickChange.Node.Data);
            }
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return replay.Subscribe(observer);
        }

        public static EventListener Instance { get; } = new();
    }
}
