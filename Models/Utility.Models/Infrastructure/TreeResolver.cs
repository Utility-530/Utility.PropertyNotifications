using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Cogs.Collections;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.ServiceLocation;
using Utility.Trees.Abstractions;
using ITreeRepository = Utility.Interfaces.Exs.ITreeRepository;

namespace Utility.Models
{
    public class TreeResolver : IResolver<IReadOnlyTree>, IObservable<IReadOnlyTree>
    {
        private ITreeRepository repository = Globals.Resolver.Resolve<ITreeRepository>();
        private readonly ObservableDictionary<string, Lazy<IReadOnlyTree>> _factories = [];

        public TreeResolver()
        {
            //Initialise();
        }

        public void Register(string key, Func<IReadOnlyTree> factory)
        {
            _factories[key] = new(factory);
        }

        public IDisposable Subscribe(IObserver<IReadOnlyTree> observer)
        {
            foreach (var factory in _factories.Values)
            {
                observer.OnNext(factory.Value);
            }

            return Observable.FromEventPattern<NotifyDictionaryChangedEventArgs<string, Lazy<IReadOnlyTree>>>(
                handler => _factories.DictionaryChanged += handler,
                handler => _factories.DictionaryChanged -= handler)
                .Subscribe(eventPattern =>
                {
                    var e = eventPattern.EventArgs;
                    if (e.Action == NotifyDictionaryChangedAction.Add)
                    {
                        foreach (var newItem in e.NewItems)
                        {
                            observer.OnNext(newItem.Value.Value);
                        }
                    }
                });
        }

        public IReadOnlyTree this[string key] => _factories[key].Value;

        public IObservable<IReadOnlyTree> Children(ITree node)
        {
            return Observable.Create<IReadOnlyTree>(observer =>
            {
                List<IReadOnlyTree> nodes = [node];
                observer.OnNext(node);
                return repository.FindRecursive((GuidKey)(node as IGetKey).Key)
                    .Subscribe(async key =>
                    {
                        var _node = nodes.Single(a => (a as IGetKey).Key.ToString() == key.ParentGuid.ToString());

                        activate(_node as ITree, key).Subscribe(a =>
                        {
                            observer.OnNext(a);
                            nodes.Add(a);
                        });
                    }
                    , () =>
                    {
                        observer.OnCompleted();
                    });
            });
        }

        private IObservable<IReadOnlyTree> activate(ITree node, Structs.Repos.Key? _key)
        {
            return Observable.Create<IReadOnlyTree>(observer =>
            {
                var _new = (IReadOnlyTree)DataActivator.Activate(_key);
                (_new as ISetParent<IReadOnlyTree>).Parent = node;

                node.Add(_new);
                (_new as ISetKey).Key = new GuidKey(_key.Value.Guid);

                if (_key.HasValue == false)
                {
                    throw new Exception("dde33443 21");
                }

                observer.OnNext(_new);
                return Disposable.Empty;
            });
        }
    }

    internal static class DataActivator
    {
        public static object Activate(Structs.Repos.Key? a)
        {
            if (infos().SingleOrDefault() is { } constructorInfo)
            {
                return constructorInfo.Invoke([a.Value.Name]);
            }

            var _data = ActivateAnything.Activate.New(a.Value.Type);

            if (_data is ISetName sname)
            {
                if (_data is IGetName { Name: { } name })
                {
                    if (name != a.Value.Name)
                        sname.Name = a.Value.Name;
                }
                else
                    sname.Name = a.Value.Name;
            }
            return _data;

            IEnumerable<ConstructorInfo> infos() => from x in a.Value.Type.GetConstructors()
                                                    let p = x.GetParameters()
                                                    where p.Length == 1 && p[0].ParameterType == typeof(string)
                                                    select x;
        }
    }
}