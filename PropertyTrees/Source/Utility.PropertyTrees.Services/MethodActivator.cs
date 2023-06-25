using Utility.Models;
using Utility.Infrastructure;
using System.Reactive.Linq;
using System.Collections.Concurrent;
using Utility.Nodes;
using static Utility.Observables.Generic.ObservableExtensions;
using Utility.Observables.NonGeneric;
using Utility.Observables.Generic;
using System.Reactive.Disposables;

namespace Utility.PropertyTrees.Services
{
    public class MethodActivator : BaseObject
    {
        private readonly ConcurrentDictionary<Key, MethodNode> guids = new();

        public override Key Key => new(Guids.MethodActivator, nameof(MethodActivator), typeof(MethodActivator));

        public override object? Model => guids;

        public Interfaces.Generic.IObservable<MethodActivationResponse> OnNext(MethodActivationRequest value)
        {
            var methodInfo = value.MethodInfo;
            var data = value.Data;

            return Create<MethodActivationResponse>(observer =>
            {
                return ToMethod(value.Key ?? Guid.NewGuid())
                .Subscribe(a =>
                {
                    observer.OnNext(new MethodActivationResponse(a));
                    observer.OnCompleted();
                });
            });

            Interfaces.Generic.IObservable<MethodNode> ToMethod(Guid guid)
            {
                var instance = CreateInstance(guid, methodInfo.Name, methodInfo.ReturnType, typeof(MethodNode));
                return instance;
           
                Interfaces.Generic.IObservable<MethodNode> CreateInstance(Guid parent, string name, Type returnType, Type type)
                {
                    if (type == null)
                    {
                        throw new ArgumentNullException("type");
                    }
                    return Create<MethodNode>(observer =>
                    {
                        CompositeDisposable composite = new();
                        Observe<FindPropertyResponse, FindPropertyRequest>(new(new Key(parent, name, returnType)))
                        .Subscribe(result =>
                        {
                            Key key = result.Keys.SingleOrDefault() as Key ?? throw new Exception("dfb 43 4df");
                            //if (guids.ContainsKey(key))
                            //      observer.OnNext(guids[key]);
                            var args = new object[] { key.Guid };
                            Observe<ObjectCreationResponse, ObjectCreationRequest>(new(type, new[] { typeof(BaseObject) }, args))
                            .Subscribe(response =>
                            {
                                var instance = response.Instance as MethodNode ?? throw new Exception("fg e4  ll;");
                                guids[key] = instance;
                                instance.Data = data;
                                instance.MethodInfo = methodInfo;
                                observer.OnNext(instance);
                                observer.OnCompleted();
                            },
                            () => observer.OnCompleted()).DisposeWith(composite);
                        }).DisposeWith(composite);
                        return composite;
                    });
                }
            }
        }
    }
}