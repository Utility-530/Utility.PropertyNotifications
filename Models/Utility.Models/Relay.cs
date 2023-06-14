using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Observables.Generic;

namespace Utility.Models
{
    //public abstract class Relay : Relay<Message>, IKey<Key>, IRelay
    //{
    //    public abstract Key Key { get; }

    //    public bool Equals(IKey<Key>? other)
    //    {
    //        return other?.Key == Key;
    //    }
    //    public bool Equals(IEquatable? other)
    //    {
    //        return Equals(other as IKey<Key>);
    //    }
    //}

    //public interface IRelay : IRelay<Message>, IKey<Key>
    //{

    //}


    //public static class RelayHelper
    //{
    //    public static void Broadcast(this IRelay relay, Message message)
    //    {
    //        foreach (var observer in relay.Observers)
    //            observer.OnNext(message);
    //    }
    //}
}