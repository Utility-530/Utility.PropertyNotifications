using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cogs.Collections;
using DryIoc;
using Splat;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Meta;
using Utility.Models;
using Utility.Nodes;
using Utility.Nodify.Base.Abstractions;
using Utility.Repos;
using Utility.ServiceLocation;
using Utility.Trees.Abstractions;
using ITreeRepository = Utility.Interfaces.Exs.ITreeRepository;

namespace Utility.Nodify.Engine
{
    public class DiagramFactory : IDiagramFactory
    {
        Guid guid = new("004cf888-a762-4149-a3b9-7a0911cdf1a9");
        const string sqliteName = "O:\\source\\repos\\Utility\\Nodes\\Utility.Nodes.Demo.Editor\\Data\\first_7.sqlite";
        //private IContainer container;
        private ITreeRepository repository;
        private readonly ObservableDictionary<string, Lazy<IReadOnlyTree>> _factories = [];

        public DiagramFactory()
        {
            repository = new TreeRepository(sqliteName);
        }

        public Task Build(IDiagramViewModel diagram)
        {
            var tree = new NodeViewModel()
            {
                Key = guid.ToString(),
                Data = new RootDescriptor(typeof(Model)),
                Diagram = diagram
            };
            //_factories["004cf888-a762-4149-a3b9-7a0911cdf1a9"] = new(() => tree);

            var tcs = new TaskCompletionSource();
            children(tree)
                .Subscribe(item =>
                {
                    var node = Globals.Resolver.Resolve<IViewModelFactory>().CreateNode(item);
                    diagram.Nodes.Add(node);
                }, () =>
                {
                    tcs.SetResult();
                });

            return tcs.Task;

            IObservable<IReadOnlyTree> children(IReadOnlyTree node)
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

            IObservable<IReadOnlyTree> activate(ITree node, Structs.Repos.Key? _key)
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
