using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Utility.Collections;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Models;

namespace Utility.Infrastructure
{
    public interface IBase : IKey<Key>, IObserver
    {
        object Output { get; }
    }

    public record GuidValue(Guid Guid, object Value, int Remaining);

    public abstract class BaseObject : BaseViewModel, IBase
    {
        private ConcurrentDictionary<Guid, Subject<GuidValue>> dictionary = new();
        private static SynchronizationContext? context;

        public static Resolver Resolver { get; set; }

        public static SynchronizationContext Context { get => context?? throw new Exception("Mising Context"); set => context = value; }

        public abstract Key Key { get; }


        public Collection Errors { get; } = new();


        public bool Equals(IKey<Key>? other)
        {
            return other?.Key.Equals(Key) ?? false;
        }

        public bool Equals(IEquatable? other)
        {
            return (other as IKey<Key>)?.Equals(this.Key) ?? false;
        }


        public virtual object? Output { get; set; }

        protected virtual void Broadcast(object obj)
        {
            Output = obj;
            Resolver.OnNext(this);
        }

        protected IObservable<T> Observe<T, TR>(TR tr)
        {
            var guid = Guid.NewGuid();
            var subject = new Subject<GuidValue>();
            dictionary[guid] = subject;
            Broadcast(new GuidValue(guid, tr, 0));
            var output = new ReplaySubject<T>(1);
            subject.Subscribe(a =>
            {
                switch (a.Value)
                {
                    case nameof(OnCompleted):
                        output.OnCompleted();
                        break;
                    case Exception e:
                        Errors.Add(e);
                        output.OnError(e);
                        output.OnCompleted();
                        break;
                    case T t:
                        output.OnNext(t);
                        break;
                    default:
                        break;
                }
                if (a.Remaining == 0)
                    output.OnCompleted();
            });
            return output;
        }

        public virtual bool OnNext(object next)
        {
            if (next is GuidValue { Guid: var guid } keyType)
            {
                if (dictionary.TryGetValue(guid, out var value))
                {
                    value.OnNext(keyType);
                    return true;
                }
            }
            return false;
        }

        public void OnStarted()
        {
            throw new NotImplementedException();
        }
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Key.Name;
        }
    }
}
