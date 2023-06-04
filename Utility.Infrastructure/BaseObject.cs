using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using Utility.Collections;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Observables.Generic;
using Utility.Observables.NonGeneric;
using LanguageExt;
using Utility.Helpers;

namespace Utility.Infrastructure
{

    public interface IBase : IKey<Key>, System.IObserver<object>
    {
        public object Output { get; set; }
    }

    public record GuidValue(IGuid Value, Guid Node)
    {

        public GuidValue(IGuid Value, Guid Node, GuidValue? Previous = default) : this(Value, Node)
        {
            this.Previous = Previous;
        }

        public Guid Target => Value.Guid;

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

        public int Remaining { get; }

        public bool IsCompleted => Remaining == 0;

        public Exception Exception { get; }
        public GuidValue? Previous { get; }
    }

    public record SubjectKey(Guid Source, Guid Target, Guid Node);

    public abstract class BaseObject : BaseViewModel, IBase
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

        public virtual object? Output { get; set; }

        protected Utility.Interfaces.Generic.IObservable<TOutput> Observe<TOutput, TInput>(TInput tInput) where TInput : IGuid
        {
            return Resolver.Register<TOutput, TInput>(this, tInput);
        }

        public void OnNext(object value)
        {
            Output = value;
            Resolver.OnBase(this);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        //public void OnProgress(int complete, int total)
        //{
        //    throw new NotImplementedException();
        //}
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


    public static class ReflectionHelper
    {
        public static Dictionary<Guid, SingleParameterMethod> GetSingleParameterMethods(this IBase instance)
        {
            return instance.GetType()
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(m => m.Name == nameof(IObserver.OnNext) && m.GetParameters().Length == 1)
                        .ToDictionary(m =>
                        m.GetParameters().Single().ParameterType.GUID,
                        m => new SingleParameterMethod(instance, m));
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

    public interface IObserverIOType : Utility.Interfaces.Generic.IObserver<GuidValue>, IIOType
    {
    }

    public class SingleParameterMethod : IObserverIOType
    {
        private readonly MethodInfo methodInfo;
        private readonly IBase instance;

        public SingleParameterMethod(IBase instance, MethodInfo methodInfo)
        {
            this.methodInfo = methodInfo;
            InType = methodInfo.GetParameters().Single().ParameterType;
            OutType = GetOutType();
            this.instance = instance;
        }

        //public Guid Key => Combine(instance.Key.Guid, InType.GUID);
        public Guid Guid => instance.Key.Guid;

        public Type InType { get; }

        public Type OutType { get; }

        public List<IDisposable> disposables { get; } = new();
        public void OnNext(GuidValue parameter)
        {
            IDisposable? disposable = null;
            object? output;
            try
            {
                output = methodInfo.Invoke(instance, new object[] { parameter.Value });

                if (methodInfo.ReturnType == typeof(void))
                    return;
            }
            catch (Exception ex)
            {
                instance.OnNext(new GuidValue(GuidBase.OnError(GetGuid(), ex), Guid, parameter));
                return;
            }

            if (ReflectionHelper.TrySubscribe(output, a =>
            {
                if (a is not IGuid guid)
                    throw new Exception("6 dfdfff444");
                instance.OnNext(new GuidValue(guid, Guid, parameter));
            },
            e => instance.OnNext(new GuidValue(GuidBase.OnError(GetGuid(), e), Guid, parameter)),
            () => {

                if (disposable is not null)
                {
                    disposable?.Dispose();
                    disposables.Remove(disposable);
                }
                instance.OnNext(new GuidValue(GuidBase.OnCompleted(GetGuid()), Guid, parameter));
            },
            (a, b) => instance.OnNext(new GuidValue(GuidBase.OnProgress(GetGuid(), a, b), Guid, parameter)))
                is IDisposable _disposable)
            {
                disposable = _disposable;
                disposables.Add(disposable);
            }
            else
            {
                if (output is not IGuid guid)
                    throw new Exception(" dfdfff444");
                instance.OnNext(new GuidValue(guid, Guid, parameter));
            }


        }

        Type GetOutType()
        {
            if (methodInfo.ReturnType.GetGenericArguments().SingleOrDefault() is Type type)
                return type;
            else
                return methodInfo.ReturnType;
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
    }

}