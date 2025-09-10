using Cogs.Collections;
using DynamicData;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.ServiceLocation;
using Utility.Trees;
using Utility.Trees.Abstractions;
using ITreeRepository = Utility.Interfaces.Exs.ITreeRepository;

namespace Utility.Models
{
    public class NamedTree : Tree, IName
    {
        public NamedTree()
        {
            Data = Name;
        }
        public string Name { get; set; } = "root";
    }

    public class TreeResolver : IResolver<IReadOnlyTree>, IObservable<IReadOnlyTree>
    {
        private CompositeDisposable compositeDisposable = new CompositeDisposable();
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

        //public void Initialise()
        //{
        //    var tree = new NamedTree() { Key = "004cf888-a762-4149-a3b9-7a0911cdf1a9" };
        //    _factories["004cf888-a762-4149-a3b9-7a0911cdf1a9"] = new(() => tree);
        //    children2(tree)
        //        .Subscribe(a =>
        //        {
        //            _factories[(a as IGetKey).Key.ToString()] = new(() => a);
        //        })
        //        .DisposeWith(compositeDisposable);
        //}

        //IObservable<IReadOnlyTree> children(ITree node)
        //{
        //    return Observable.Create<IReadOnlyTree>(observer =>
        //    {
        //        int i = 0, j = 0;
        //        bool flag = false;
        //        return repository.Find((GuidKey)(node as IGetKey).Key)
        //            .Subscribe(async key =>
        //            {

        //                i++;
        //                //}
        //                if (key.HasValue)
        //                {
        //                    flag = true;
        //                    FindChild(node, key.Value.Guid).Subscribe(a =>
        //                    {
        //                        observer.OnNext(a);
        //                        children((ITree)a).Subscribe(observer.OnNext);
        //                    }, () => { });
        //                }
        //            }
        //            , () =>
        //            {

        //            });
        //    });
        //}

        public IObservable<IReadOnlyTree> Children(ITree node)
        {
            return Observable.Create<IReadOnlyTree>(observer =>
            {
                int i = 0, j = 0;
                bool flag = false;
                List<IReadOnlyTree> nodes = new();
                nodes.Add(node);
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

        //public IObservable<IReadOnlyTree> FindChild(ITree node, Guid guid)
        //{
        //    return Observable.Create<IReadOnlyTree>(observer =>
        //    {
        //        return repository
        //        .Find((GuidKey)(node as IGetKey).Key, guid: guid)
        //        .Subscribe(_key =>
        //        {
        //            activate(node, _key).Subscribe(a =>
        //            {
        //                node.Add(a);
        //                observer.OnNext(a);
        //                observer.OnCompleted();
        //            });
        //        });
        //    });
        //}

        IObservable<IReadOnlyTree> activate(ITree node, Structs.Repos.Key? _key)
        {
            return Observable.Create<IReadOnlyTree>(observer =>
            {
                return node.ToTree(DataActivator.Activate(_key)).ToObservable()
                    .Cast<IReadOnlyTree>()
                    .Subscribe(_new =>
                    {
                        (_new as ISetKey).Key = new GuidKey(_key.Value.Guid);

                        if (_key.HasValue == false)
                        {
                            throw new Exception("dde33443 21");
                        }

                        observer.OnNext(_new);
                    });
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

    public static class ModelResolverExtensions
    {
        public static System.Type toType(this object data)
        {
            return data is IGetType { } getType ? getType.GetType() : data.GetType();
        }

        public static string Name(this ITree node)
        {
            if (node.Data is IGetName getName)
            {
                return getName.Name;
            }
            return node.Data.ToString();
        }
    }
}
