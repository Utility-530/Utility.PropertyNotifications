using System.Reactive.Subjects;
using Utility.Interfaces.Generic;

namespace Utility.Models
{
    public interface INode : IKey<Key>, ISubject<Message>
    {
        List<IObserver<Message>> Observers { get; }
    }

    public static class NodeHelper
    {
        public static void Broadcast(this INode node, Message message)
        {
            foreach (var observer in node.Observers.ToArray())
                observer.OnNext(message);
        }
    }
}