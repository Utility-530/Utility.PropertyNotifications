using System.Reactive.Linq;
using System.Reflection;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Observables.Generic;
using Utility.Observables.NonGeneric;
using LanguageExt;
using Utility.Helpers;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Utility.ViewModels.Base;

namespace Utility.Infrastructure
{
    public record GuidValue(IGuid Value)
    {
        public GuidValue(IGuid Value, GuidValue? Previous = default) : this(Value)
        {
            this.Previous = Previous;
        }

        public Guid Source
        {
            get
            {
                GuidValue previous = this;
                while (previous.Previous != default)
                {
                    previous = previous.Previous;
                }
                return previous.Target;
            }
        }

        public Guid Target => Value.Guid;

        public GuidValue? Previous { get; }
    }

    public record SubjectKey(Guid Source, Guid Target, Guid Node);

    public abstract class BaseObject : BaseViewModel, IKey<Key> 
    {
        public static IResolver Resolver { get; set; }

        private static SynchronizationContext? context;
        public static SynchronizationContext Context { get => context ?? throw new Exception("Mising Context"); set => context = value; }

        public BaseObject()
        {
        }

        public abstract Key Key { get; }

        public virtual object Model { get; }

        public bool Equals(IKey<Key>? other)
        {
            return other?.Key.Equals(Key) ?? false;
        }

        public bool Equals(IEquatable? other)
        {
            return (other as IKey<Key>)?.Equals(this.Key) ?? false;
        }

        protected Utility.Interfaces.Generic.IObservable<TOutput> Observe<TOutput, TInput>(TInput tInput, [CallerMemberName] string? callerMemberName = null) where TInput : IGuid
        {
            return Resolver.Register<TInput, TOutput>(this.Key, tInput);
        }

        protected void Send(IGuid guid, [CallerMemberName] string? callerMemberName = null)
        {
            if (guid is not GuidValue { Value: var iguid, Target: Guid target, Source: Guid source } guidValue)
            {
                guidValue = new GuidValue(guid, new GuidValue(this.Key));
                source = guidValue.Source;
                target = guidValue.Target;
            }

            Resolver.Send(guidValue);
        }

        public override string ToString()
        {
            return Key.Name;
        }

        protected void Dispatch(Action action)
        {
            (Context ?? throw new Exception("missing context"))
                .Post(a =>
                {
                    action();
                }, default);
        }
    }

    public class InvokeOne
    {
        private readonly MethodInfo methodInfo;
        private readonly object instance;

        public InvokeOne(MethodInfo methodInfo, object instance)
        {
            this.methodInfo = methodInfo;
            this.instance = instance;
        }

        public MethodInfo MethodInfo => methodInfo;
        public object Instance => instance;

        public Type InType => MethodInfo.GetParameters().Single().ParameterType;
        public Type OutType => MethodInfo.ReturnType;

        public object? _(object value)
        {
            return methodInfo.Invoke(instance, new[] { value });
        }
    }

    public static class ReflectionHelper
    {
        public static IEnumerable<SingleParameterMethod> GetSingleParameterMethods(this IKey<Key> instance, IResolver resolver)
        {
            return instance.GetType()
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(m => m.Name == nameof(IObserver.OnNext) && m.GetParameters().Length == 1)
                    .Select(m => new SingleParameterMethod(resolver, instance.Key, new InvokeOne(m, instance)));
        }

        public static IDisposable? TrySubscribe(object instance, Action<object> action, Action<Exception> onError, Action onCompleted, Action<int, int> onProgress)
        {
            var methods = instance
                            .GetType()
                            .GetMethods();

            var single = methods
                            .SingleOrDefault(m => m.Name == nameof(IObservable.Subscribe));

            // if return type is not an observable
            if (single == default)
            {

                if (instance.TryGetPrivateFieldValue("_source", out var source))
                    return TrySubscribe(source, action, onError, onCompleted, onProgress);
                else if (instance.TryGetPrivateFieldValue("_value", out var value))
                {
                    action.Invoke(value);
                    return default;
                }
                throw new Exception("DSVds sss");
            }

            var arg = instance.GetType().GetGenericArguments().SingleOrDefault();
            if (arg != null)
            {
                var observer = Activator.CreateInstance(
                            typeof(Observer<>).MakeGenericType(arg), action, onError, onCompleted, onProgress);

                return (IDisposable?)single.Invoke(instance, new[] { observer });
            }
            else
            {
                return (IDisposable?)single.Invoke(instance, new[] { new Observer(action, onError, onCompleted, onProgress) });
            }

            throw new Exception("D5 666SVds sss");
        }
    }
    public interface IIOType
    {
        Type InType { get; }
        Type OutType { get; }
    }

    public interface IObserverIOType : IIOType
    {
        public Key Key { get; }
        bool Unlock(GuidValue guidValue);
        void Send(GuidValue guidValue);
    }

    public class SingleParameterMethod : IObserverIOType
    {
        private readonly InvokeOne methodInfo;
        private readonly IResolver resolver;

        public SingleParameterMethod(IResolver resolver, Key key, InvokeOne methodInfo)
        {
            this.methodInfo = methodInfo;
            InType = methodInfo.InType;
            OutType = GetOutType();
            this.resolver = resolver;
            Key = new Key(key.Guid, InType.Name, InType);
        }

        public Key Key { get; }

        public Type InType { get; }
        
        public Type OutType { get; }

        public List<IDisposable> disposables { get; } = new();

        public ObservableCollection<object> Outputs { get; } = new();

        public void Send(GuidValue parameter)
        {
            IDisposable? disposable = null;
            object? output;

            var guidValue = new GuidValue(Key, parameter);
            try
            {
                output = methodInfo._(parameter.Value);

                if(output is IGuid guid)
                {
                    var value = new GuidValue(guid, guidValue);
                    Outputs.Add(value);
                    resolver.Send(value);
                    resolver.Send(new GuidValue(GuidBase.OnCompleted(GetGuid()), guidValue));
                    return;
                }

                if (methodInfo.OutType == typeof(void))
                    return;
            }
            catch (Exception ex)
            {
                resolver.Send(new GuidValue(GuidBase.OnError(GetGuid(), ex), guidValue));
                return;
            }

            if (ReflectionHelper.TrySubscribe(output, a =>
            {
                if (a is not IGuid guid)
                    throw new Exception("6 dfdfff444");
                var value = new GuidValue(guid, guidValue);
                Outputs.Add(value);
                resolver.Send(value);
            },
            e => resolver.Send(new GuidValue(GuidBase.OnError(GetGuid(), e), guidValue)),
            () =>
            {

                if (disposable is not null)
                {
                    disposable?.Dispose();
                    disposables.Remove(disposable);
                }
                resolver.Send(new GuidValue(GuidBase.OnCompleted(GetGuid()), guidValue));
            },
            (a, b) => resolver.Send(new GuidValue(GuidBase.OnProgress(GetGuid(), a, b), guidValue)))
                is IDisposable _disposable)
            {
                disposable = _disposable;
                disposables.Add(disposable);
            }
            else
            {
                if (output is not IGuid guid)
                    throw new Exception(" dfdfff444");
                resolver.Send(new GuidValue(guid, guidValue));
            }


        }

        Type GetOutType()
        {
            if (methodInfo.OutType.GetGenericArguments().SingleOrDefault() is Type type)
                return type;
            else
                return methodInfo.OutType;
        }


        Guid GetGuid()
        {
            return GetOutType().GUID;
        }

        public void OnProgress(int arg1, int arg2)
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

        public bool Equals(IEquatable? other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(SingleParameterMethod? other)
        {
            return other?.InType.Equals(this.InType) ==
                other?.OutType.Equals(this.OutType) == true;
        }


        public override string ToString()
        {
            return InType.Name.ToString() + " " + OutType.Name.ToString();
        }

        //public IDisposable Subscribe(IObserver<GuidValue> observer)
        //{
        //    return new Disposer<GuidValue>(observers, observer);
        //}

        public static Guid Combine(Guid guid1, Guid guid2)
        {
            const int BYTECOUNT = 16;
            byte[] destByte = new byte[BYTECOUNT];
            byte[] guid1Byte = guid1.ToByteArray();
            byte[] guid2Byte = guid2.ToByteArray();

            for (int i = 0; i < BYTECOUNT; i++)
            {
                destByte[i] = (byte)(guid1Byte[i] ^ guid2Byte[i]);
            }
            return new Guid(destByte);
        }

        public bool Unlock(GuidValue guidValue)
        {
            return guidValue.Target == this.InType.GUID;
        }
    }
}